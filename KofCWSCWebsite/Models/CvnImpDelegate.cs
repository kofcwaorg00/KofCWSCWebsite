using Microsoft.Identity.Client;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class CvnImpDelegate
{
    [Display(Name ="Submitter Date")]
    public DateTime? SubmissionDate { get; set; }

    [Display(Name = "Submitter Email")]
    public string? FormSubmitterSEmail { get; set; }
    [Display(Name = "Council Name")]
    public string? CouncilName { get; set; }

    [Display(Name = "Council #")]
    public int? CouncilNumber { get; set; }

    public string? D1FirstName { get; set; }
    public string? D1MiddleName { get; set; }
    public string? D1LastName { get; set; }
    public string? D1Suffix { get; set; }
    public int? D1MemberID { get; set; }
    public string? D1Address1 { get; set; }

    public string? D1Address2 { get; set; }
    public string? D1City { get; set; }
    public string? D1State { get; set; }
    public string? D1ZipCode { get; set; }
    public string? D1Phone { get; set; }

    public string? D1Email { get; set; }

    public string? D2FirstName { get; set; }

    public string? D2MiddleName { get; set; }

    public string? D2LastName { get; set; }

    public string? D2Suffix { get; set; }

    public int? D2MemberID { get; set; }

    public string? D2Address1 { get; set; }

    public string? D2Address2 { get; set; }

    public string? D2City { get; set; }

    public string? D2State { get; set; }

    public string? D2ZipCode { get; set; }

    public string? D2Phone { get; set; }

    public string? D2Email { get; set; }

    public string? A1FirstName { get; set; }

    public string? A1MiddleName { get; set; }

    public string? A1LastName { get; set; }

    public string? A1Suffix { get; set; }

    public int? A1MemberID { get; set; }

    public string? A1Address1 { get; set; }

    public string? A1Address2 { get; set; }

    public string? A1City { get; set; }

    public string? A1State { get; set; }

    public string? A1ZipCode { get; set; }

    public string? A1Phone { get; set; }

    public string? A1Email { get; set; }

    public string? A2FirstName { get; set; }

    public string? A2MiddleName { get; set; }

    public string? A2LastName { get; set; }

    public string? A2Suffix { get; set; }

    public int? A2MemberID { get; set; }

    public string? A2Address1 { get; set; }

    public string? A2Address2 { get; set; }

    public string? A2City { get; set; }

    public string? A2State { get; set; }

    public string? A2ZipCode { get; set; }

    public string? A2Phone { get; set; }

    public string? A2Email { get; set; }

    [Key]
    public int Id { get; set; }
    public string? Validation {  get; set; }
    public string? RecType { get; set; }
}
