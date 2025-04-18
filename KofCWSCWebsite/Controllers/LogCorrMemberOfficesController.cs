using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Pages.Forms;

namespace KofCWSCWebsite.Controllers
{
    public class LogCorrMemberOfficesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiHelper _apiHelper;

        public LogCorrMemberOfficesController(ApplicationDbContext context,ApiHelper apiHelper)
        {
            _context = context;
            _apiHelper = apiHelper;
        }

        // GET: LogCorrMemberOffices
        public async Task<IActionResult> Index()
        {
            var results = await _apiHelper.GetAsync<List<LogCorrMemberOfficeVM>>("LogCorrMemberOffices");
            
            return View(results);
        }

        public async Task<IActionResult> UpdateProcessFlag(int id)
        {
            var results = await _apiHelper.GetAsync<int>($"ProcLogCorrMemberOffices/{id}");
            return RedirectToAction("Index");
        }
        // GET: LogCorrMemberOffices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblLogCorrMemberOffice = await _context.TblLogCorrMemberOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblLogCorrMemberOffice == null)
            {
                return NotFound();
            }

            return View(tblLogCorrMemberOffice);
        }

        // GET: LogCorrMemberOffices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: LogCorrMemberOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,ChangeType,ChangeDate,MemberId,OfficeId,PrimaryOffice,Year,District,Council,Assembly,Processed")] LogCorrMemberOffice tblLogCorrMemberOffice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblLogCorrMemberOffice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblLogCorrMemberOffice);
        }

        // GET: LogCorrMemberOffices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblLogCorrMemberOffice = await _context.TblLogCorrMemberOffices.FindAsync(id);
            if (tblLogCorrMemberOffice == null)
            {
                return NotFound();
            }
            return View(tblLogCorrMemberOffice);
        }

        // POST: LogCorrMemberOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,ChangeType,ChangeDate,MemberId,OfficeId,PrimaryOffice,Year,District,Council,Assembly,Processed")] LogCorrMemberOffice tblLogCorrMemberOffice)
        {
            if (id != tblLogCorrMemberOffice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblLogCorrMemberOffice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblLogCorrMemberOfficeExists(tblLogCorrMemberOffice.Id))
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
            return View(tblLogCorrMemberOffice);
        }

        // GET: LogCorrMemberOffices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblLogCorrMemberOffice = await _context.TblLogCorrMemberOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblLogCorrMemberOffice == null)
            {
                return NotFound();
            }

            return View(tblLogCorrMemberOffice);
        }

        // POST: LogCorrMemberOffices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblLogCorrMemberOffice = await _context.TblLogCorrMemberOffices.FindAsync(id);
            if (tblLogCorrMemberOffice != null)
            {
                _context.TblLogCorrMemberOffices.Remove(tblLogCorrMemberOffice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblLogCorrMemberOfficeExists(int id)
        {
            return _context.TblLogCorrMemberOffices.Any(e => e.Id == id);
        }
    }
}
