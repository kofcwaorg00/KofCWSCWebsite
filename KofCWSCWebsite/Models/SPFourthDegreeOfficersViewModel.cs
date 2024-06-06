using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    public class SPFourthDegreeOfficersViewModel
    {
        public List<SPGetChairmanInfoBlock>? FourthDegreeOfficers { get; set; }
    }
}
