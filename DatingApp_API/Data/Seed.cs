using System.Collections.Generic;
using System.Linq;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Newtonsoft.Json;

namespace DatingApp_API.Data
{
  // This class in order to take json data and then seed this data into DB
  // want to take these Jason objects and deserialize them 
  // into user objects to match our models.
  public class Seed
  {
    // we'll call this seed users and we're making this a "static class" because
    // we don't want to create a new instance of this class.
    public static void SeedUsers(DataContext context)
    {
      // First, going to check DB, is there data in there
      // Any() is from LINQ
      if (!context.Users.Any())
      {
        // To read json file in this program
        var userData = System.IO.File.ReadAllText("Data/UserSeedData.json");
        // Convert userData (string type) into user "object "
        // And then we can use loop function to read this object
        var users = JsonConvert.DeserializeObject<List<User>>(userData);

        // Password need to be created for password Hash and password Salt
        // so need AuthRepository
        foreach (var user in users)
        {
          byte[] passwordhash, passwordSalt;

          // Not need to worry about aync.
          // Below method is going to be called our application very first starts up and 
          // it's going to be called once when our application starts up.
          // so there's no way that any concurrent requests
          CreatePasswordHash("password", out passwordhash, out passwordSalt);

          user.PasswordHash = passwordhash;
          user.PasswordSalt = passwordSalt;
          user.Username = user.Username.ToLower();
          // Add user into context
          context.Users.Add(user);
        }
        // Add all users into context and then save context
        context.SaveChanges();

        // What we're going to do is we're going to 
        // add this to our program into Program.cs.

      }
    }

    // Need make CreatePasswordHash() as static, then static SeedUsers can use CreatePasswordHash()
    private static void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }


  }
}

