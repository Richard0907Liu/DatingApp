using System;

namespace DatingApp.API.Models
{
  public class Photo
  {
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }

    public DateTime DateAdded { get; set; }

    // Make this photo on the member card
    public bool IsMain { get; set; }

    // Need photoId, because after saving photos into Cloudinary, the Cloudinary would send photo public Id into DB
    public string PublicId { get; set; }  // Add a new model, need to add a new migration

    // Connect back to User, when User is deleted, this Photo is also deleted, cascade delete
    public User User { get; set; }

    public int UserId { get; set; }

  }
}