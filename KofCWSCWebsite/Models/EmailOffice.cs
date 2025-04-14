using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace KofCWSCWebsite.Models;

public partial class EmailOffice
{
    public int Id { get; set; }

    [MaxLength(50)]
    public string Subject { get; set; } = null!;

    public string From { get; set; } = null!;

    public string Body { get; set; } = null!;

    [DisplayName("Date Sent")]
    public DateTime DateSent { get; set; } 

    public bool Fs { get; set; }

    public bool Gk { get; set; }

    public bool Fn { get; set; }

    public bool Fc { get; set; }

    public bool Dd { get; set; }

    public bool All { get; set; }
    [NotMapped]
    public IFormFile? Attachment { get; set; }
}
