using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public partial class CvnDelegateDays
    {
        public int Council {  get; set; }
        public int MemberId { get; set; }
        public string Type { get; set; }
        public int District { get; set; }
        [DisplayName("Member Name")]
        public string FullName { get; set; }
        public string Delegate {  get; set; }
        public string? Day1 { get; set; }
        public string? Day2 { get; set; }
        public string? Day3 { get; set; }

    }
}
