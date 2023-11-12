using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;

namespace BookingSystem.Controllers;

public class AuthController : Controller
{
    private readonly ILogger<AuthController> _logger;
    public static readonly string USER_SESSION_STRING = "currentUser";

    private AuthService authService;

    public AuthController(ILogger<AuthController> logger)
    {
        _logger = logger;
        authService = new AuthService();
    }



    [GetCurrentUser()]
    public IActionResult Login(User currentUser)
    {

        if (currentUser != null)
        {
            return RedirectToAction("Index", "Home", null);
        }

        MetaService metaData = new MetaService();
        string UrlString = "Login";
        string errstring = "";
        ViewBag.Meta = metaData.GetMetaData(UrlString, out errstring);


        return View();
    }
    [GetCurrentUser()]
    public IActionResult Register(User currentUser)
    {
        if (currentUser != null)
        {
            return RedirectToAction("Index", "Home", null);
        }

        MetaService metaData = new MetaService();
        string UrlString = "SignUp";
        string errstring = "";
        ViewBag.Meta = metaData.GetMetaData(UrlString, out errstring);


        return View();
    }


    [HttpPost]
    public IActionResult Register(string name, string email, string password, string passwordConfirm)
    {

        if(String.IsNullOrWhiteSpace(name) || String.IsNullOrWhiteSpace(email) || String.IsNullOrWhiteSpace(password) || String.IsNullOrWhiteSpace(passwordConfirm))
        {
            TempData["error"] = "Please fill in all fields.";
            return View();
        }

            if (password != passwordConfirm)
        {
            TempData["error"] = "Password don't match";
            return View();
        }

        int status = authService.Register(name, email, password);

        if(status == -2)
        {
            TempData["error"] = "Email already in use by another user.";
            return View();
        }
        if (status == -1)
        {
            TempData["error"] = "Could not register user. Please try again.";
            return View();
        }

        return RedirectToAction("Success");
    }

    public IActionResult Success()
    {
        return View();
    }

    public void AddUserToSession(string useremail)
    {

        HttpContext.Session.SetString(USER_SESSION_STRING, useremail);

    }

    public void RemoveUserFromSession()
    {

        HttpContext.Session.Clear();

    }



    public string GetUserFromSession() // TODO : Change to User object
    {
        string userEmail = HttpContext.Session.GetString(USER_SESSION_STRING);

        if (String.IsNullOrEmpty(userEmail)) return null;

        return userEmail;

    }

    [HttpPost]
    public IActionResult Login(string email, string password)
    {


        if(String.IsNullOrEmpty(email) || String.IsNullOrEmpty(password))
        {
            TempData["error"] = "Email and password need a value";
            return RedirectToAction("Login");
        }
        int loginStatus = authService.Login(email, password);

        if (loginStatus < 0)
        {
            TempData["error"] = "Invalid email or password";
            return RedirectToAction("Login");

        }
        else if (loginStatus == 0)
        {
            TempData["error"] = "Could not find user";
            return RedirectToAction("Login");
        }

        // Login successful

        // TODO : Add user to session
        AddUserToSession(email);

        return RedirectToAction("Login");

    }

    [HttpPost]
    public IActionResult Logout()
    {
        RemoveUserFromSession();
        return RedirectToAction("Login");
    }

    public IActionResult Privacy()
    {
        return View();
    }

    [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
    public IActionResult Error()
    {
        return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
    }
}

