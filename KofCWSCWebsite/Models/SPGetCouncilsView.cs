using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class SPGetCouncilsView
{
    
    [DisplayName("No")]
    [Key]
    public int CouncilNo { get; set; }
    [DisplayName("Dis")]
    public int? District { get; set; }

    public string? City { get; set; }

    [DisplayName("Council Name")]
    public string? CouncilName { get; set; }

    [DisplayName("Grand Knight")]
    public string GrandKnight { get; set; }
    public int GKMemberID { get; set; }

    [DisplayName("Financial Secretary")]
    public string FinancialSecretary { get; set; }
    public int FSMemberID { get; set; }
    public string Chaplain { get; set; }
    public int ChapMemberID { get; set; }

    [DisplayName("Meeting Info")]
    public string? MeetingInfo { get; set; }

    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}", ApplyFormatInEditMode = true)]
    public DateTime? Chartered { get; set; }

    [DisplayName("WebSite")]
    public string? WebSiteUrl { get; set; }

    public string? Diocese { get; set; }
    [DisplayName("Bulletin")]
    public string? BulletinUrl { get; set; }
    public string? Heading { get; set; }
  
}
