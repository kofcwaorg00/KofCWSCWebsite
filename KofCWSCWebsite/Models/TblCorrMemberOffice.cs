using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblCorrMemberOffice
{
    [Key]
    public int Id { get; set; }
    
    public int MemberId { get; set; }
    [Display(Name = "Office")]
    public int OfficeId { get; set; }
    
    public bool PrimaryOffice { get; set; }

    public int? Year { get; set; }
    public int? Council { get; set; }
    public int? District { get; set; }
    public int? Assembly { get; set; }
}
