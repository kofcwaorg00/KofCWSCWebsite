using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    public partial class SPGetAssysView
    {
        [DisplayName("No")]
        [Key]
        public int AssyNo { get; set; }
        public string? City { get; set; }

        [DisplayName("Assembly Name")]
        public string? AssyName { get; set; }

        [DisplayName("Faithful Navigator")]
        public string? FN { get; set; }
        public int FNID { get; set; }

        [DisplayName("Faithful Comptroller")]
        public string? FC { get; set; }
        public int FCID { get; set; }

        [DisplayName("Faithful Friar")]
        public string? FF { get; set; }
        public int FFID { get; set; }

        public string? MeetingInfo { get; set; }

        [DisplayName("Website")]
        public string? WebsiteURL { get; set; }

        public string? ML { get; set; }
        public string? Heading { get; set; }
    }
}
