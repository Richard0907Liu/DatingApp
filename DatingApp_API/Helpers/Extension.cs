using System;
using Microsoft.AspNetCore.Http;

namespace DatingApp_API.Helpers
{
  // Uuse "static", means that we don't need to create a new instance of extension 
  // when we want to use one of its methods
  public static class Extension
  {
    // Use for handling error
    // Use "this" can directly get variable in chain function when this method is triggered
    public static void AddApplicationError(this HttpResponse response, string message)
    {
      // Want to add additional headers(key, value)
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
    }

    public static int CalculateAge(this DateTime theDateTime)  // parameter from AutoMapperProfiles
    {
      var age = DateTime.Today.Year - theDateTime.Year;
      // Need to check there is user's birthdaty.
      if (theDateTime.AddYears(age) > DateTime.Today)
        age--; // take off age attribute from user profile

      return age;

    }

  }
}