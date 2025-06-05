using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    
    public class SPGetSOSView
    {
        public string? Heading { get; set; }
        public string? Photo { get; set; }
        public string? Email { get; set; }
        public string? FullName { get; set; }
        public string? OfficeDescription { get; set; }
        public string? ContactUs { get; set; }
        public string? DirInfo { get; set; }
        public int MemberID { get; set; }
        [Key]
        public int SortBy { get; set; }
        public int KofCID { get; set; }
    }
}
