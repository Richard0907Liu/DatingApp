using System.IO;
using Microsoft.AspNetCore.Mvc;

namespace DatingApp_API.Controllers
{
    // For development, deployment
    // Pass wwwroot index.html to webAPI
    // Need to do is give our API some additional configuration so that it knows what to do
    // and can pass off the routing responsibility to angular when we refresh our page when we're on an angular route.
    // unlike our previous API controllers we need view support with this one.
    public class Fallback : Controller
    {
        public IActionResult Index() 
        {
            return PhysicalFile(Path.Combine(Directory.GetCurrentDirectory(), "wwwroot",
                "index.html"), "text/HTML");  // File type "text/HTML"
        }
        // then back to Startup.cs
    }
}