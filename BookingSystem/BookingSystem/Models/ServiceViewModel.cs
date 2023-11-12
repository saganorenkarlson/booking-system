using BookingSystem.Models;

namespace BookingSystem.ViewModels
{
    public class ServiceViewModel
    {
        public Service Service { get; set; }
        public List<Booking> Bookings { get; set; }
        public int HoursBooked { get; set; }
        public List<User> Users { get; set; }
        public List<User> UsersForService { get; set; }
        public int UserId { get; set; }

    }
}
