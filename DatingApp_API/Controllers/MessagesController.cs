using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp.API.Models;
using DatingApp_API.Data;
using DatingApp_API.Dtos;
using DatingApp_API.Helpers;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp_API.Controllers
{
  // Perform custom action filer, so user everytime use those method well automatically record lastActive
  [ServiceFilter(typeof(LogUserActivity))]
  [Authorize]
  [Route("api/users/{userId}/[controller]")]
  [ApiController]
  public class MessagesController : ControllerBase
  {
    private readonly IDatingRepository _repo;
    private readonly IMapper _mapper;
    public MessagesController(IDatingRepository repo, IMapper mapper)
    {
      _mapper = mapper;
      _repo = repo;
    }

    [HttpGet("{id}", Name="GetMessage")] // this id is a person who get a message from the logged user.
    public async Task<IActionResult> GetMessage(int userId, int id)
    {
        // User form controllerBase.User, that store a lots variables in ClaimTypes
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized(); // If the id is not the same id from server

        var messageFromRepo = await _repo.GetMessage(id);

        if(messageFromRepo == null)
            return NotFound();
        
        return Ok(messageFromRepo);
    }

    [HttpGet]
    public async Task<IActionResult> GetMessagesForUser(int userId, 
        [FromQuery] MessageParams messageParams)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

        messageParams.UserId = userId;

        // get Enumerable messages
        var messagesFromRepo = await _repo.GetMessagesForUser(messageParams);

        var messages = _mapper.Map<IEnumerable<MessageToReturnDto>>(messagesFromRepo);

        // Because we're returning our pagination as well as in this particular request, 
        // add "pagination" to our response Headers
        // AddPagination() in Extension.cs
        Response.AddPagination(messagesFromRepo.CurrentPage, 
            messagesFromRepo.PageSize, messagesFromRepo.TotalCount, messagesFromRepo.TotalPages);
        
        return Ok(messages); // all messages the user got

    }

    // For getting message thread
    // Have to add "thread/", because program cannot tell between [HttpGet("{recipientId")] and [HttpGet("{id}", Name="GetMessage")]
    [HttpGet("thread/{recipientId}")]  // Get conversation only between two users.
    public async Task<IActionResult> GetMessageThread(int userId, int recipientId)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
        
        var messageFromRepo = await _repo.GetMessageThread(userId, recipientId);
        
        // Because these are returning the message entities we're going to need 
        // to map these into our message to return.
        var messageThread = _mapper.Map<IEnumerable<MessageToReturnDto>>(messageFromRepo);
        
        return Ok(messageThread);

    }


    [HttpPost] // not need messageId, system automatically generate messageId
    public async Task<IActionResult> CreateMessage(int userId, 
        MessageForCreationDto messageForCreationDto)
    {
        var sender = await _repo.GetUser(userId);  // get sender's all info for instant message communication

        if (sender.Id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

        // Set up userId
        messageForCreationDto.SenderId = userId;
        // Get recipient's info
        var recipient = await _repo.GetUser(messageForCreationDto.RecipientId);

        if(recipient == null)
            return BadRequest("Could not find user");

        // Want to map our messageForCreationDto into our message class
        var message = _mapper.Map<Message>(messageForCreationDto);

        _repo.Add(message);

        // var messageToReturn = _mapper.Map<MessageForCreationDto>(message); 
        // MessageForCreationDto not include many properties like photUrl, knownAs etc. 
        // It cannot return proper info to front end

      
        if(await _repo.SaveAll()) 
        {
            // let me get the correct Id in return from the server when we create a new message
            // 
            var messageToReturn = _mapper.Map<MessageToReturnDto>(message); 
            return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, messageToReturn);  
            
            // Use "message" as param, that would return passwordHash and passwordSalt. It's not suitable
            //return CreatedAtRoute("GetMessage", new {userId, id = message.Id}, message);  
        }

        throw new Exception("Creating the message failed on save");
    }

    // Delete the message but use "POST" method, becuse only both sides delete the message, this message would be deleted;
    [HttpPost("{id}")]
    public async Task<IActionResult> DeleteMessage(int id, int userId)
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();

        var messageFromRepo = await _repo.GetMessage(id);

        // Sender deletes this message
        if(messageFromRepo.SenderId == userId)
            messageFromRepo.SenderDeleted = true;
        // Recipient deletes this message
        if(messageFromRepo.RecipientId == userId)
            messageFromRepo.RecipientDeleted = true;
        // Both sides delete this message
        if(messageFromRepo.SenderDeleted && messageFromRepo.RecipientDeleted)
            _repo.Delete(messageFromRepo);

        if(await _repo.SaveAll())
            return NoContent();
        
        throw new Exception("Error deleting the message");
    }

    // Adding the Mark as Read functionality
    [HttpPost("{id}/read")]
    public async Task<IActionResult> MarkMessageAsRead(int userId, int id)  // id => messageId
    {
        if (userId != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
            return Unauthorized();
        
        var message = await _repo.GetMessage(id);
        // Check 
        if(message.RecipientId != userId)
            return Unauthorized();

        message.IsRead = true;
        message.DateRead = DateTime.Now;

        await _repo.SaveAll();

        return NoContent();
    }

  }
}