using com.sun.xml.@internal.bind.v2.model.core;
using KofCWSCWebsite.Areas.Identity.Data;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using System.Security.Claims;

namespace KofCWSCWebsite.Pages.Councils
{
    public class PreFSDetailsModel : PageModel
    {
        private readonly SignInManager<KofCUser> _signInManager;
        private readonly UserManager<KofCUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        public PreFSDetailsModel(
            UserManager<KofCUser> userManager,
            RoleManager<IdentityRole> roleManager,
            DataSetService dataSetService,
            ApiHelper apiHelper)
        {
            _userManager = userManager;
            _roleManager = roleManager;
            _dataSetService = dataSetService;
            _apiHelper = apiHelper; 
        }
        [BindProperty]
        public PreFSDetailsModel Input { get; set; }

        public int Council { get; set; }
        public KofCUser CurrentUser { get; set; }    
        public async Task OnGetAsync(string returnUrl = null)
        {

            CurrentUser = await _userManager.GetUserAsync(User);
            
            var myMember = _apiHelper.GetAsync<TblMasMember>($"IsKofCMember/{CurrentUser.KofCMemberID}");
            // then get the council for this user
            Council = myMember.Result.Council;
        }
    }
}
