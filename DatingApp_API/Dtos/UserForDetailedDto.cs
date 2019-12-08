using System;
using System.Collections.Generic;
using DatingApp.API.Models;

namespace DatingApp_API.Dtos
{
  public class UserForDetailedDto
  {
    public int Id { get; set; }
    public string Username { get; set; }
    public string Gender { get; set; }
    public int Age { get; set; }
    public string KnowAs { get; set; }
    public DateTime Created { get; set; }
    public DateTime LastActive { get; set; }
    public string Introduction { get; set; }
    public string LookingFor { get; set; }
    public string Interests { get; set; }
    public string City { get; set; }
    public string Country { get; set; }
    public string PhotoUrl { get; set; }

    // "Navigation property", allows for navigation from one end of an association to the other end. 
    // From one Dto (model) to another Dto (model)
    public ICollection<PhotosForDetailedDto> Photos { get; set; }
  }
}