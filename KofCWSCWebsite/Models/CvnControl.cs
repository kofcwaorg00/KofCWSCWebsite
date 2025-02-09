using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace KofCWSCWebsite.Models;

public partial class CvnControl
{
    public int Id { get; set; }

    [DisplayName("Enter String for Location in the form 'State Convention May nn-nn, 202n City, WA'")]
    public string LocationString { get; set; } = null!;
    public string Address { get; set; }
    public string City { get; set; }
    public string State { get; set; }
    public string ZipCode { get; set; }
    [DisplayName("Dollars Per Day")]
    public decimal MPDDay { get; set; }
    [DisplayName("Cents per Mile")]
    public decimal MPDMile { get; set; }
    [DisplayName("Closest City to the Venue Location, This is used as a link to the mileage data. If you don't see your location then add one.")]
    public string Location { get; set; }
}
