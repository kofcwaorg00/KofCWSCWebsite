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
    public class TblMasPSOsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblMasPSOsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblMasPSOs
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblMasPsos.ToListAsync());
        }

        // GET: TblMasPSOs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasPso = await _context.TblMasPsos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblMasPso == null)
            {
                return NotFound();
            }

            return View(tblMasPso);
        }

        // GET: TblMasPSOs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasPSOs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,StateDeputy,StateSecretary,StateTreasurer,StateAdvocate,StateWarden")] TblMasPso tblMasPso)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblMasPso);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblMasPso);
        }

        // GET: TblMasPSOs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasPso = await _context.TblMasPsos.FindAsync(id);
            if (tblMasPso == null)
            {
                return NotFound();
            }
            return View(tblMasPso);
        }

        // POST: TblMasPSOs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,StateDeputy,StateSecretary,StateTreasurer,StateAdvocate,StateWarden")] TblMasPso tblMasPso)
        {
            if (id != tblMasPso.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblMasPso);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMasPsoExists(tblMasPso.Id))
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
            return View(tblMasPso);
        }

        // GET: TblMasPSOs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblMasPso = await _context.TblMasPsos
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblMasPso == null)
            {
                return NotFound();
            }

            return View(tblMasPso);
        }

        // POST: TblMasPSOs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblMasPso = await _context.TblMasPsos.FindAsync(id);
            if (tblMasPso != null)
            {
                _context.TblMasPsos.Remove(tblMasPso);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblMasPsoExists(int id)
        {
            return _context.TblMasPsos.Any(e => e.Id == id);
        }
    }
}
