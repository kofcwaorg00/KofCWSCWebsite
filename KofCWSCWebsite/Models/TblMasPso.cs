using System;
using System.Collections.Generic;

namespace KofCWSCWebsite.Models;

public partial class TblMasPso
{
    public int Id { get; set; }

    public int? Year { get; set; }

    public string? StateDeputy { get; set; }

    public string? StateSecretary { get; set; }

    public string? StateTreasurer { get; set; }

    public string? StateAdvocate { get; set; }

    public string? StateWarden { get; set; }
}
