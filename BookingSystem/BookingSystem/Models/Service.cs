namespace BookingSystem.Models
{
    public class Service
    {
        public int ID { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public int BookingLimit { get; set; }
        public Service() { }
        public Service(int id, string name, string description, int bookingLimit)
        {
            ID = id;
            Name = name;
            Description = description;
            BookingLimit = bookingLimit;
        }
    }
}
