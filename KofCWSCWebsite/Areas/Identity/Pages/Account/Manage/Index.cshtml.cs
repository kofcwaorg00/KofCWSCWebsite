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

namespace KofCWSCWebsite.Areas.Identity.Pages.Account.Manage
{
    public class IndexModel : PageModel
    {
        private readonly UserManager<KofCUser> _userManager;
        private readonly SignInManager<KofCUser> _signInManager;
        private readonly IConfiguration _configuration;

        public IndexModel(
            UserManager<KofCUser> userManager,
            SignInManager<KofCUser> signInManager,
            IConfiguration configuration)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _configuration = configuration; 
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
        }

        private async Task LoadAsync(KofCUser user)
        {
            var userName = await _userManager.GetUserNameAsync(user);
            var phoneNumber = await _userManager.GetPhoneNumberAsync(user);

            Username = userName;

            Input = new InputModel
            {
                PhoneNumber = phoneNumber,
                FirstName = user.FirstName,
                LastName = user.LastName,
                KofCMemberID = user.KofCMemberID              
            };
        }
        public KofCUser? CurrentUser { get; set; }
        public async Task<IActionResult> OnGetAsync()
        {
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

            user.FirstName = Input.FirstName;
            user.LastName = Input.LastName;
            user.KofCMemberID = Input.KofCMemberID;
            await _userManager.UpdateAsync(user);

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

            await _userManager.UpdateAsync(user);

            await _signInManager.RefreshSignInAsync(user);
            StatusMessage = "Your profile has been updated";
            return RedirectToPage();
        }
    }
}
