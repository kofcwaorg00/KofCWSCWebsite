using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class CvnImpDelegatesLog
{
    public int Id { get; set; }

    public Guid? Guid { get; set; }

    public DateTime? Rundate { get; set; }

    public string? Type { get; set; }

    public int? MemberId { get; set; }

    public string? Data { get; set; }
}
