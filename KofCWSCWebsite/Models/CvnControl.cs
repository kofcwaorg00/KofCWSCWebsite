using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KofCWSCWebsite.Models;

public partial class CvnControl
{
    public int Id { get; set; }

    [DisplayName("Enter String for Location in the form 'State Convention' May nn-nn, 202n City, WA")]
    public string LocationString { get; set; } = null!;
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? ZipCode { get; set; }
}
