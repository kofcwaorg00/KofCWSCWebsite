// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Text.Encodings.Web;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.Extensions.Logging;
using KofCWSCWebsite.Services;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Pages.AOI;
using Serilog;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.Extensions.Primitives;


namespace KofCWSCWebsite.Areas.Identity.Pages.Account
{
    public class RegisterModel : PageModel
    {
        private readonly SignInManager<KofCUser> _signInManager;
        private readonly UserManager<KofCUser> _userManager;
        private readonly IUserStore<KofCUser> _userStore;
        private readonly IUserEmailStore<KofCUser> _emailStore;
        private readonly ILogger<RegisterModel> _logger;
        private readonly ISenderEmail _emailSender;
        private readonly RoleManager<IdentityRole> _roleManager;
        private readonly DataSetService _dataSetService;
        public RegisterModel(
            UserManager<KofCUser> userManager,
            IUserStore<KofCUser> userStore,
            SignInManager<KofCUser> signInManager,
            ILogger<RegisterModel> logger,
            ISenderEmail emailSender,
            RoleManager<IdentityRole> roleManager,
            DataSetService dataSetService)
        {
            _userManager = userManager;
            _userStore = userStore;
            _emailStore = GetEmailStore();
            _signInManager = signInManager;
            _logger = logger;
            _emailSender = emailSender;
            _roleManager = roleManager;
            _dataSetService = dataSetService;
        }

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
        public string ReturnUrl { get; set; }
        public string RemoteIPAddr {  get; set; }
        /// <summary>
        ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
        ///     directly from your code. This API may change or be removed in future releases.
        /// </summary>
        public IList<AuthenticationScheme> ExternalLogins { get; set; }

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

            [Remote(action: "VerifyKofCID", controller: "Users")]
            [Required]
            //[StringLength(50, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 1)]
            [Display(Name = "KofC Member Number")]
            public int KofCMemberID { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]

            [Display(Name = "UserID (email)")]
            public string Email { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [StringLength(100, ErrorMessage = "The {0} must be at least {2} and at max {1} characters long.", MinimumLength = 6)]
            [DataType(DataType.Password)]
            [Display(Name = "Password")]
            public string Password { get; set; }

            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [DataType(DataType.Password)]
            [Display(Name = "Confirm password")]
            [Compare("Password", ErrorMessage = "The password and confirmation password do not match.")]
            public string ConfirmPassword { get; set; }

        }


        public async Task OnGetAsync(string returnUrl = null)
        {
            ReturnUrl = returnUrl;
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            RemoteIPAddr = HttpContext.Request.HttpContext.Connection.RemoteIpAddress.ToString();
        }

        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            //****************************************************************************************
            // 09/21/2024 Tim Philomeno
            // Need to add AOL email address validation here
            //////////////if (Input.Email.ToLower().Contains("aol.com"))
            //////////////{
            //////////////    string myer = "We are not able to support AOL email addresses. Please consider registering for a free email address at gmail, yahoo, or Microsoft Outlook (online)";
            //////////////    ModelState.AddModelError(string.Empty, myer);
            //////////////    Log.Error(myer + " - " + Input.Email);
            //////////////    return Page();
            //////////////}
            //----------------------------------------------------------------------------------------
            //****************************************************************************************
            // 09/21/2024 Tim Philomeno
            // need to validate kofcid again here
            var myType = this.GetType();
            var KofCMemberID = Input.KofCMemberID;
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/VerifyKofCID/" + KofCMemberID);
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();

                var result = responseTask.Result;
                var myAns = result.Content.ReadAsStringAsync().Result;

