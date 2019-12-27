using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using DatingApp_API.Helpers;
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


    public async Task<Photo> GetMainPhotoForUser(int userId)
    {
      // Where => return many user's photos, and then add "FirstOrDefaultAsync" just return "one photo"
      return await _context.Photos.Where(u => u.UserId == userId).FirstOrDefaultAsync(p => p.IsMain);
    }

    public async Task<Photo> GetPhoto(int id)
    {
      var photo = await _context.Photos.FirstOrDefaultAsync(p => p.Id == id);

      return photo;
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

    // For pagination
    // Paging information such as page number on the page size, how many items on the page and 
    // and the total count of items as well as total pages
    public async Task<PagedList<User>> GetUsers(UserParams userParams)
    {
      // not return User, return PagedList
      // ToList() is an async method, so here not use async
      // Order By lastActive as by default
      var users = _context.Users.Include(p => p.Photos)
        .OrderByDescending(u => u.LastActive).AsQueryable();  // make users as Queryable, so add AsQueryable()

      // Not show current user, 
      users = users.Where(u => u.Id != userParams.UserId);
      // show oppsite gender
      users = users.Where(u => u.Gender == userParams.Gender);

      // For the liker list
      if(userParams.Likers)
      {
          var userLikers = await GetUserLikes(userParams.UserId, userParams.Likers);
          // When userLikers "Contain" u.Id, that would be selected.
          users = users.Where(u => userLikers.Contains(u.Id));

      }
      // For the likee list
      if(userParams.Likees)
      {
        var userLikees = await GetUserLikes(userParams.UserId, userParams.Likers);
          // When userLikees "Contain" u.Id, that would be selected.
          users = users.Where(u => userLikees.Contains(u.Id));
      }


      // Calculate what we want in return.
      if(userParams.MinAge != 18 || userParams.MaxAge != 99) // user specify age 
      {
        // Need to calculate members' age 
        // use minus (-userParams.MaxAge) because calculate the earliest DOB
        var minDob = DateTime.Today.AddYears(-userParams.MaxAge -1 );
        // find latest DOB 
        var maxDob = DateTime.Today.AddYears(-userParams.MinAge);

        users = users.Where(u => u.DateOfBirth >= minDob && u.DateOfBirth <= maxDob);
      }

      // Order filter
      if(!string.IsNullOrEmpty(userParams.OrderBy))
      {
        switch(userParams.OrderBy)
        {
          case "created":
            users = users.OrderByDescending(u => u.Created);
            break;
          default:
            users = users.OrderByDescending(u => u.LastActive);
            break;
        }
      }


      // we've created a static method, CreateAsync() in PagedList.cs.
      // First param is IQueryable. IQueryable means params can be use with Linq
      return await PagedList<User>.CreateAsync(users, userParams.PageNumber, userParams.PageSize);
      // And then go to UsersContorller

    }

    // Old one, no pagination
    // public async Task<IEnumerable<User>> GetUsers()  
    // {
    //   // ToListAsync() => get a list of users
    //   var users = await _context.Users.Include(p => p.Photos).ToListAsync();
    //   return users;
    // }

    // Create private method for like list
    // id => userId
    private async Task<IEnumerable<int>> GetUserLikes(int id, bool likers)
    {
      // gonna return list of likers, list of likees from logged user
      // 
      var user = await _context.Users
        .Include(x => x.Likers)
        .Include(x => x.Likees)
        .FirstOrDefaultAsync(u => u.Id == id);

        if (likers) // likers exist
        {
          // want to return all of the users have liked the currently logged-in user.
          // In order to return a list of integers we need to use only select
          // Using select, this will return a collection of integers instead of a collection of users
          // return a list the likerId
          return user.Likers.Where(u => u.LikeeId == id).Select(i => i.LikerId);
        }
        else 
        {
          return user.Likees.Where(u => u.LikerId == id).Select(i => i.LikeeId);
        }
      
    }

    public async Task<bool> SaveAll()
    {
      // if _context.SaveChangesAsync() <= 0, means saving back is not successfully
      return await _context.SaveChangesAsync() > 0;
    }

     public async Task<Like> GetLike(int userId, int recipientId)
    {
      // If like doesn't exist, then return Null; if exist, it returns like
      return await _context.Likes.FirstOrDefaultAsync(u => 
        u.LikerId == userId && u.LikeeId == recipientId);
    }

  }
}