using Azure.Data.AppConfiguration;
using Humanizer;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.IdentityModel.Tokens;
using System.ComponentModel.DataAnnotations;

namespace KofCWSCWebsite.Pages.Councils
{
    public class ContactCouncilModel : PageModel
    {
        public readonly ApiHelper _apiHelper;
        public ContactCouncilModel(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "From (Your Email)")]
        [Required(ErrorMessage = "Email is required.")]
        [EmailAddress(ErrorMessage = "Please enter a valid email address.")]
        public string Email { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Required]
        [EmailAddress]
        [Display(Name = "Confirm Email")]
        [Compare("Email", ErrorMessage = "Email and confirmation must match.")]
        public string ConfirmEmail { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        public string To { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "Your Name")]
        public string VName { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "Your Phone Number")]
        [Required(ErrorMessage = "Phone number is required.")]
        [RegularExpression(@"^(\+?\d{1,2}\s?)?(\(?\d{3}\)?[\s.-]?)?\d{3}[\s.-]?\d{4}$", ErrorMessage = "Enter a valid phone number.")]
        [Phone(ErrorMessage = "Please enter a valid phone number.")]
        public string VPhone { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "Council Number")]
        [Required(ErrorMessage = "Please select a council.")]
        public int CouncilNo { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "Subject")]
        [Required(ErrorMessage = "Email Subject is required.")]
        public string EmailSubject { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        [Display(Name = "Message")]
        [Required(ErrorMessage = "Email Message is required.")]
        public string EmailMessage { get; set; }
        //-----------------------------------------------------------------------------
        [BindProperty]
        public bool SendCopy { get; set; }
        //-----------------------------------------------------------------------------
        public List<SelectListItem> CouncilOptions { get; set; }

        public List<SelectListItem> RecipientRoles { get; set; }

        public async Task OnGetAsync(string? to,int? CouncilNo)
        {
            await LoadCouncilsAsync();

            RecipientRoles = new List<SelectListItem>
        {
            new SelectListItem { Value = "gk", Text = "Grand Knight" },
            new SelectListItem { Value = "fs", Text = "Financial Secretary" }
        };

            // Example council list — replace with DB or service call if needed
            //CouncilOptions = new List<SelectListItem>
            //{
            //    new SelectListItem { Value = "1234", Text = "Council 1234" },
            //    new SelectListItem { Value = "5678", Text = "Council 5678" },
            //    new SelectListItem { Value = "9012", Text = "Council 9012" }
            //};
            if (CouncilNo.HasValue)
            {
                this.CouncilNo = CouncilNo.Value;
            }
            if (to.IsNullOrEmpty())
            {
                this.To = "gk";
            }
            else
            {
                this.To = to;
            }

        }
        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                await LoadCouncilsAsync(); // repopulate dropdowns
                RecipientRoles = new List<SelectListItem>
                {
                    new SelectListItem { Value = "gk", Text = "Grand Knight" },
                    new SelectListItem { Value = "fs", Text = "Financial Secretary" }
                };

                return Page(); // redisplay with validation errors
            }
            //--------------------------------------------------------------------
            // 8/15/2025 Tim Philomeno
            // Process form data here (e.g., send email, save to DB)
            // check to see if the selected recipient, gk or fs has an email
            // if not, check to see if the council has a dd and send
            // if not, send to state deputy
            //
            // if check box is checked make the CC the Sender Email address
            //--------------------------------------------------------------------
            // log the email
            var newEmail = new EmailOffice
            {
                All = false,
                Fs = false,
                Gk = false,
                Fn = false,
                Fc = false,
                Dd = false,
                Subject = EmailSubject,
                From  = Email,
                Body = $"{EmailMessage} (This Email Entry was generated using the Council Contact Form - Name = {VName} - Phone = {VPhone} - Council = {CouncilNo} - SendCopy = {SendCopy})",
                DateSent = DateTime.Now
            };
            await _apiHelper.PostAsync<EmailOffice, EmailOffice>("Emails", newEmail);
            //--------------------------------------------------------------------
            //return RedirectToPage("Success"); // or wherever you want to go
            return RedirectToPage("Success", new { userName = VName, councilNo = CouncilNo });
        }
        private async Task LoadCouncilsAsync()
        {
            var iCouncils = await _apiHelper.GetAsync<List<TblValCouncil>>("Councils");
            CouncilOptions = new List<SelectListItem>
            {
                new SelectListItem
                {
                    Value = "",
                    Text = "Select a Council"
                }
            }
            .Concat(iCouncils.Select(c =>
            {
                var fullText = $"{c.CNumber} - {c.CName} (District #{c.District})";
                return new SelectListItem
                {
                    Value = c.CNumber.ToString(),
                    Text = fullText.Length > 50 ? fullText.Substring(0, 50) + "..." : fullText
                };
            })).ToList();
        }
    }
}
