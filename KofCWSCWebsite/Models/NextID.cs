using Microsoft.EntityFrameworkCore;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Models
{
    public  partial class NextID
    {
        [Key]
        public string NextTempID { get; set; }
    }
}
