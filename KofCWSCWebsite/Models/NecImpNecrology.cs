using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class NecImpNecrology
{
    public DateTime? SubDate { get; set; }

    public string? SubFname { get; set; }

    public string? SubLname { get; set; }

    public string? SubEmail { get; set; }

    public string? SubRole { get; set; }

    public int? CouncilId { get; set; }

    public string? DecPrefix { get; set; }

    public string? DecFname { get; set; }

    public string? DecMname { get; set; }

    public string? DecLname { get; set; }

    public string? DecSuffix { get; set; }

    public string? DecFmorKn { get; set; }

    public string? Fmprefix { get; set; }

    public string? Fmfname { get; set; }

    public string? Fmmname { get; set; }

    public string? Fmlname { get; set; }

    public string? Fmsuffix { get; set; }

    public string? Relationship { get; set; }

    public DateTime? Dod { get; set; }

    public int? DecMemberId { get; set; }

    public string? MemberType { get; set; }

    public string? DecOfficesHeld { get; set; }

    public int? AssemblyId { get; set; }

    public string? Nokprefix { get; set; }

    public string? Nokfname { get; set; }

    public string? Nokmname { get; set; }

    public string? Noklname { get; set; }

    public string? Noksuffix { get; set; }

    public string? Nokrelate { get; set; }

    public string? Nokaddress1 { get; set; }

    public string? Nokaddress2 { get; set; }

    public string? Nokcity { get; set; }

    public string? Nokstate { get; set; }

    public string? Nokzip { get; set; }

    public string? Nokcountry { get; set; }

    public string? Comments { get; set; }
    [Key]
    public int Id { get; set; }
    public long SubID { get; set; }
}
