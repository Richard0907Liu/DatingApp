using System;
using System.Collections.Generic;


// Need to go to DataContext to set up DbSet<>
namespace DatingApp.API.Models
{
  public class User
  {
    // Create first
    public int Id { get; set; }

    public string Username { get; set; }

    // password Hash and password Salt, store hash and salt
    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }

    // Create later 
    public string Gender { get; set; }
    public DateTime DateOfBirth { get; set; }
    public string KnowAs { get; set; }
     public string KnownAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }

    public string Country { get; set; }

    // Use ICollection<Photo>, each user has a collection of photos al part of profile
    public virtual ICollection<Photo> Photos { get; set; }
    // Link to Photos, and Photos has to connect User back, want the "cascade delete"

    // a person has many Liker
    public virtual ICollection<Like> Likers {get; set;}

    // a person has been liked by many people
    public virtual ICollection<Like> Likees {get; set;}
    public virtual ICollection<Message> MessageSent { get; set; }
    public virtual ICollection<Message> MessageReceived { get; set; }
  }
}