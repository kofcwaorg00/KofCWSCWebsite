using Microsoft.Identity.Client;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using KofCWSCWebsite.Data;

namespace KofCWSCWebsite.Models;

public partial class TblValCouncilFSEdit
{
    [DisplayName("No")]
    public int CNumber { get; set; }
    public string Status { get; set; } = null!;
    public string? CLocation { get; set; } = null!;
    public string? CName { get; set; }
    public int District { get; set; }
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

    [DisplayName("Address")]
    [NoPoBox]
    public string PhyAddress { get; set; } = "";

    [DisplayName("City")]
    public string PhyCity { get; set; } = "";

    [DisplayName("State")]
    public string PhyState { get; set; } = "";

    [DisplayName("Postal Code")]
    public string PhyPostalCode { get; set; } = "";

    [DisplayName("Address")]
    public string MailAddress { get; set; } = "";

    [DisplayName("City")]
    public string MailCity { get; set; } = "";

    [DisplayName("State")]
    public string MailState { get; set; } = "";

    [DisplayName("Postal Code")]
    public string MailPostalCode { get; set; } = "";

    [DisplayName("Address")]
    public string MeetAddress { get; set; } = "";

    [DisplayName("City")]
    public string MeetCity { get; set; } = "";

    [DisplayName("State")]
    public string MeetState { get; set; } = "";

    [DisplayName("Postal Code")]
    public string MeetPostalCode { get; set; } = "";

    [DisplayName("Day of Week (for example 4th Tue)")]
    public string? BMeetDOW { get; set; }

    [DisplayName("Meeting Time (for example 7:30pm)")]
    public string? BMeetTime { get; set; }

    [DisplayName("Day of Week (for example 4th Tue)")]
    public string? OMeetDOW { get; set; }

    [DisplayName("Meeting Time (for example 7:30pm)")]
    public string? OMeetTime { get; set; }

    [DisplayName("Day of Week (for example 4th Tue)")]
    public string? SMeetDOW { get; set; }

    [DisplayName("Meeting Time (for example 7:30pm)")]
    public string? SMeetTime { get; set; }
    public bool SeatedDelegateDay1D1 { get; set; }
    public bool SeatedDelegateDay2D1 { get; set; }
    public bool SeatedDelegateDay3D1 { get; set; }
    public bool SeatedDelegateDay1D2 { get; set; }
    public bool SeatedDelegateDay2D2 { get; set; }
    public bool SeatedDelegateDay3D2 { get; set; }
    public bool PaidMPD { get; set; }
    public string? FSAddress { get; set; }
    public string? FSCity { get; set; }
    public string? FSState { get; set; }
    public string? FSPostalCode { get; set; }
}
