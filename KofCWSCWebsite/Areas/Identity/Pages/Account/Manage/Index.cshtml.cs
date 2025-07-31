// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System.ComponentModel.DataAnnotations;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Drawing;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Microsoft.AspNetCore.Mvc.Rendering;

namespace KofCWSCWebsite.Areas.Identity.Pages.Account.Manage
{
    
    public class IndexModel : PageModel
    {
        private readonly UserManager<KofCUser> _userManager;
        private readonly SignInManager<KofCUser> _signInManager;
        private readonly IConfiguration _configuration;
        private readonly ApiHelper _apiHelper;
        public List<Microsoft.AspNetCore.Mvc.Rendering.SelectListItem> Councils { get; set; }
        public IndexModel(
            UserManager<KofCUser> userManager,
            SignInManager<KofCUser> signInManager,
            IConfiguration configuration,
            ApiHelper apiHelper)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration;
            _apiHelper = apiHelper;
        }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public string Username { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [TempData]
        public string StatusMessage { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        [BindProperty]
        public InputModel Input { get; set; }

        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public class InputModel
        {
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "First name")]
            public string FirstName { get; set; }

            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "Last name")]
            public string LastName { get; set; }

            [Required]
            //[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "KofC Member Number")]
            public int KofCMemberID { get; set; }
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Phone]
            [Display(Name = "Phone number")]
            public string PhoneNumber { get; set; }

            [Display(Name = "Profile picture")]
            public IFormFile? ProfilePicture { get; set; }

            public string? CurrentPicturePath { get; set; }
            public string Address { get; set; }
            public string City { get; set; }
            public string State { get; set; }
            public string PostalCode { get; set; }
            public string Wife { get; set; }
            public int Council { get; set; }
        }

        private async Task LoadAsync(KofCUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;
            var member = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{user.KofCMemberID}");

            Input = new InputModel
            {
                //**************************************************************************************
                // 7/30/2025 Tim Philomeno
                // currently there is no way to validate the phone number as we don't have an
                // sms server setup
                //PhoneNumber = phoneNumber,
                PhoneNumber = member?.Phone ?? phoneNumber,
                //-------------------------------------------------------------------------------------
                FirstName = user.FirstName,
                LastName = user.LastName,
                KofCMemberID = user.KofCMemberID,

                Address = member?.Address ?? user.Address,
                City = member?.City ?? user.City,
                State = member?.State ?? user.State,
                PostalCode = member?.PostalCode ?? user.PostalCode,
                Wife = member?.WifesName ?? user.Wife,
                Council = (int)(member?.Council ?? user.Council)
            };
        }
        public KofCUser? CurrentUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
            await LoadCouncilsAsync();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }
            CurrentUser = user as KofCUser;
            await LoadAsync(user);
            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            await LoadCouncilsAsync();
            var user = await _userManager.GetUserAsync(User);
            if (user == null)
            {
                return NotFound($"Unable to load user with ID '{_userManager.GetUserId(User)}'.");
            }

            if (!ModelState.IsValid)
            {
                await LoadAsync(user);
                return Page();
            }
            // mymember will contain the current member information that we will modify and save
            var mymember = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{user.KofCMemberID}");

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.KofCMemberID = Input.KofCMemberID;
            user.PhoneNumber = Input.PhoneNumber;
            user.Address = Input.Address;
            user.City = Input.City;
            user.State = Input.State;
            user.PostalCode = Input.PostalCode;
            user.Wife = Input.Wife;
            user.Council = Input.Council;

            mymember.LastUpdated = DateTime.Now;
            mymember.LastUpdatedBy = await Utils.GetUserProp<int>(User, _userManager, "KofCMemberID");
            mymember.Phone = Input.PhoneNumber;
            mymember.Address = Input.Address;
            mymember.City = Input.City;
            mymember.State = Input.State;
            mymember.PostalCode = Input.PostalCode;
            mymember.WifesName = Input.Wife;
            mymember.Council = Input.Council;

            
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);
            if (Input.PhoneNumber != phoneNumber)
            {
                var setPhoneResult = await _userManager.SetPhoneNumberAsync(user, Input.PhoneNumber);
                if (!setPhoneResult.Succeeded)
                {
                    StatusMessage = "Unexpected error when trying to set phone number.";
                    return RedirectToPage();
                }
            }
            var file = Input.ProfilePicture;

            if (file != null && file.Length > 0)
            {
                // Check size (max 2MB)
                if (file.Length > 2 * 1024 * 1024)
                {
                    ModelState.AddModelError("Input.ProfilePicture", "File too large. Max 2MB.");
                    return Page();
                }

                // Check content type
                var allowedTypes = new[] { "image/jpeg", "image/png" };
                if (!allowedTypes.Contains(file.ContentType.ToLower()))
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Only JPG and PNG files are allowed.");
                    return Page();
                }

                // Check extension
                var ext = Path.GetExtension(file.FileName).ToLowerInvariant();
                if (ext is not ".jpg" and not ".jpeg" and not ".png")
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Invalid file extension.");
                    return Page();
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
                        return Page();
                    }

                    if (dpiX < 72 || dpiY < 72)
                    {
                        ModelState.AddModelError("Input.ProfilePicture", "Image resolution must be at least 72 DPI.");
                        return Page();
                    }
                }
                catch (Exception)
                {
                    ModelState.AddModelError("Input.ProfilePicture", "Invalid image file.");
                    return Page();
                }

                // Reset memory stream for upload
                memoryStream.Position = 0;

                // Upload to Azure Blob Storage (adjust for your Azure config)
                KeyVaultHelper kvh = new KeyVaultHelper(_configuration);
                var blobServiceClient = new Azure.Storage.Blobs.BlobServiceClient(kvh.GetSecret("AZBSPCS"));

                var containerClient = blobServiceClient.GetBlobContainerClient(_configuration["AzureBlobStorage:MCContainerName"]);
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
               // await _userManager.UpdateAsync(user);
            }
            try
            {
                //*************************************************************
                // 7/31/2025 Tim Philomeno
                // Wait till we are all done then update the database
                // update DotNetUsers
                await _userManager.UpdateAsync(user);
                // update tbl_MasMembers
                await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"Member/{mymember.MemberId}", mymember);
                // we should probably do a try/catch just incase????

                await _signInManager.RefreshSignInAsync(user);
                StatusMessage = "Your profile has been updated";
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                StatusMessage = ex.Message;
                return RedirectToPage();
            }

        }
        private async Task LoadCouncilsAsync()
        {
            var iCouncils = await _apiHelper.GetAsync<List<TblValCouncil>>("Councils");
            Councils = iCouncils.Select(c => new SelectListItem
            {
                Value = c.CNumber.ToString(),
                Text = $"{c.CNumber} - {c.CName} (District #{c.District})"
           is var fullText && fullText.Length > 50
           ? fullText.Substring(0, 50) + "..."
           : fullText
            }).ToList();
            //var iCouncils = await _apiHelper.GetAsync<List<TblValCouncil>>("Councils");
            //Councils = iCouncils.Select(c => new SelectListItem
            //{
            //    Value = c.CNumber.ToString(),
            //    Text = $"{c.CNumber} - {c.CName} (District #{c.District})"
            //}).ToList();
        }
    }
}
