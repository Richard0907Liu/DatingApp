
// DTO => Data transfer object
using System;
using System.ComponentModel.DataAnnotations;

namespace DatingApp_API.Dtos
{
  public class UserForRegisterDto
  {
    [Required]
    public string Username { get; set; }

    [Required]
    [StringLength(8, MinimumLength = 4, ErrorMessage = "You must specify password between 4 and 8 characters")]
    public string Password { get; set; }

    [Required]
    public string gender { get; set; }

    [Required]
    public string KnownAs {get; set;}  // 

    [Required]
    public DateTime DateOfBirth { get; set; }

    [Required]
    public string City {get; set;}

    [Required]
    public string Country {get; set;}
    public DateTime Created {get; set;}
    public DateTime LastActive {get; set;}

    // Create constructor for setting Date
    public UserForRegisterDto() {
      Created = DateTime.Now;
      LastActive = DateTime.Now;
    }

  }
}