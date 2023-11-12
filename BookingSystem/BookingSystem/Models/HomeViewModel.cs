using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class HomeViewModel
    {
        public List<Service> Services { get; set; }
        public List<Booking> Bookings { get; set; }
        public List<User> Users { get; set; }
    }
}
