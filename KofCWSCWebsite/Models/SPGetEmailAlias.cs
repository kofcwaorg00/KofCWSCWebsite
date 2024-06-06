using Microsoft.EntityFrameworkCore;

namespace KofCWSCWebsite.Models
{
    [Keyless]
    public class SPGetEmailAlias
    {
        public string? Office { get; set; }

        public string? Alias { get; set; }
    }
}
