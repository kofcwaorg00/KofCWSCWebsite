using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class AspNetUserRole
{
    [Required]
    public string UserId { get; set; } = null!;
    [Required]
    public string RoleId { get; set; } = null!;
}
