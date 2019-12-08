using System.Collections.Generic;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
  // Need to Add this DatingRepository into Startup.cs
  public class DatingRepository : IDatingRepository
  {
    private readonly DataContext _context;
    public DatingRepository(DataContext context)
    {
      _context = context;

    }

    public void Add<T>(T entity) where T : class
    {
      // we've " not used an async method" here which issues in synchronous code because when we add something
      // into our context " we're not actually querying or doing anything" with the database at this point.
      // This is going to be saved in memory until we actually save our changes back to database 
      _context.Add(entity);
    }

    public void Delete<T>(T entity) where T : class
    {
      // not use async, the same reason like Add()
      _context.Remove(entity);
    }

    public async Task<User> GetUser(int id)
    {
      // Include our photos to return as well.
      // Photos are considered "navigation properties", Users class and to get Photo class
      // Users model dosen't automatically include Photos model
      // Make Users model include Phtots model and then search id
      // populate Users model with Photos Model
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(u => u.Id == id);
      // if no id, FirstOrDefaultAsync returns null
      return user;
    }

    public async Task<IEnumerable<User>> GetUsers()
    {
      // ToListAsync() => get a list of users
      var users = await _context.Users.Include(p => p.Photos).ToListAsync();

      return users;
    }

    public async Task<bool> SaveAll()
    {
      // if _context.SaveChangesAsync() <= 0, means saving back is not successfully
      return await _context.SaveChangesAsync() > 0;
    }
  }
}