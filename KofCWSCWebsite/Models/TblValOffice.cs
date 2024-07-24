using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblValOffice //country master
{
    [Key]
    public int OfficeId { get; set; }
    [Display(Name = "Description")]
    public string? OfficeDescription { get; set; }
    [Display(Name = "Sort Order")]
    public int? DirSortOrder { get; set; }
    [Display(Name = "Alt Description")]
    public string? AltDescription { get; set; }
    [Display(Name = "Email Alias")]
    public string? EmailAlias { get; set; }
    [Display(Name = "Formal Title?")]
    public bool? UseAsFormalTitle { get; set; }
    [Display(Name = "Contact Us String")]
    public string? ContactUsstring { get; set; }
    [Display(Name = "Tag Line")]
    public string? WebPageTagLine { get; set; }
    [Display(Name = "Supreme URL")]
    public string? SupremeUrl { get; set; }
    public int GroupId { get; set; }
}
