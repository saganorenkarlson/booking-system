namespace BookingSystem.Models
{
    public class Booking
    {
        public int ID { get; set; }
        public DateTime StartDate {  get; set; }
        public DateTime EndDate { get; set; }
        public int UserID { get; set; }
        public Service Service {  get; set; }
        public string UserName { get; set; }
        public Booking() { }

        public Booking(int id, DateTime startDate, DateTime endDate, int userid, Service service, string userName)
        {
            ID = id;
            StartDate = startDate;
            EndDate = endDate;
            UserID = userid;
            Service = service;
            UserName = userName;
        }

    }
}
