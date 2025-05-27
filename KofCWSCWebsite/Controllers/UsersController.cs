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
        private readonly IConfiguration _configuration;

        public UsersController(DataSetService dataSetService, ApiHelper apiHelper,UserManager<KofCUser> userManager,IConfiguration configuration)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
            _userManager = userManager;
            _configuration = configuration;
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

        [Route("EditPhoto/{id}")]
        public async Task<IActionResult> EditPhoto(int id)
        {
            var user = await _userManager.Users
                .Where(u => u.KofCMemberID == id)
                .FirstOrDefaultAsync();
            TempData["HasUser"] = user == null? ViewBag.HasUser = false:ViewBag.HasUser = true;
            TempData["PicUser"] = id;
            
            return View(user);
        }

        [Route("UploadProfilePicture/{id}")]
        public async Task<IActionResult> UploadProfilePicture(IFormFile file,int id)
        {
            var user = await _userManager.Users
                .Where(u => u.KofCMemberID == id)
                .FirstOrDefaultAsync();

            if (file != null && file.Length > 0)
            {
                // Check size (max 2MB)
                if (file.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Input.ProfilePicture", "File too large. Max 2MB.");
                    TempData["HasUser"] = true;
                    return View("EditPhoto",user);
                }

                // Check content type
                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Only JPG and PNG files are allowed.");
                    TempData["HasUser"] = true;
                    return View("EditPhoto", user);
                }

                // Check extension
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext is not ".jpg" and not ".jpeg" and not ".png")
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Invalid file extension.");
                    TempData["HasUser"] = true;
                    return View("EditPhoto", user);
                }

                using var memoryStream = new MemoryStream();
                await file.CopyToAsync(memoryStream);
                memoryStream.Position = 0;

                try
                {
                    using var image = System.Drawing.Image.FromStream(memoryStream);

                    int width = image.Width;
                    int height = image.Height;
                    float dpiX = image.HorizontalResolution;
                    float dpiY = image.VerticalResolution;

                    if (width < 200 || height < 200)
                    {
                        ModelState.AddModelError("Input.ProfilePicture", "Image must be at least 200x200 pixels.");
                        TempData["HasUser"] = true;
                        return View("EditPhoto", user);
                    }

                    if (dpiX < 72 || dpiY < 72)
                    {
                        ModelState.AddModelError("Input.ProfilePicture", "Image resolution must be at least 72 DPI.");
                        TempData["HasUser"] = true;
                        return View("EditPhoto", user);
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Invalid image file.");
                    TempData["HasUser"] = true;
                    return View("EditPhoto", user);
                }

                // Reset memory stream for upload
                memoryStream.Position = 0;

                // Upload to Azure Blob Storage (adjust for your Azure config)
                var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(_configuration["AzureBlobStorage:ConnectionString"]);
                var containerClient = blobServiceClient.GetBlobContainerClient(_configuration["AzureBlobStorage:ContainerName"]);
                await containerClient.CreateIfNotExistsAsync();
                await containerClient.SetAccessPolicyAsync(Azure.Storage.Blobs.Models.PublicAccessType.Blob);

                var uniqueFileName = $"{Guid.NewGuid()}{ext}";
                var blobClient = containerClient.GetBlobClient(uniqueFileName);
                await blobClient.UploadAsync(memoryStream, overwrite: true);

                // Optionally delete old image
                if (!string.IsNullOrEmpty(user.ProfilePictureUrl))
                {
                    try
                    {
                        var oldBlobName = Path.GetFileName(new Uri(user.ProfilePictureUrl).LocalPath);
                        var oldBlobClient = containerClient.GetBlobClient(oldBlobName);
                        await oldBlobClient.DeleteIfExistsAsync();
                    }
                    catch { /* ignore */ }
                }

                // Update user record
                user.ProfilePictureUrl = blobClient.Uri.ToString();
                await _userManager.UpdateAsync(user);
            }
            return RedirectToAction("Index", "TblMasMembers", new { lastname = user.LastName});
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
