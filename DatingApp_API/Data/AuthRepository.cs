using System;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp.API.Models;
using Microsoft.EntityFrameworkCore;

namespace DatingApp_API.Data
{
  public class AuthRepository : IAuthRepository
  {
    private readonly DataContext _context;

    public AuthRepository(DataContext context)  // DataContext => Depedency Inject
    {
      _context = context;
    }
    public async Task<User> Login(string username, string password)
    {
      // Need to include "photos" for getting photoUrl
      var user = await _context.Users.Include(p => p.Photos).FirstOrDefaultAsync(x => x.Username == username);

      if (user == null)
        return null;
      // Check passwordHash
      if (!VerifyPasswordHash(password, user.PasswordHash, user.PasswordSalt))
        return null; // If password hash is not authorized

      // If verify successfully
      return user;
    }

    private bool VerifyPasswordHash(string password, byte[] passwordHash, byte[] passwordSalt)
    {   // Kind of reverse CreatePasswordHash()
      using (var hmac = new System.Security.Cryptography.HMACSHA512(passwordSalt))  // passwordSalt as a key to comoupte hash and then get password hash
      { // computeHash is byte[]
        var computeHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
        // Check computeHash byte[] and paswordHash byte[] are the same
        for (int i = 0; i < computeHash.Length; i++)
        {
          if (computeHash[i] != passwordHash[i])
            return false;
        }
      }
      return true;
    }

    public async Task<User> Register(User user, string password)  // password need to be converted for passwordHash and passwordSalt
    {
      byte[] passwordHash, passwordSalt;
      // pass passwordHash and passwordSalt as "reference" not value
      // pass a reference to byte[] passwordHash
      CreatePasswordHash(password, out passwordHash, out passwordSalt);  // get hash and salt values for token

      user.PasswordHash = passwordHash;
      user.PasswordSalt = passwordSalt;

      await _context.Users.AddAsync(user); // Add username password to database
      await _context.SaveChangesAsync(); // Save value to database

      return user;
    }

    private void CreatePasswordHash(string password, out byte[] passwordHash, out byte[] passwordSalt)
    {
      using (var hmac = new System.Security.Cryptography.HMACSHA512())
      {
        passwordSalt = hmac.Key;
        passwordHash = hmac.ComputeHash(System.Text.Encoding.UTF8.GetBytes(password));
      }
    }

    public async Task<bool> UserExists(string username)
    {   // AnyAsync, want to compare this username against any other user
      if (await _context.Users.AnyAsync(x => x.Username == username))
        return true; // It has found a matching username in database

      return false;
    }
  }
}