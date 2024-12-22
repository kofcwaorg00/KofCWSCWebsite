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

    public string Office { get; set; } = null!;

    public string Payee { get; set; } = null!;

    [DisplayName("Check#")]
    public int? CheckNumber { get; set; }

    [DisplayName("Check Date")]
    public DateOnly? CheckDate { get; set; }

    public bool? Day1 { get; set; }

    public bool? Day2 { get; set; }

    public bool? Day3 { get; set; }

    public string? Day1G { get; set; }

    public string? Day2G { get; set; }

    public string? Day3G { get; set; }

    [DisplayName("Mileage One Way")]
    public int Miles { get; set; }

    [DisplayName("Check Total")]
    public decimal CheckTotal { get; set; }

    public string Location { get; set; } = null!;
}
