using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models;

public partial class CvnMileage
{
    public int Id { get; set; }


    public int Council { get; set; }

    [DisplayName("Venu Location")]
    public string Location { get; set; }

    [DisplayName("Mileage from Council to Venu Location")]
    public int Mileage { get; set; }
}
