using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KofCWSCWebsite.Models;

public partial class TblValAssy
{
    [DisplayName("Assembly")]
    public int ANumber { get; set; }
    [DisplayName("Location")]
    public string? ALocation { get; set; }
    [DisplayName("Assembly Name")]
    public string? AName { get; set; }

    public string? AddInfo1 { get; set; }

    public string? AddInfo2 { get; set; }

    public string? AddInfo3 { get; set; }

    public string? WebSiteUrl { get; set; }
    [DisplayName("Master")]
    public string? MasterLoc { get; set; }
    public string? Status { get; set; }
}
