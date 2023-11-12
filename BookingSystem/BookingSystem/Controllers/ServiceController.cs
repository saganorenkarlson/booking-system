using BookingSystem.Models;
using Microsoft.AspNetCore.Mvc;
using BookingSystem.ViewModels;
using System.Security.Claims;

namespace BookingSystem.Controllers
{
    public class ServiceController : Controller
    {

        public IActionResult Index()
        {
            
            return View();
        }

        [AuthGuard()]
        public IActionResult ViewService(User currentUser,int id)
        {
            //TODO: Add errorhandling
            BookingService bookingService = new BookingService();

            MetaService metaData = new MetaService();
            string UrlString = "Service";
            string errstring = "";
            ViewBag.Meta = metaData.GetMetaData(UrlString, out errstring);

            string errormsg = "";
            Service service = bookingService.getService(id, out errormsg);

            List<Booking> bookings = bookingService.getBookingsByService(id, out errormsg);
            int hoursBooked = bookingService.getHoursBookedByUser(currentUser.ID, id);

            // Get all users
            UserService userService = new UserService();
            List<User> users = userService.GetUsers(out errormsg);

            // Get users for service 
            List<User> usersForService = bookingService.getUsersByService(id, out errormsg);

            var viewModels = new ServiceViewModel
            {
                Service = service,
                Bookings = bookings,
                HoursBooked = hoursBooked,
                Users = users,
                UsersForService = usersForService,
                UserId = currentUser.ID,
            };
        
            return View(viewModels);
        }


        [HttpPost]
        public IActionResult addPersonToService(Service service, List<int> users)
        {
            BookingService bookingService = new BookingService();
            bookingService.addUsers(service.ID, users);

            return RedirectToAction("ViewService", new { id = service.ID });
        }


        [HttpPost]
        [AuthGuard()]

        public IActionResult CreateBooking(User currentUser, Booking newBooking, int serviceID)
        {
            //TODO: error handling
            BookingService bookingService = new BookingService();
            string errormsg = "";
            List<Booking> existingBookings = bookingService.getBookingsByService(serviceID, out errormsg);
            int hoursBooked = bookingService.getHoursBookedByUser(currentUser.ID, serviceID);
            Service service = bookingService.getService(serviceID, out errormsg);

            bool isOverlap = existingBookings.Any(existingBooking =>
            newBooking.StartDate < existingBooking.EndDate &&
            newBooking.EndDate > existingBooking.StartDate);

            var newBookingHours = (newBooking.EndDate - newBooking.StartDate).TotalHours;

            bool bookingIsValid = hoursBooked + newBookingHours < service.BookingLimit && !isOverlap && newBooking.StartDate < newBooking.EndDate && newBooking.StartDate >= DateTime.Today;

            if (bookingIsValid)
            {
                newBooking.UserID = currentUser.ID;
                bookingService.createBooking(newBooking, serviceID, out errormsg);

                return RedirectToAction("ViewService", new { id = serviceID });
            }
            else
            {
                // Handle error
            }

            return RedirectToAction("ViewService", new { id = serviceID });
        }


        [AuthGuard(),HttpPost]
        public IActionResult LeaveService(User currentUser, int serviceID)
        {
            BookingService bookingService = new BookingService();
            bookingService.leaveService(serviceID, currentUser.ID);

            return RedirectToAction("Index", "Home");
        }


        [AuthGuard()]
        public IActionResult DeleteBooking(User currentUser, int bookingID, int serviceID)
        {
            BookingService bookingService = new BookingService();
            string errormsg = "";
            bookingService.deleteBooking(bookingID, out errormsg);
            return RedirectToAction("ViewService", new { id = serviceID });

        }

        [HttpPost, AuthGuard()]
        public IActionResult UpdateService(Service service)
        {
            BookingService bookingService = new BookingService();
            bookingService.updateService(service);

            return RedirectToAction("ViewService", new { id = service.ID });
        }

    }
}
