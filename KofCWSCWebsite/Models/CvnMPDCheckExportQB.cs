using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public partial class CvnMPDCheckExportQB
    {
        public string? RefNumber { get; set; }
        public string? BankAccount { get; set; }
        public string? Vendor {  get; set; }
        public string? VendorAddress { get; set; }
        public string? VendorCity { get; set; }
        public string? VendorState { get; set; }
        public string? VendorZip { get; set; }
        public DateOnly? TxnDate { get; set; }
        public decimal? Amount { get; set; }
        public string? Memo { get; set; }
        public string? ExpenseAccount { get; set; }
        public decimal? ExpenseAmount { get; set; }
    }
}
