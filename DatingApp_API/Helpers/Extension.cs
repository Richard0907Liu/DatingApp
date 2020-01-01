using System;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Newtonsoft.Json.Serialization;

namespace DatingApp_API.Helpers
{
  // Uuse "static", means that we don't need to create a new instance of extension 
  // when we want to use one of its methods
  public static class Extension
  {
    // Use for handling error
    // Use "this" can directly get variable in chain function when this method is triggered
    // First param is HTTP reponse, can be added information into Header . Second param is an error message from startup.cs
    public static void AddApplicationError(this HttpResponse response, string message)
    {
      // Want to add additional headers(key, value)
      response.Headers.Add("Application-Error", message);
      response.Headers.Add("Access-Control-Expose-Headers", "Application-Error");
      response.Headers.Add("Access-Control-Allow-Origin", "*");
    }

    // Add information into "response Header" for pagination and paging
    public static void AddPagination(this HttpResponse response, 
      int currentPage, int itemsPerPage, int totalItems, int totalPages)
    {
      // Console.WriteLine("totalItems in Extension::::::::::" + totalItems );  // 0
      var paginationHeader =  new PaginationHeader(currentPage, itemsPerPage, totalItems, totalPages);

      // pagination in headers have to be camel case not title case for angular front end.
      var camelCaseFormatter = new JsonSerializerSettings();
      camelCaseFormatter.ContractResolver = new CamelCasePropertyNamesContractResolver();
      // Add key:value into Headers, value need to be converted an object to a JSON string
      // SerializeObject() has two overloads
      response.Headers.Add("Pagination", 
        JsonConvert.SerializeObject(paginationHeader, camelCaseFormatter)); // pass camel case properties into Headers
      //response.Headers.Add("Pagination", JsonConvert.SerializeObject(paginationHeader)); // not sutiable for angular becase it return title case string
      
      response.Headers.Add("Access-Control-Expose-Headers", "Pagination");
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