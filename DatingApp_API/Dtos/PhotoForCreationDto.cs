using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp_API.Dtos
{
  public class PhotoForCreationDto
  {
    public string Url { get; set; }

    // Use IFormFile to upload the file, represents a file sent with HTTP request.
    public IFormFile File { get; set; }  // IFormFile name has to match The Key in postman or Other Input key
    public string Description { get; set; }
    public DateTime DateAdded { get; set; }
    // This PublicId is got back from Cloudinary
    public string PublicId { get; set; }

    // Add constructor, can add the datetime inside here 
    public PhotoForCreationDto()
    {
      DateAdded = DateTime.Now;
    }

  }
}