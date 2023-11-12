using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.Models;
using BookingSystem.ViewModels;
using Microsoft.AspNetCore.Authorization;

namespace BookingSystem.Controllers;

public class HomeController : Controller
{

    [AuthGuard()]
    public IActionResult Index(User currentUser)
    {
        //TODO: Add error handling

        BookingService bookingService = new BookingService();
        MetaService metaData = new MetaService();
        string UrlString= "Home";
        string errstring = "";
        ViewBag.Meta = metaData.GetMetaData(UrlString, out errstring);
        string errormsg = "";

        List<Service> services = bookingService.getServicesByUser(currentUser.ID, out errormsg);
        List<Booking> bookings = bookingService.getBookingsByUser(currentUser.ID, out errormsg);

        // Get all users
        UserService userService = new UserService();
        List<User> users = userService.GetUsers(out errormsg);

        users.Remove(users.Find(user => user.ID == currentUser.ID));

        // Creating an instance of the ViewModel and populating it
        var viewModel = new HomeViewModel
        {
            Services = services,
            Bookings = bookings,
            Users = users
        };

        return View(viewModel);
    }

    //POST for adding new service
    //return Service/Index
    [HttpPost, AuthGuard()]
    public IActionResult CreateService(User currentUser, Service newService, List<int> users)
    {
        users.Add(currentUser.ID);
        BookingService bookingService = new BookingService();
        bookingService.createService(newService, users);

        return RedirectToAction("Index");
    }


}

