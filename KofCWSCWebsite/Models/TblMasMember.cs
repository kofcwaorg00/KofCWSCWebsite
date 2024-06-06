using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class TblMasMember
{
    public int MemberId { get; set; }

    public string? KofCid { get; set; }

    //public string? Prefix { get; set; }

    //public DateTime? PrefixUpdated { get; set; }

    //public string? PrefixUpdatedBy { get; set; }
    [Display(Name = "First Name")]
    public string? FirstName { get; set; }

    //public DateTime? FirstNameUpdated { get; set; }

    //public string? FirstNameUpdatedBy { get; set; }

    //public string? NickName { get; set; }

    //public DateTime? NickNameUpdated { get; set; }

    //public string? NickNameUpdatedBy { get; set; }

    public string? Mi { get; set; }

    //public DateTime? Miupdated { get; set; }

    //public string? MiupdatedBy { get; set; }
    [Display(Name = "Last Name")]
    public string? LastName { get; set; }

    //public DateTime? LastNameUpdated { get; set; }

    //public string? LastNameUpdatedBy { get; set; }

    //public string? Suffix { get; set; }

    //public DateTime? SuffixUpdated { get; set; }

    //public string? SuffixUpdatedBy { get; set; }

    //public string? AddInfo1 { get; set; }

    //public DateTime? AddInfo1Updated { get; set; }

    //public string? AddInfo1UpdatedBy { get; set; }

    public string? Address { get; set; }

    //public DateTime? AddressUpdated { get; set; }

    //public string? AddressUpdatedBy { get; set; }

    public string? City { get; set; }

    //public DateTime? CityUpdated { get; set; }

    //public string? CityUpdatedBy { get; set; }

    public string? State { get; set; }

    //public DateTime? StateUpdated { get; set; }

    //public string? StateUpdatedBy { get; set; }

    public string? PostalCode { get; set; }

    //public DateTime? PostalCodeUpdated { get; set; }

    //public string? PostalCodeUpdatedBy { get; set; }

    public string? Phone { get; set; }

    //public DateTime? PhoneUpdated { get; set; }

    //public string? PhoneUpdatedBy { get; set; }
    [Display(Name = "Wife")]
    public string? WifesName { get; set; }

    //public DateTime? WifesNameUpdated { get; set; }

    //public string? WifesNameUpdatedBy { get; set; }

    //public string? AddInfo2 { get; set; }

    //public DateTime? AddInfo2Updated { get; set; }

    //public string? AddInfo2UpdatedBy { get; set; }

    //public string? FaxNumber { get; set; }

    //public DateTime? FaxNumberUpdated { get; set; }

    //public string? FaxNumberUpdatedBy { get; set; }

    public int Council { get; set; }

    //public DateTime? CouncilUpdated { get; set; }

    //public string? CouncilUpdatedBy { get; set; }

    public int? Assembly { get; set; }

    //public DateTime? AssemblyUpdated { get; set; }

    //public string? AssemblyUpdatedBy { get; set; }

    public int? Circle { get; set; }

    //public DateTime? CircleUpdated { get; set; }

    //public string? CircleUpdatedBy { get; set; }

    public string? Email { get; set; }

    //public DateTime? EmailUpdated { get; set; }

    //public string? EmailUpdatedBy { get; set; }

    //public bool? Deceased { get; set; }

    //public DateTime? DeceasedUpdated { get; set; }

    //public string? DeceasedUpdatedBy { get; set; }

    //public string? CellPhone { get; set; }

    //public DateTime? CellPhoneUpdated { get; set; }

    //public string? CellPhoneUpdatedBy { get; set; }

    //public DateTime? LastUpdated { get; set; }

    //public bool? SeatedDelegateDay1 { get; set; }

    //public bool? SeatedDelegateDay2 { get; set; }

    //public bool? SeatedDelegateDay3 { get; set; }

    //public bool? PaidMpd { get; set; }

    //public bool? Bulletin { get; set; }

    //public DateTime? BulletinUpdated { get; set; }

    //public string? BulletinUpdatedBy { get; set; }

    //public string? UserId { get; set; }

    //public string? Data { get; set; }

    //public bool? DataChanged { get; set; }

    //public DateTime? LastLoggedIn { get; set; }

    //public bool? CanEditAdmUi { get; set; }

    //public int? DoNotEmail { get; set; }

    //public bool? HidePersonalInfo { get; set; }

    //public string? WhyDoNotEmail { get; set; }
}
