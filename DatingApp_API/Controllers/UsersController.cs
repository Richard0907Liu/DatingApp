using System.Collections.Generic;
using System.Threading.Tasks;
using AutoMapper;
using DatingApp_API.Data;
using DatingApp_API.Dtos;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp_API.Controllers
{
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

      var usersToReturn = _mapper.Map<IEnumerable<UserForListDto>>(users);

      return Ok(usersToReturn);
      // return Ok(users); // Not good, because return passwordHash, passwordSalt etc 
    }

    [HttpGet("{id}")]
    public async Task<IActionResult> GetUser(int id)
    {
      var user = await _repo.GetUser(id);

      // Map(), this method job is to execute a mapping from the source objects to destination object.
      // map user (source) into "UserForDetailedDto" (destination) and not map personal info 
      var userToReturn = _mapper.Map<UserForDetailedDto>(user);

      return Ok(userToReturn);
      //return Ok(user);  // Not good, because return passwordHash, passwordSalt etc 
    }
  }
}