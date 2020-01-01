using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using DatingApp.API.Data;
using DatingApp_API.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

namespace DatingApp_API
{
  public class Program
  {
    public static void Main(string[] args)
    {
      // store a reference to the create web host builder but will defer running it till later.
      // host would run after seeding data from Seed.cs
      var host = CreateHostBuilder(args).Build();  // old one includes Run(),    CreateHostBuilder(args).Build().Run();

      // because we want to dispose of our data context as soon as we've seeded our users 
      // then we're going to wrap this inside a "using statement"
      using (var scope = host.Services.CreateScope())
      {
        var services = scope.ServiceProvider;
        try
        {
          var context = services.GetRequiredService<DataContext>();
          // Use database migrate command
          // Migrate(), it will create the DB if it does not already exist
          context.Database.Migrate(); // set this context to has Mirgrate() command
                                      // Pass context to Seed.SeedUsers, finally Seed will migrate and save data in DB
          Seed.SeedUsers(context);


        }
        catch (Exception ex)
        {
          var logger = services.GetRequiredService<ILogger<Program>>();
          logger.LogError(ex, "An error occured during migration!");
        }
      }

      // In here, finally run host
      host.Run();
    }

    public static IHostBuilder CreateHostBuilder(string[] args) =>
        Host.CreateDefaultBuilder(args)
          .ConfigureWebHostDefaults(webBuilder =>
          {
            webBuilder.UseStartup<Startup>();
          });
  }
}
