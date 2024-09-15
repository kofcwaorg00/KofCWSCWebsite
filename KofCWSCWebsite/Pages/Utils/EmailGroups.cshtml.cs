using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KofCWSCWebsite;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;


namespace KofCWSCWebsite.Pages.Utils
{
    public class EmailGroupsModel : PageModel
    {
        private readonly IConfiguration _configuration;
        public EmailGroupsModel(IConfiguration configuration)
        {
            _configuration = configuration; 
        }

        [BindProperty]
        public InputModel Input { get; set; }
        public string ReturnUrl { get; set; }

        public void OnGet()
        {
        }

        public class InputModel
        {
            public string sSubject { get; set; }
            [EmailAddress]
            public string sFrom { get; set; }
            public string sBody { get; set; }
            public bool cFS { get; set; }
            public bool cGK { get; set; }
            public bool cFN { get; set; }
            public bool cFC { get; set; }
            public bool cAll { get; set; }
            public bool cDD { get; set; }
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                bool mysuccess = false;
                returnUrl ??= Url.Content("~/");
                //check to see that at least one is checked
                if (!(Input.cFN || Input.cFN || Input.cGK || Input.cFC || Input.cAll))
                {
                    ModelState.AddModelError(string.Empty, "You must select at least one group");
                    //throw new Exception("You must select at least one group");
                }
                if (Input.cFS)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFSs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To FSs", null, _configuration);
                }
                if (Input.cGK)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllGKs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null,_configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To GKs", null, _configuration);
                }
                if (Input.cFN)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFNs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null,_configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To FNs", null, _configuration);
                }
                if (Input.cFC)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFCs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null,_configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To FCs", null, _configuration);
                }
                if (Input.cAll)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllMembers@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null,_configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To All", null, _configuration);
                }
                if (Input.cDD)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedDASP("AllDDs@kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, "To All", null, _configuration);
                }
                if (mysuccess)
                {
                    return RedirectToPage("EmailGroupsConfirm");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email failed.");
                    return Page();
                }

                //Testing
                //////////if (Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null, _configuration))
                //////////{
                //////////    ModelState.AddModelError(string.Empty, "Your email has been sent to the selected groups!");
                //////////    return RedirectToPage("EmailGroupsConfirm");
                //////////}
                //////////else
                //////////{
                //////////    ModelState.AddModelError(string.Empty, "Email failed.");
                //////////    return Page();
                //////////}
                //return Page();
                // If we got this far, something failed, redisplay form

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                return Page();
            }

            }
        }
    }
