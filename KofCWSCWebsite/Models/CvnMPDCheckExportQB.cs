﻿using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public partial class CvnMPDCheckExportQB
    {
        public string? CheckNo { get; set; }
        [Display(Name ="Bank Account")]
        public string? BankAccount { get; set; }
        public string? Payee {  get; set; }
        public string? Address { get; set; }
        public string? Date { get; set; }
        public decimal? Amount { get; set; }
        public string? Memo { get; set; }
        public string? Category { get; set; }
        public string? Type { get; set; }
        public string? Description { get; set; }
        [Display(Name = "Print Later")]
        public string? PrintLater { get; set; }
    }
}
