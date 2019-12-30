using Microsoft.EntityFrameworkCore;
using DatingApp.API.Models;

namespace DatingApp.API.Data
{
  // How many DbSet<>, that would show how many table in DB
  public class DataContext : DbContext
  {
    public DataContext(DbContextOptions<DataContext> options) : base(options) { }

    // When changing inside our Models either we create a new class ro modify properties
    // ,need to add a new migration and then apply that to our database
    public DbSet<Value> Values { get; set; }

    public DbSet<User> Users { get; set; }

    // Code first to Add Photos, named for table inside DB, and then create a new migration
    public DbSet<Photo> Photos { get; set; }

    public DbSet<Like> Likes {get; set; }
    public DbSet<Message>  Messages {get; set; }  // Need to configure this relationship in entity framework

    // When "many to many" relationship, has to configure those relationships
    // Want to use "Fluent API" to explicitly tell about the relationship (many to many, not one to many), need to override
    // In DB, there are not Liker and Likee, just LikerId and LikeeId
    protected override void OnModelCreating(ModelBuilder builder)
    {
      // For like list, many to many
      // HasKey(), HasOne etc are the "Fluent API" to explicitly tell about the relationship
      builder.Entity<Like>()
        .HasKey(k => new {k.LikerId, k.LikeeId}); // both of them as primary key

      // For likee behavior
      builder.Entity<Like>()
        // this is where we use the fluent API to define the relationship and configure entity framework
        // one likee has many likers
        .HasOne(u => u.Likee)
        .WithMany(u => u.Likers)
        // Need to specify a foreign key which is going to be going back to our users table.
        .HasForeignKey(u => u.LikeeId)
        // specify "Ondelete behaviour" because we don't want the deletion of like 
        // to have a cascading deletion of a user.
        .OnDelete(DeleteBehavior.Restrict);

      // For liker behavoir
      builder.Entity<Like>()
        .HasOne(u => u.Liker)
        .WithMany(u => u.Likees)
        .HasForeignKey(u => u.LikerId)
        .OnDelete(DeleteBehavior.Restrict);

      /////// For messages, many to many
      ///////
      builder.Entity<Message>()
        .HasOne(u => u.Sender) 
        .WithMany(m => m.MessageSent) // one to many, and the other side also one to many => many to many
        .OnDelete(DeleteBehavior.Restrict);

      builder.Entity<Message>()
      .HasOne(u => u.Recipient) 
      .WithMany(m => m.MessageReceived) // one to many, and the other side also one to many => many to many
      .OnDelete(DeleteBehavior.Restrict);

    }
  }
}