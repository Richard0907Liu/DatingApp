using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp_API.Data;
using DatingApp_API.Dtos;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using AutoMapper;

namespace DatingApp_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]  // => Imply http use [FromBody], for checking input Validation 
  public class AuthController : ControllerBase  // Register and login
  {
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;
    private readonly IMapper _mapper;
    public AuthController(IAuthRepository repo, IConfiguration config, IMapper mapper) // Dependency Injection
    {
      _mapper = mapper;
      _config = config;
      _repo = repo;

    }

    [HttpPost("register")]
    //public async Task<IActionResult> Register(string username, string password)
    //                      Register([FromBody] UserForRegisterDto UserForRegisterDto)
    public async Task<IActionResult> Register(UserForRegisterDto UserForRegisterDto)  // Use Dto
    {
      // validate request, later write
      // If no [ApiController], write below code for validation
      // if (!ModelState.IsValid)
      //   return BadRequest(ModelState);


      UserForRegisterDto.Username = UserForRegisterDto.Username.ToLower();

      // Check this username already exist or not 
      if (await _repo.UserExists(UserForRegisterDto.Username))
        return BadRequest("Username already exists");

      var userToCreate = _mapper.Map<User>(UserForRegisterDto); // User as destination, UserForRegisterDto as source
      // Old one.Create a new username
      // var userToCreate = new User
      // {
      //   Username = UserForRegisterDto.Username  // Username is of User in Models
      // };

      var createdUser = await _repo.Register(userToCreate, UserForRegisterDto.Password);

      // not want to include passwordHash and passwordSalt, so Map<>
      var userToReturn = _mapper.Map<UserForDetailedDto>(createdUser);

      // need to specify the string of the route name. have a method in UsersController to getAUser
      // first param "name of route" GetUser method in UsersController, 
      //second param call this controler with value {conroller, value } and send back as location as Header 
      // Third param in here is what we need to return the value or object from the controller
      return CreatedAtRoute("GetUser", new {controller = "Users", id = createdUser.Id}, userToReturn);
      //return StatusCode(201); old , just to cheat the postman and front-end
    }

    // Let logged in users to get token and maintain the authorization
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto)
    {

      // Make throw Exception as gloabal exception
      // throw new Exception("Computer says no!!!!"); For TEST

      // // userFromRepo get all "user" info from Login(), and then create new "Claim" to save Id as ClaimTypes.NameIdentifier
      var userFromRepo = await _repo.Login(UserForLoginDto.Username.ToLower(), UserForLoginDto.Password);
      // Check username already exists in database 
      if (userFromRepo == null) // if username doesn't exist in DB
        return Unauthorized();

      // Build a token and return to user, contian userId, userName, token
      // Once get token, save some info inside server, not need to get user name and user id every request.
      var claims = new[]
      {           // Get Id and store in Server
          new Claim(ClaimTypes.NameIdentifier, userFromRepo.Id.ToString()),
          // Get username and store in server
          new Claim(ClaimTypes.Name, userFromRepo.Username)
        };

      // Next step, need a key to sign for token
      // hash, byte[]
      // Use GetSection of IConfiguration storing key for couple different places, inject "IConfiguration"
      // .Value to get value of this token
      // Need to go to appsettings.json to create "AppSettings:Token"  
      var key = new SymmetricSecurityKey(Encoding.UTF8
        .GetBytes(_config.GetSection("AppSettings:Token").Value));

      // Get credential, use string algorithm to "hash" the abvoe key
      var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha512Signature);

      // Create token description 
      // Save cred into SingingCredential
      var tokenDescriptor = new SecurityTokenDescriptor
      {
        Subject = new ClaimsIdentity(claims),
        Expires = DateTime.Now.AddDays(1),
        SigningCredentials = creds
      };
      // Need token handler
      var tokenHandler = new JwtSecurityTokenHandler();
      // Use CreateToken to get JWT Token
      var token = tokenHandler.CreateToken(tokenDescriptor);

      var user = _mapper.Map<UserForListDto>(userFromRepo);

      // return token to client
      // use tokenHandler to write token and save into token
      return Ok(new
      {
        token = tokenHandler.WriteToken(token),
        // return user info, first need include "photos" in AuthRepository
        user
      });
    }
  }
}