                if (myAns == "false" || myAns.ToLower().Contains("invalid"))
                {
                    string myError = string.Concat("Member Number ",KofCMemberID, " is not found in our database.");
                    string myLogError = string.Concat("Member Number ", KofCMemberID, " is not found in our database.", "Using IP Address => ", GetClientIpAddress(HttpContext));
                    
                    Log.Error(myType + myLogError);
                    ModelState.AddModelError(string.Empty, myError);
                    return Page();
                }
            }
            //----------------------------------------------------------------------------------------
            returnUrl ??= Url.Content("~/");
            ExternalLogins = (await _signInManager.GetExternalAuthenticationSchemesAsync()).ToList();
            if (ModelState.IsValid)
            {
                var user = CreateUser();

                user.FirstName = Input.FirstName;
                user.LastName = Input.LastName;
                user.KofCMemberID = Input.KofCMemberID;

                await _userStore.SetUserNameAsync(user, Input.Email, CancellationToken.None);
                await _emailStore.SetEmailAsync(user, Input.Email, CancellationToken.None);
                try
                {
                    var result = await _userManager.CreateAsync(user, Input.Password);

                    if (result.Succeeded)
                    {
                        _logger.LogInformation("User created a new account with password.");

                        var userId = await _userManager.GetUserIdAsync(user);
                        var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                        //****************************************************************************************
                        // 6/22/2024 Tim Philomeno
                        // Added this to "prinme" the roles and add me to Admin for a new database
                        //****************************************************************************************
                        if (user.Email == "tphilomeno@comcast.net")
                        {
                            await _roleManager.CreateAsync(new IdentityRole("Member"));
                            await _roleManager.CreateAsync(new IdentityRole("Admin"));
                            await _roleManager.CreateAsync(new IdentityRole("StateOfficer"));
                            await _roleManager.CreateAsync(new IdentityRole("StateChairman"));
                            await _roleManager.CreateAsync(new IdentityRole("DataAdmin"));
                            await _userManager.AddToRoleAsync(user, "Admin");
                        }
                        //---------------------------------------------------------------------------------------
                        var rcode = await _userManager.AddToRoleAsync(user, "Member");

                        code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                        var callbackUrl = Url.Page(
                            "/Account/ConfirmEmail",
                            pageHandler: null,
                            values: new { area = "Identity", userId = userId, code = code, returnUrl = returnUrl },
                            protocol: Request.Scheme);

                        await _emailSender.SendEmailAsync(Input.Email, "Confirm your email",
                        $"You have received this email to registered your account as a member of Washington State Council, Knights of Columbus. For any support questions please contact <a href=\"/ContactUs?messageRecipient=Website:%20Technical%20Support\">Technical Support</a>.<br /><br />" +
                        $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br /><br />" +
                        $"NOTE: This confirmation email expires in 20 minutes.<br /><br /><br /><br />" +
                        $"This email was sent to " + Input.Email + " by Washington State Council, Knights of Columbus. � 1995 - " + DateTime.Now.Year + " Washington State Council. All Rights Reserved");



                        if (_userManager.Options.SignIn.RequireConfirmedAccount)
                        {
                            return RedirectToPage("RegisterConfirmation", new { email = Input.Email, returnUrl = returnUrl });
                        }
                        else
                        {
                            await _signInManager.SignInAsync(user, isPersistent: false);
                            return LocalRedirect(returnUrl);
                        }
                    }
                    foreach (var error in result.Errors)
                    {
                        ModelState.AddModelError(string.Empty, error.Description);
                    }
                }
                catch (Exception ex)
                {

                    ModelState.AddModelError(string.Empty, "Only 1 registration account is allowed per Member");
                    Log.Error(Utils.FormatLogEntry(this, ex));
                }
               
            }

            // If we got this far, something failed, redisplay form
            return Page();
        }

        private KofCUser CreateUser()
        {
            try
            {
                return Activator.CreateInstance<KofCUser>();
            }
            catch
            {
                throw new InvalidOperationException($"Can't create an instance of '{nameof(KofCUser)}'. " +
                    $"Ensure that '{nameof(KofCUser)}' is not an abstract class and has a parameterless constructor, or alternatively " +
                    $"override the register page in /Areas/Identity/Pages/Account/Register.cshtml");
            }
        }

        private IUserEmailStore<KofCUser> GetEmailStore()
        {
            if (!_userManager.SupportsUserEmail)
            {
                throw new NotSupportedException("The default UI requires a user store with email support.");
            }
            return (IUserEmailStore<KofCUser>)_userStore;
        }
        private string GetClientIpAddress(HttpContext context)
        {
            string ipAddress = context.Request.Headers["X-Forwarded-For"];

            if (StringValues.IsNullOrEmpty(ipAddress))
            {
                ipAddress = context.Connection.RemoteIpAddress?.ToString();
            }
            else
            {
                // Sometimes the X-Forwarded-For header contains a list of IP addresses
                // The client's IP is the first one
                ipAddress = ipAddress.Split(',').FirstOrDefault();
            }

            return ipAddress;
        }
    }
}
