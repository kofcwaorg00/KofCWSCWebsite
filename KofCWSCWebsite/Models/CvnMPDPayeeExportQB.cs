using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public partial class CvnMPDPayeeExportQB
    {
        public string? Name { get; set; }
        public string? Company { get; set; }
        public string? Email {  get; set; }
        public string? Phone { get; set; }
        public string? Mobile { get; set; }
        public string? Fax { get; set; }
        public string? Website { get; set; }
        public string? Street { get; set; }
        public string? City { get; set; }
        public string? State { get; set; }
        public string? Zip { get; set; }
        public string? Country { get; set; }
        public decimal? OpeningBalance { get; set; }
        public DateOnly? Date {  get; set; }
        public string? TaxID { get; set; }
    }
}
