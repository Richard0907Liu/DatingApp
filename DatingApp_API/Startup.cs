using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.HttpsPolicy;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;

using DatingApp.API.Data;
using DatingApp_API.Data;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System.Net;
using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using DatingApp_API.Helpers;
using AutoMapper;

namespace DatingApp_API
{
  public class Startup
  {
    public Startup(IConfiguration configuration)
    {
      Configuration = configuration;
    }

    public IConfiguration Configuration { get; }

    // This method gets called by the runtime. Use this method to add services to the container.
    public void ConfigureServices(IServiceCollection services)
    {
      services.AddDbContext<DataContext>(x => x.UseSqlite(
            // Get info from appsettings.json
            Configuration.GetConnectionString("DefaultConnection"))
      );

      // AddControllers
      // Use NewtonsoftJson that have feature we want. Not use System.Text.Json
      // Solved !? show wrong on Postman, 500Internal Server Error???
      services.AddControllers().AddNewtonsoftJson(opt =>
      {
        // Solved, Newtonsoft.Json.JsonSerializationException: Self referencing loop detected for property    
        opt.SerializerSettings.ReferenceLoopHandling =
        Newtonsoft.Json.ReferenceLoopHandling.Ignore;
      });

      // Old one, replaced with AddNewtonsoftJson()
      // using System.Text; but it is not quite ready form prime time.
      // And it doesn't include features we want 
      //services.AddControllers();
      // END AddControllers

      // For CORS
      services.AddCors(); // Use this as middleware

      // Link to Cloudinary
      // Need to specify type "CloudinarySettings" and then get info from CloudinarySettings in appsettings.json
      services.Configure<CloudinarySettings>(Configuration.GetSection("CloudinarySettings"));

      //

      // AutoMapper
      services.AddAutoMapper(typeof(DatingRepository).Assembly);  // go to UsersController
      // END AutoMapper


      // services.AddScoped()
      // Add Repository into Startup, different method for creating instance of repository
      // AddSingleton() to create instance of our repository throughout the application
      // AddTransient(), is useful for lightweight state services because there are created each time they are requested.

      // AddScoped(), means service is created once per request within the scope and it's equivalent to a siingleton 
      // but in the current scope itself. for each http request it uses the same instance not use others.
      // It suitable for AuthRepository
      services.AddScoped<IAuthRepository, AuthRepository>();

      // Add DatingRepository
      services.AddScoped<IDatingRepository, DatingRepository>();
      // END of adding repository

      // Add Authentication
      services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
        .AddJwtBearer(options =>
        {
          options.TokenValidationParameters = new TokenValidationParameters
          {
            ValidateIssuerSigningKey = true,
            // key in SymmetricSecurityKey() is stored in config, 
            // and key is string, need to transfer as byte[]
            IssuerSigningKey = new SymmetricSecurityKey(Encoding.ASCII
                  .GetBytes(Configuration.GetSection("AppSettings:Token").Value)),
            ValidateIssuer = false,
            ValidateAudience = false
          };
      });
      // END of adding authentication

      // user AddScoped,  because we want this to create a new instance of this per requests
      services.AddScoped<LogUserActivity>();
    }

    // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
    public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
    {
      if (env.IsDevelopment())
      {
        // When in Development mode, that would throw exception, 
        // but in Production mode not throw any exception.
        app.UseDeveloperExceptionPage();
      }
      else
      {  // When in Production mode, that would use "gloabal exceptions handler"
        // This exceptionHandler adds middleware to our pipeline that catch exceptions
        app.UseExceptionHandler(builder =>
        {
          // The context in this case is related to our http request and response
          builder.Run(async context =>
          {
            // Set this StatusCode
            context.Response.StatusCode = (int)HttpStatusCode.InternalServerError;

            // Get the details of the error. error stores the perticular error
            var error = context.Features.Get<IExceptionHandlerFeature>();
            if (error != null)
            {
              // Need to extend this response to ADD error on the header of request
              // Use AddApplicationError of Helpers/Extension.cs
              // And then go to Extentions.cs to add information into reponse Header with Error message
              context.Response.AddApplicationError(error.Error.Message);

              // Write the error message into http response 
              await context.Response.WriteAsync(error.Error.Message);

            }
          });
        });
      }

      // app.UseHttpsRedirection();

      app.UseRouting();

      app.UseAuthentication(); // for .NET 3.0
      app.UseAuthorization();

      // Code for CORS .NET 3.0

      app.UseCors(x => x.AllowAnyOrigin().AllowAnyMethod().AllowAnyHeader());
      // Need "AllowCredentials()" for uploading files, this would actually fix our problem with ng2-file-upload.
      // But use  AllowCredentials() means needing to use "cookies" for authenitcaton, but we do not use cookies authentication



      app.UseEndpoints(endpoints =>
      {
        endpoints.MapControllers();

        // Code for 3.0
        //endpoints.MapFallbackToController("Index", "Fallback");

      });
    }
  }
}
