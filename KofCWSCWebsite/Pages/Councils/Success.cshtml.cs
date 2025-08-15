using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KofCWSCWebsite.Pages.Councils
{
    public class SuccessModel : PageModel
    {
        public string UserName { get; set; }
        public int CouncilNo { get; set; }

        public void OnGet(string userName, int councilNo)
        {
            UserName = userName;
            CouncilNo = councilNo;
        }
    }
}

