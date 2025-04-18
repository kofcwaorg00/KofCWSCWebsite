using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class LogCorrMemberOffice
{
    public int Id { get; set; }

    public string ChangeType { get; set; } = null!;

    public DateTime ChangeDate { get; set; }

    public int MemberId { get; set; }

    public int OfficeId { get; set; }

    public bool PrimaryOffice { get; set; }

    public int? Year { get; set; }

    public int? District { get; set; }

    public int? Council { get; set; }

    public int? Assembly { get; set; }

    public bool Processed { get; set; }
}
