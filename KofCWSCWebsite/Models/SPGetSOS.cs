using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public class SPGetSOS
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
    }
}
