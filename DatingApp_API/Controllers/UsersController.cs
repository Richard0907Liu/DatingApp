using System;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;
using AutoMapper;
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

    [HttpGet]
    public async Task<IActionResult> GetUsers()
    {
      var users = await _repo.GetUsers();

      // Put vairable users into Map<> and then want to get IEnumerable<UserForListDto>
      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

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
  }
}