using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public class SPGetDDs
    {
        [Display(Name = "District")]
        public int DistrictI { get; set; }

        [Display(Name = "District Page")]
        public string? District { get; set; }
        
        [Display(Name = "Name")]
        public string? FullName { get; set; }
        
        [Display(Name = "Assigned Councils")]
        public string? AssignedCouncils { get; set; }
        
        public string? Email { get; set; }

        public string Heading { get; set; }
        public int MemberID { get; set; }
    }
}
