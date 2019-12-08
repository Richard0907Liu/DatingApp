using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp_API.Data
{
  // Create Repository pattern for managing method to get data from database
  // 
  public interface IAuthRepository  // It must be "Interface"
  {   // Task, represent an asynchronous operation that not return a value
    Task<User> Register(User user, string password);

    Task<User> Login(string username, string password);
    Task<bool> UserExists(string username); // to check the username is already in database or not
  }
}