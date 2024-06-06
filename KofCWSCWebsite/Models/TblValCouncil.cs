using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblValCouncil
{
    [DisplayName("No")]
    public int CNumber { get; set; }
    [DisplayName("City")]
    public string? CLocation { get; set; }
    [DisplayName("Council Name")]
    public string? CName { get; set; }
    [DisplayName("Dist")]
    public int? District { get; set; }

    public string? AddInfo1 { get; set; }

    public string? AddInfo2 { get; set; }

    public string? AddInfo3 { get; set; }

    public bool? LiabIns { get; set; }

    [DisplayName("Diocese")]
    public string? DioceseId { get; set; }
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Chartered { get; set; }

    public string? WebSiteUrl { get; set; }

    public string? BulletinUrl { get; set; }

    public decimal? Arbalance { get; set; }

    public string Status { get; set; } = null!;
}
