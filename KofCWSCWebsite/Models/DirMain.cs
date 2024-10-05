using FastReport.Web;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class DirMain
{
    public required WebReport WebReport { get; set; }
}