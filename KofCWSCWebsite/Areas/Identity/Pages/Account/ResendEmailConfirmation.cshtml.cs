// Licensed to the .NET Foundation under one or more agreements.
// The .NET Foundation licenses this file to you under the MIT license.
#nullable disable

using System;
using System.ComponentModel.DataAnnotations;
using System.Text;
using System.Text.Encodings.Web;
using System.Threading.Tasks;
using KofCWSCWebsite.Areas.Identity.Data;
using KofCWSCWebsite.Services;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.WebUtilities;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;

namespace KofCWSCWebsite.Areas.Identity.Pages.Account
{
    [AllowAnonymous]
    public class ResendEmailConfirmationModel : PageModel
    {
        private readonly UserManager<KofCUser> _userManager;
        private readonly ISenderEmail _emailSender;

        public ResendEmailConfirmationModel(UserManager<KofCUser> userManager, ISenderEmail emailSender)
        {
            _userManager = userManager;
            _emailSender = emailSender;
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
        public class InputModel
        {
            /// <summary>
            ///     This API supports the ASP.NET Core Identity default UI infrastructure and is not intended to be used
            ///     directly from your code. This API may change or be removed in future releases.
            /// </summary>
            [Required]
            [EmailAddress]
            public string Email { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            var user = await _userManager.FindByEmailAsync(Input.Email);
            if (user == null)
            {
                ModelState.AddModelError(string.Empty, "Email Not Found. Please try again.");
                return Page();
            }

            try
            {
                var userId = await _userManager.GetUserIdAsync(user);
                var code = await _userManager.GenerateEmailConfirmationTokenAsync(user);
                code = WebEncoders.Base64UrlEncode(Encoding.UTF8.GetBytes(code));
                var host = Request.Host.Value;
                var imgUrl = "<img src=" + host + "/images/LOGO-WA1.jpg style='text-align:left' />";
                var callbackUrl = Url.Page(
                    "/Account/ConfirmEmail",
                    pageHandler: null,
                    values: new { userId = userId, code = code },
                    protocol: Request.Scheme);
                await _emailSender.SendEmailAsync(
                    Input.Email,
                    "Confirm your email",
                    $"You have received this email as a registered member of Washington State Council, Knights of Columbus. For any support questions please email support@kofc-wa.org.<br /><br />" +
                    $"Please confirm your account by <a href='{HtmlEncoder.Default.Encode(callbackUrl)}'>clicking here</a>.<br /><br />" +
                    $"NOTE: This confirmation email expires in 20 minutes.<br /><br /><br /><br />" +
                    $"This email was sent to " + Input.Email + " by Washington State Council, Knights of Columbus.©1995-" + DateTime.Now.Year + " Washington State Council. All Rights Reserved"
                    );
                //$"Please confirm your account by clicking here {HtmlEncoder.Default.Encode(callbackUrl)}");

                ModelState.AddModelError(string.Empty, "Confirmation email sent. Please check your email.");
                return Page();

            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                ModelState.AddModelError(string.Empty, "Confirmation email was not sent. Please email webmaster@kofc-wa.org to report this.");
                return Page();
            }

        }
    }
}
