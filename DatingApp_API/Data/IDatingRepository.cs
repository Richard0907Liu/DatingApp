using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Models;
using DatingApp_API.Helpers;

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

    // Return pageList for pagination, pagesize, totalitems, CurrentPage etc..
    // need to put params into here
    Task<PagedList<User>> GetUsers(UserParams userParams);
    
    // Old GetUsers()
    // Task<IEnumerable<User>> GetUsers(); // Old one
    
    Task<User> GetUser(int id);

    // For Getting Photos info including photo uri
    Task<Photo> GetPhoto(int id);
    Task<Photo> GetMainPhotoForUser(int userId);

  }

}