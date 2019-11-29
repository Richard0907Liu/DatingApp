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

namespace DatingApp_API.Controllers
{
  [Route("api/[controller]")]
  [ApiController]  // => Imply http use [FromBody], for checking input Validation 
  public class AuthController : ControllerBase  // Register and login
  {
    private readonly IAuthRepository _repo;
    private readonly IConfiguration _config;
    public AuthController(IAuthRepository repo, IConfiguration config) // Dependency Injection
    {
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

      // Create a new username
      var userToCreate = new User
      {
        Username = UserForRegisterDto.Username  // Username is of User in Models
      };

      var createdUser = await _repo.Register(userToCreate, UserForRegisterDto.Password);
      return StatusCode(201);
    }

    // Let logged in users to get token and maintain the authorization
    [HttpPost("login")]
    public async Task<IActionResult> Login(UserForLoginDto UserForLoginDto)
    {
      var userFromRepo = await _repo.Login(UserForLoginDto.Username.ToLower(), UserForLoginDto.Password);
      // Check username already exists in database 
      if (userFromRepo == null) // if username doesn't exist in DB
        return Unauthorized();

      // Build a token and return to user, contian userId, userName, token
      // Once get token, save some info inside server, not need to get user name and user id every request.
      var claims = new[]
      {           // Get id and store in server
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

      // return token to client
      // use tokenHandler to write token and save into token
      return Ok(new
      {
        token = tokenHandler.WriteToken(token)
      });

    }
  }
}