using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class TblWebTrxAoi
{
    public int Id { get; set; }

    public string Type { get; set; } = null!;

    public string Title { get; set; }

    public string? GraphicUrl { get; set; }

    public string? Text { get; set; }

    public string? LinkUrl { get; set; }

    public DateTime? PostedDate { get; set; }

    public bool? Expired { get; set; }
}
