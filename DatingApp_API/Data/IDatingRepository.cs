using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;

namespace DatingApp_API.Data
{
  public interface IDatingRepository
  {
    // T represent a type of User or a type of Photo
    // Use "where" is a generic constrain
    void Add<T>(T entity) where T : class;
    void Delete<T>(T entity) where T : class;

    // The Task class represents a single operation that does not return a value and that usually executes asynchronously.
    Task<bool> SaveAll();

    Task<IEnumerable<User>> GetUsers();

    Task<User> GetUser(int id);
  }
}