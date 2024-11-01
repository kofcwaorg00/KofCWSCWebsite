using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Serilog;
using static com.sun.net.httpserver.Authenticator;
using Microsoft.AspNetCore.Authorization;
namespace KofCWSCWebsite.Controllers
{
    public class EmailOfficesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly IConfiguration _configuration;

        public EmailOfficesController(ApplicationDbContext context, IConfiguration configuration)
        {
            _context = context;
            this._configuration = configuration;
        }

        // GET: EmailOffices
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblWebTrxEmailOffices.ToListAsync());
        }

        // GET: EmailOffices/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailOffice = await _context.TblWebTrxEmailOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailOffice == null)
            {
                return NotFound();
            }

            return View(emailOffice);
        }

        // GET: EmailOffices/Create
        [Authorize(Roles = "Admin,StateOfficer,StateChairman")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,StateOfficer,StateChairman")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,From,Body,Fs,Gk,Fn,Fc,Dd,All")] EmailOffice emailOffice)
        {
            try
            {
                if (!(emailOffice.Fs || emailOffice.Fn || emailOffice.Gk || emailOffice.Fc || emailOffice.All || emailOffice.Dd))
                {
                    //*******************************************************************************
                    // 09/20/2024 Tim Philomeno
                    // this should never happen because we have jquery to make sure at least one
                    // checkbox is checked.
                    ModelState.AddModelError(string.Empty, "You must select at least one group");
                    return RedirectToAction("Create", "EmailOffices");
                    //--------------------------------------------------------------------------------
                }
                //*******************************************************************************
                // 09/20/2024 Tim Philomeno
                // Let's log the email message and then send it.  That way we have a record
                // of what was sent.
                if (ModelState.IsValid)
                {
                    Log.Information("Sending Email");
                    _context.Add(emailOffice);
                    await _context.SaveChangesAsync();
               }
                //--------------------------------------------------------------------------------
                //Testing
                //////////if (Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration))
                //////////{
                //////////    ModelState.AddModelError(string.Empty, "Your email has been sent to the selected groups!");
                //////////    return RedirectToPage("/Utils/EmailGroupsConfirm");
                //////////}
                //////////else
                //////////{
                //////////    ModelState.AddModelError(string.Empty, "Email failed.");
                //////////    return RedirectToPage("/error");
                //////////}
                //*******************************************************************************
                // 09/20/2024 Tim Philomeno
                // ok let's do the deed
                bool mysuccess = false;
                if (emailOffice.Fs)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFSs@mg.kofc-wa.org",emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedAZ("tphilomeno@comcast.net", emailOffice.From, "", "", emailOffice.Subject,emailOffice.Body , null,_configuration);
                }
                if (emailOffice.Gk)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllGKs@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, "FROM GK", null, _configuration);
                }
                if (emailOffice.Fn)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFNs@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, "FROM FN", null, _configuration);
                }
                if (emailOffice.Fc)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllFCs@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, "FROM FC", null, _configuration);
                }
                if (emailOffice.All)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedMG("AllMembers@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, "FROM ALL", null, _configuration);
                }
                if (emailOffice.Dd)
                {
                    mysuccess = Services.Utils.SendEmailAuthenticatedAZ("AllDDs@kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);
                    //mysuccess = Services.Utils.SendEmailAuthenticatedDASP("webmaster@kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, "FROM DD", null, _configuration);
                }
                if (mysuccess)
                {
                    // send a copy to the originator before returning the final confirmation of delivery
                    mysuccess = Services.Utils.SendEmailAuthenticatedAZ(emailOffice.From, emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration);

                    return RedirectToPage("/Utils/EmailGroupsConfirm");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email failed.");
                    return View();
                }
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                return RedirectToAction("index", "Home");
            }

        }

        // GET: EmailOffices/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailOffice = await _context.TblWebTrxEmailOffices.FindAsync(id);
            if (emailOffice == null)
            {
                return NotFound();
            }
            return View(emailOffice);
        }

        // POST: EmailOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Subject,From,Body,Fs,Gk,Fn,Fc,Dd,All")] EmailOffice emailOffice)
        {
            if (id != emailOffice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(emailOffice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!EmailOfficeExists(emailOffice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(emailOffice);
        }

        // GET: EmailOffices/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var emailOffice = await _context.TblWebTrxEmailOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (emailOffice == null)
            {
                return NotFound();
            }

            return View(emailOffice);
        }

        // POST: EmailOffices/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var emailOffice = await _context.TblWebTrxEmailOffices.FindAsync(id);
            if (emailOffice != null)
            {
                _context.TblWebTrxEmailOffices.Remove(emailOffice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool EmailOfficeExists(int id)
        {
            return _context.TblWebTrxEmailOffices.Any(e => e.Id == id);
        }
    }
}
