using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblValOffice //country master
{
    [Key]
    public int OfficeId { get; set; }

    public string? OfficeDescription { get; set; }

    public int? DirSortOrder { get; set; }

    public string? AltDescription { get; set; }

    public string? EmailAlias { get; set; }

    public bool? UseAsFormalTitle { get; set; }

    public string? ContactUsstring { get; set; }

    public string? WebPageTagLine { get; set; }

    public string? SupremeUrl { get; set; }
    public int GroupId { get; set; }
}
