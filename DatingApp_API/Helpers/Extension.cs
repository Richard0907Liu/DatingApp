using Microsoft.AspNetCore.Http;

namespace DatingApp_API.Helpers
{
  // Uuse "static", means that we don't need to create a new instance of extension 
  // when we want to use one of its methods
  public static class Extension
  {
    // Use for handling error
    public static void AddApplicationError(this HttpResponse response, string message)
    {
      // Want to add additional headers(key, value)
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
    }
  }
}