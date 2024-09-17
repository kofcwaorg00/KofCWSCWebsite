using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class EmailOffice
{
    public int Id { get; set; }

    public string Subject { get; set; } = null!;

    public string From { get; set; } = null!;

    public string Body { get; set; } = null!;

    public bool? Fs { get; set; }

    public bool? Gk { get; set; }

    public bool? Fn { get; set; }

    public bool? Fc { get; set; }

    public bool? Dd { get; set; }

    public bool? All { get; set; }
}
