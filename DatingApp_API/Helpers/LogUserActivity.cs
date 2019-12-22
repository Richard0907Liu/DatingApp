using System;
using System.Security.Claims;
using System.Threading.Tasks;
using DatingApp_API.Data;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection; // for GetService


namespace DatingApp_API.Helpers
{
  // Implement "custome Action Filter"
  // Want to use this class, need to add this as a service into ConfigureServices of startup class 
  public class LogUserActivity : IAsyncActionFilter  
  {
      // One method inside this interface
      // First one is if we want to do something when the action is being executed. (Means before a method is active, do this filter)
      // The second option allows us to run some code after the action has been executed (Means when this method is totally proceessd, do this filter)
      // We can run code before or during (context) and after (next) and actions being executed.
    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        // Balow because we are awaiting "next". This is this means we're waiting until "the action has been completed" 
        // and then we're going to use our resultContext to do various things resulting in  
        // eventually updating the user's last active property

        // inside this "resultContext" is going to be a type of action executed context and also give us access
        // to things like the HTTP context fully action as being executed them all gets the user Id
        var resultContext = await next();
        
        // inside the Parse(), that would be a token
        var userId = int.Parse(resultContext.HttpContext.User
            .FindFirst(ClaimTypes.NameIdentifier).Value);

        // Get an instance of repo, because want to user a method in repository
        // RequestServices, this is being provided as a service in our "dependency injection container inside startup class."
        // by using this and then we can say get service and we specify the service (interface or ?class) we want to get.
        // startup has many service, so user ReqeustServices can get certain service inside startup
        var repo = resultContext.HttpContext.RequestServices.GetService<IDatingRepository>();

        // Get user
        var user = await repo.GetUser(userId);
        // Get user's lastActive date
        user.LastActive = DateTime.Now;
        await repo.SaveAll();
    }
  }
}