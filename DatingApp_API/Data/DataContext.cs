using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // When changing inside our Models either we create a new class ro modify properties
    // ,need to add a new migration and then apply that to our database
    public DbSet<Value> Values { get; set; }

    public DbSet<User> Users { get; set; }

  }
}