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
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblWebTrxEmailOffices.ToListAsync());
        }

        // GET: EmailOffices/Details/5
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
        public IActionResult Create()
        {
            return View();
        }

        // POST: EmailOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Subject,From,Body,Fs,Gk,Fn,Fc,Dd,All")] EmailOffice emailOffice)
        {
            try
            {
                //Testing
                if (Services.Utils.SendEmailAuthenticatedMG("testing@mg.kofc-wa.org", emailOffice.From, "", "", emailOffice.Subject, emailOffice.Body, null, _configuration))
                {
                    ModelState.AddModelError(string.Empty, "Your email has been sent to the selected groups!");
                    return RedirectToPage("/Utils/EmailGroupsConfirm");
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Email failed.");
                    return RedirectToPage("/error");
                }


                //if (ModelState.IsValid)
                //{
                //    Log.Information("Sending Email");
                //    //_context.Add(emailOffice);
                //    //await _context.SaveChangesAsync();
                //    //return RedirectToAction(nameof(Index));
                //}
                //return RedirectToPage("/Utils/EmailGroupsConfirm");
            }
            catch (Exception ex)
            {
                Log.Error(ex.Message + " - " + ex.InnerException);
                return RedirectToAction("index", "Home");
            }

        }

        // GET: EmailOffices/Edit/5
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
