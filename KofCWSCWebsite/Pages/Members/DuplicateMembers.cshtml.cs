using KofCWSC.API.Models;
using KofCWSCWebsite.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using Org.BouncyCastle.Asn1.Mozilla;

namespace KofCWSCWebsite.Pages.Members
{
    public class DuplicateMembersModel : PageModel
    {
       public List<DuplicateMember> Members { get; set; }
        public ApiHelper _apiHelper { get; set; }    
        public DuplicateMembersModel(ApiHelper apiHelper)
        {
                _apiHelper = apiHelper;
        }
        private readonly ApplicationDbContext _context;
        public async Task OnGetAsync()
        {
         Members = await _apiHelper.GetAsync<List<DuplicateMember>>("DuplicateMembers");

            //Members = await _context.DuplicateMembers
            //    .FromSqlRaw("EXEC GetDuplicateMembers")
            //    .ToListAsync();
        }

    }
}
