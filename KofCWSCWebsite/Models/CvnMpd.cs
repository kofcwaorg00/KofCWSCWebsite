using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class CvnMpd
{
    public int Id { get; set; }

    public int MemberId { get; set; }

    public int Council { get; set; }

    [DisplayName("Dist")]
    public int District { get; set; }

    public string Group { get; set; } = null!;
    [DisplayName("Type")]
    public string Office { get; set; } = null!;

    public string Payee { get; set; } = null!;

    [DisplayName("Check#")]
    public int? CheckNumber { get; set; }

    [DisplayName("Check Date")]
    public DateOnly? CheckDate { get; set; }

    public bool? Day1D1 { get; set; }

    public bool? Day2D1 { get; set; }

    public bool? Day3D1 { get; set; }
    public bool? Day1D2 { get; set; }

    public bool? Day2D2 { get; set; }

    public bool? Day3D2 { get; set; }

    [DisplayName("Day1")]
    public string? Day1GD1 { get; set; }

    [DisplayName("Day2")]
    public string? Day2GD1 { get; set; }

    [DisplayName("Day3")]
    public string? Day3GD1 { get; set; }

    [DisplayName("Day1")]
    public string? Day1GD2 { get; set; }

    [DisplayName("Day2")]
    public string? Day2GD2 { get; set; }

    [DisplayName("Day3")]
    public string? Day3GD2 { get; set; }

    [DisplayName("Mileage One Way")]
    public int? Miles { get; set; }

    [DisplayName("Check Total")]
    public decimal? CheckTotal { get; set; }

    [DisplayName("Venue Location")]
    public string Location { get; set; } = null!;

    public bool PayMe { get; set; }
    public string? CouncilStatus { get; set; }
    public int GroupID { get; set; }

    [DisplayName("Council Location")]
    public string? CouncilLocation { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? Zip { get; set; }
    public string? Memo { get; set; }
    public string? CheckAccount { get; set; }
    public string? Category { get; set; }
}
