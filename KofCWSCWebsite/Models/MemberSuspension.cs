using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class MemberSuspension
{
    public int Id { get; set; }

    public int KofCid { get; set; }

    public string? Comment { get; set; }
    public DateTime Updated { get; set; }
    public int UpdatedBy { get; set; }
}
