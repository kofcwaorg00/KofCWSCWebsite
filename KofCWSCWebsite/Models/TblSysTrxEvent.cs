using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class TblSysTrxEvent
{
    public int Id { get; set; }

    public string? Title { get; set; }

    public string? Description { get; set; }

    public DateTime? Begin { get; set; }

    public DateTime? End { get; set; }

    public bool? isAllDay { get; set; }

    public string? AttachUrl { get; set; }

    public string? AddedBy { get; set; }

    public DateTime? DateAdded { get; set; }
}
