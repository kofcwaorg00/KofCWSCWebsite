using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public class SPGetChairmanInfoBlock
    {
     
        [Display(Name = "Name")]
        public string? FullName { get; set; }
        public string? Phone { get; set; }
        public string? Title { get; set; }
        public string? Photo { get; set; }
        public string? Email { get; set; }
        public string? ChairGraphic { get; set; }
        public string? Data { get; set; }
        public int OfficeID { get; set; }
        public string? WebPageTagLine {  get; set; } 
        public string? SupremeURL { get; set; }
        public int MemberID { get; set; }
        public string Heading { get; set; }
        public int? KofCID { get; set; }
    }
}
