using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class CvnLocation
{
    public int Id { get; set; }

    public string Location { get; set; }
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
}
