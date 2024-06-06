using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;

namespace KofCWSCWebsite.Controllers
{
    public class TblMasAwardsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblMasAwardsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblMasAwards
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblMasAwards.ToListAsync());
        }

        // GET: TblMasAwards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasAward = await _context.TblMasAwards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblMasAward == null)
            {
                return NotFound();
            }

            return View(tblMasAward);
        }

        // GET: TblMasAwards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasAwards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AwardName,AwardDescription,AwardDueDate,LinkToTheAwardForm,AwardSubmissionEmailAddress")] TblMasAward tblMasAward)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblMasAward);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblMasAward);
        }

        // GET: TblMasAwards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasAward = await _context.TblMasAwards.FindAsync(id);
            if (tblMasAward == null)
            {
                return NotFound();
            }
            return View(tblMasAward);
        }

        // POST: TblMasAwards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AwardName,AwardDescription,AwardDueDate,LinkToTheAwardForm,AwardSubmissionEmailAddress")] TblMasAward tblMasAward)
        {
            if (id != tblMasAward.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblMasAward);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMasAwardExists(tblMasAward.Id))
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
            return View(tblMasAward);
        }

        // GET: TblMasAwards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasAward = await _context.TblMasAwards
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblMasAward == null)
            {
                return NotFound();
            }

            return View(tblMasAward);
        }

        // POST: TblMasAwards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblMasAward = await _context.TblMasAwards.FindAsync(id);
            if (tblMasAward != null)
            {
                _context.TblMasAwards.Remove(tblMasAward);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblMasAwardExists(int id)
        {
            return _context.TblMasAwards.Any(e => e.Id == id);
        }
    }
}
