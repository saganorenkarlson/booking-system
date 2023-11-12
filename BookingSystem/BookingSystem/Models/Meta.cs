namespace BookingSystem.Models
{
    public class Meta
    {
        public string PageURL { get; set; }
        public string Keywords { get; set; }
        public string Description { get; set; }
        public DateTime PublicedDate { get; set; }
        public string Locale { get; set; }

        public string Sitename { get; set; }

        public string ContentType { get; set; }



        public Meta() { }

        public Meta(string pageURL ,string keywords, string description,DateTime publicedDate,string locale ,string sitename,string contentType)
        {
            PageURL=pageURL;
            Keywords=keywords;
            Description=description;
            PublicedDate=publicedDate;
            Locale=locale;
            Sitename=sitename;
            ContentType=contentType;
        }
    }
}
