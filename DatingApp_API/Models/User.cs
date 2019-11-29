namespace DatingApp.API.Models
{
  public class User
  {
    public int Id { get; set; }

    public string Username { get; set; }

    // password Hash and password Salt, store hash and salt
    public byte[] PasswordHash { get; set; }

    public byte[] PasswordSalt { get; set; }
  }
}