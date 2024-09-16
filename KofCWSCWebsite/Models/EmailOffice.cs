using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    public class EmailOffice
    {
        public int Id { get; set; }
        public string sSubject { get; set; }
        [EmailAddress]
        public string sFrom { get; set; }
        public string sBody { get; set; }
        public bool cFS { get; set; }
        public bool cGK { get; set; }
        public bool cFN { get; set; }
        public bool cFC { get; set; }
        public bool cAll { get; set; }
        public bool cDD { get; set; }
    }
}
