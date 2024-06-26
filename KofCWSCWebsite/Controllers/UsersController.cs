using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    [Route("[controller]")]
    public class UsersController : Controller
    {
        private readonly ApplicationDbContext _context;
        private string? _myBaseAddress;
        private IConfiguration _configuration;

        public UsersController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            _configuration = configuration;
            _myBaseAddress = (string?)_configuration.GetSection("APIURL").GetValue(typeof(string), "LOCAL");
            if (_myBaseAddress.IsNullOrEmpty())
            {
                Log.Fatal("No API URI Initialized");
                throw new Exception("APIURL is not defined");
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


                Uri myURI = new Uri(_myBaseAddress + "/VerifyKofCID/" + KofCMemberID);
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetAsync(myURI);
                    responseTask.Wait();

                    var result = responseTask.Result;
                    var myAns = result.Content.ReadAsStringAsync().Result;

                    if (myAns == "false")
                    {
                        return Json($"Member Number {KofCMemberID} is not found in our database. Please email mailto:webmaster@kofc-wa.org with your member number, full name, email address and council.");
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
