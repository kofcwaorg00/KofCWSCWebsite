using com.sun.corba.se.impl.naming.namingutil;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public class HomePageViewModel
    {

        [Display(Name = "Name")]
        public string? FullName { get; set; }
        public string? OfficeDescription { get; set; }
        public string? Photo { get; set; }
        public string? Email { get; set; }
        public string? Data { get; set; }
        public string? TagLine { get; set; }
        public string? Class { get; set; }
        public string? URL { get; set; }
        public int OID { get; set; }
        public string Type { get; set; }
        public string Title { get; set; }
        public string? GraphicURL { get; set; }
        public string? LinkURL { get; set; }
		public DateTime PostedDate { get; set; }
        public bool Expired { get; set; }
    }
}
