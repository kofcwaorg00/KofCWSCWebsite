using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace KofCWSCWebsite.Models
{
        public class RoleEdit
    {
        public IdentityRole Role { get; set; }
        public IEnumerable<KofCUser> Members { get; set; }
        public IEnumerable<KofCUser> NonMembers { get; set; }
    }
}

