using System;

namespace DatingApp_API.Dtos
{
  public class PhotosForDetailedDto
  {
    public int Id { get; set; }
    public string Url { get; set; }
    public string Description { get; set; }

    public DateTime DateAdded { get; set; }

    // Make this photo on the member card
    public bool IsMain { get; set; }
  }
}