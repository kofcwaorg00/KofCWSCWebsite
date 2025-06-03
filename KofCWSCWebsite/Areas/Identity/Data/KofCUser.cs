using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace KofCWSCWebsite.Areas.Identity.Data;

// Add profile data for application users by adding properties to the IdentityUser class
public class KofCUser : IdentityUser
{
    [Required]
    [MaxLength(100)]
    public string FirstName { get; set; } = string.Empty;

    [Required]
    [MaxLength(100)]
    public string LastName { get; set; } = string.Empty;

    //[Remote(action: "VerifyKofCID", controller: "Users")]
    [Required]
    [DisplayName("KofC Member Number")]
    public int KofCMemberID { get; set; }

    public string? ProfilePictureUrl { get; set; }
    public string? Address { get; set; }
    public string? City { get; set; }
    public string? State { get; set; }
    public string? PostalCode { get; set; }
    public string? Wife { get; set; }
    public int? Council { get; set; }
    public bool MemberVerfied { get; set; }
}

