namespace KofCWSCWebsite.Models
{
    public class CvnMileageC
    {
        public int Id { get; set; }
        public int Council { get; set; }
        public string Location { get; set; }
        public int Mileage { get; set; }
        public string? CouncilAddress { get; set; }
        public string? VenueAddress { get; set; }
    }
}
