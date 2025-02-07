using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblMasAward
{
    public int Id { get; set; }

    [DisplayName("Award")]
    public string? AwardName { get; set; }

    [MaxLength(1000)]
    [DisplayName("Description (max 1,000 chars)")]
    public string? AwardDescription { get; set; }
    [DisplayName("Due Date")]
    [DisplayFormat(DataFormatString = "{0:MM/dd/yyyy}")]
    public DateTime? AwardDueDate { get; set; }
    [DisplayName("Link to Form")]
    public string? LinkToTheAwardForm { get; set; }
    [DisplayName("Submit To")]
    public string? AwardSubmissionEmailAddress { get; set; }
}
