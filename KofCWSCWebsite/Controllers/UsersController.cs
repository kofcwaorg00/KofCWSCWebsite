using KofCWSCWebsite.Areas.Identity.Data;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
//using Microsoft.AspNet.Identity;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.AspNetCore.Identity;


namespace KofCWSCWebsite.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        private UserManager<KofCUser> _userManager;

        public UsersController(DataSetService dataSetService, ApiHelper apiHelper,UserManager<KofCUser> userManager)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
            _userManager = userManager;
        }

        public async Task<IActionResult> UserRoles(string userId)
        {
            var user = await _userManager.FindByNameAsync(userId);
            if (user == null)
            {
                return NotFound($"user {userId} not found!"); // Handle user not found
            }

            var roles = await _userManager.GetRolesAsync(user);
            return View(roles); // Pass roles to the view
        }


        [Route("VerifyLogin")]
        public IActionResult VerifyLogin()
        {
            //****************************************************************************************************************
            // 6/22/2024 Tim Philomeno
            // The Remote Action jquery sends the url here for validation but it comes in the form of
            // https://localhost:7213/Users/VerifyKofCID?Input.KofCMemberID=1111111 and we can use the default
            // action of parameter matching because of the dot notation of the query parameter so we have to parse
            // the memberid out of the query string manually. Argggggghhhhh!!!
            //****************************************************************************************************************

            try
            {
                var myURL = Request.GetDisplayUrl();
                var KofCMemberID = myURL.Substring(myURL.IndexOf("=") + 1, myURL.Length - myURL.IndexOf("=") - 1);


                Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/VerifyLogin/" + KofCMemberID);
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetAsync(myURI);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    var myAns = result.Content.ReadAsStringAsync().Result;

                    if (myAns == "false" || myAns.ToLower().Contains("invalid"))
                    {
                        // this person has been suspended so just return a generic message
                        return Json($"Login Failed.");
                    }
                    else
                    {

                        return Json(true);
                    }
                }
            }

            catch (Exception ex)
            {

                Log.Fatal("VerifyKofCIDAsync" + ex.Message + " " + ex.InnerException);
                return Json(false);
            }
        }

        // the post i got this from had the AcceptVerbs but it doesn't look like it is necessary
        //[AcceptVerbs("GET", "POST")]
        //[HttpGet]
        [Route("VerifyKofCID")]
        public async Task<IActionResult> VerifyKofCID()
        {
            //****************************************************************************************************************
            // 6/22/2024 Tim Philomeno
            // The Remote Action jquery sends the url here for validation but it comes in the form of
            // https://localhost:7213/Users/VerifyKofCID?Input.KofCMemberID=1111111 and we can use the default
            // action of parameter matching because of the dot notation of the query parameter so we have to parse
            // the memberid out of the query string manually. Argggggghhhhh!!!
            //****************************************************************************************************************

            try
            {
                var myURL = Request.GetDisplayUrl();
                var KofCMemberID = myURL.Substring(myURL.IndexOf("=") + 1, myURL.Length - myURL.IndexOf("=") - 1);

                //var result = await _context.Database
                //    .SqlQuery<KofCMemberIDUsers>($"EXECUTE uspSYS_ValidateKofCID {KofCMemberID} ")
                //    //.Where(k => k.KofCID == KofCMemberID)
                //    .ToListAsync();

                Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/VerifyKofCID/" + KofCMemberID);
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetAsync(myURI);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    var myAns = result.Content.ReadAsStringAsync().Result;

                    if (myAns == "false" || myAns.ToLower().Contains("invalid"))
                    {
                        return Json($"Member Number {KofCMemberID} is not found in our database. Please email webmaster@kofc-wa.org with your member number, full name, email address and council.");
                    }
                    else if(myAns.ToLower().Contains("sus"))
                    {
                        // the SUS indicates that the member is suspended
                        return Json($"Member is Invalid.");
                    }
                    else
                    {
                        return Json(true);
                    }
                }
            }

            catch (Exception ex)
            {

                Log.Fatal("VerifyKofCIDAsync" + ex.Message + " " + ex.InnerException);
                return Json(false);
            }

        }
    }
}
