using Microsoft.EntityFrameworkCore;

namespace KofCWSC.API.Models
{
    [Keyless]
    public class SendToEmailAddress
    {
        public string Email { get; set; }
    }
}
