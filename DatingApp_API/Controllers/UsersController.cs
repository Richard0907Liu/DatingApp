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
  [Route("api/[controller]")]
  [ApiController]
  public class UsersController : ControllerBase
  {
    private readonly IDatingRepository _repo;
    public readonly IMapper _mapper;  // Mapping models into Database

    public UsersController(IDatingRepository repo, IMapper mapper)
    {
      _repo = repo;
      _mapper = mapper;
    }

    // For pagination, userParams => that is "pageSize"
    // Have to add [FromQuery] in Get method if want to send params from URL
    [HttpGet]
    public async Task<IActionResult> GetUsers([FromQuery] UserParams userParams)  // model from UserParams, ?pageNumber=1&pageSize=3
    { 
      // Get the user's I.D. from their token, for filtering gender
      var currentUserId =  int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value);
      // Get user info
      var userFromRepo = await _repo.GetUser(currentUserId);
      // set UserId into userParams
      userParams.UserId = currentUserId;

      // Get user's gender
      // if selected gender is null, set the oppsite gender of user by default
      // if user specifies gender not process this if condition
      if(string.IsNullOrEmpty(userParams.Gender))  
      { 
        userParams.Gender = userFromRepo.Gender == "male" ? "female" : "male";
      }


       // userParams from query string
      var users = await _repo.GetUsers(userParams);

      // Put vairable users into Map<> and then want to get IEnumerable<UserForListDto>
      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      // Return pagination infomatin in Headers, let our client application know about the pagination is 
      // so adding the pagination to the response headers.

      // Because we're inside an API controller we have access to the "Response"
      // And because we've also written an "AddPagination() in Extensions.cs" to the HTTP response.
      // add users as params in AddPagination(), users include pageSize, totalItems etc..
      //Console.WriteLine("users.TotalCount in UsersController::::::" + users.TotalCount);  //0
      Response.AddPagination(users.CurrentPage, users.PageSize,
          users.TotalCount, users.TotalPages);

      return Ok(usersToReturn);
      // return Ok(users); // Not good, because return passwordHash, passwordSalt etc 
    }

    [HttpGet("{id}", Name = "GetUser")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);

      // Map(), this method job is to execute a mapping from the source objects to destination object.
      // map user (source) into "UserForDetailedDto" (destination) and not map personal info 
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);

      return Ok(userToReturn);
      //return Ok(user);  // Not good, because return passwordHash, passwordSalt etc 
    }

    [HttpPut("{id}")]
    public async Task<IActionResult> UpdateUser(int id, UserForUpdateDto userForUpdateDto)
    {
      // User form controllerBase.User, that store a lots variables in ClaimTypes
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized(); // If the id is not the same id from server

      var userFromRepo = await _repo.GetUser(id);

      // Write userForUpdateDto into userFromRepo and then save into DB
      _mapper.Map(userForUpdateDto, userFromRepo);

      if (await _repo.SaveAll())
        return NoContent();

      throw new Exception($"Updating user {id} failed on save");
    }

    [HttpPost("{id}/like/{recipientId}")] // recipientId that the user like
    public async Task<IActionResult> LikeUser(int id, int recipientId)
    {
      if (id != int.Parse(User.FindFirst(ClaimTypes.NameIdentifier).Value))
        return Unauthorized();

      var like = await _repo.GetLike(id, recipientId);
      // Check user's like exists or not
      if(like != null) // already likes
        return BadRequest("You already like this user");

      // see if the recipients exists or not.
      if(await _repo.GetUser(recipientId) == null)
        return NotFound("Cannot find this memeber");

      // Create a new like pair
      like = new Like 
      {
        LikerId = id,
        LikeeId = recipientId
      };

      _repo.Add<Like>(like);

      if(await _repo.SaveAll())
        return Ok();
      
      return BadRequest("Failed to like user");
    }
  } 
}