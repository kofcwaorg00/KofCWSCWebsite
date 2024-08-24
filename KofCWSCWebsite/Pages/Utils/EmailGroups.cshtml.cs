using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using KofCWSCWebsite;
using Microsoft.AspNetCore.Http.HttpResults;
using System.ComponentModel.DataAnnotations;


namespace KofCWSCWebsite.Pages.Utils
{
    public class EmailGroupsModel : PageModel
    {
        public EmailGroupsModel()
        {
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
        }
        public async Task<IActionResult> OnPostAsync(string returnUrl = null)
        {
            try
            {
                bool mysuccess = false;
                returnUrl ??= Url.Content("~/");
                //check to see that at least one is checked
                //if (!(Input.cFN || Input.cFN || Input.cGK || Input.cFC || Input.cAll))
                //{
                //    throw new Exception("You must select at least one group");
                //}
                //if (Input.cFS)
                //{
                //    mysuccess = true;// Services.Utils.SendEmailAuthenticatedMG("AllFSs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null);
                //}
                //if (Input.cGK)
                //{
                //    mysuccess = true;//Services.Utils.SendEmailAuthenticatedMG("AllGKs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null);
                //}
                //if (Input.cFN)
                //{
                //    mysuccess = true;//Services.Utils.SendEmailAuthenticatedMG("AllFNs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null);
                //}
                //if (Input.cFC)
                //{
                //    mysuccess = true;//Services.Utils.SendEmailAuthenticatedMG("AllFCs@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null);
                //}
                //if (Input.cAll)
                //{
                //    mysuccess = true;//Services.Utils.SendEmailAuthenticatedMG("AllMembers@mg.kofc-wa.org", Input.sFrom, "", "", Input.sSubject, Input.sBody, null);
                //}
                //if (mysuccess)
                //{
                //    return Redirect(returnUrl);
                //}
                //else
                //{
                //    return Page();
                //}

                //Testing
                if (Services.Utils.SendEmailAuthenticatedAZ("testing@mg.kofc-wa.org", "webmaster@kofc-wa.org", "", "", "testing new site email", "this is a test", null))
                {
                    return Page();
                }
                else
                {
                    return Page();
                }

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
