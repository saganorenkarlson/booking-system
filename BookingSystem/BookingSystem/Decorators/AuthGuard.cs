using BookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.DependencyInjection;

public class AuthGuard : ActionFilterAttribute
{
    private readonly string sessionKey = "currentUser";

    public User currentUser;
    public UserService userService;
    public AuthGuard()
    {
        userService = new UserService();
    }

    public override void OnActionExecuting(ActionExecutingContext context)
    {
        var session = context.HttpContext.Session;

        string userEmail = session.GetString(sessionKey);
        if (String.IsNullOrEmpty(userEmail)) // Check the session variable value
        {
            // Perform your action here
            // For example, you can redirect to another action or return a specific result.
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
        }

        currentUser = userService.GetUserByEmail(userEmail);

        if(currentUser == null)
        {
            context.Result = new RedirectToActionResult("Login", "Auth", null);
            return;
    
        }
        context.ActionArguments["currentUser"] = currentUser;


        base.OnActionExecuting(context);
    }
}
