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
    public class TblValOfficesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblValOfficesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblValOffices
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblValOffice.ToListAsync());
        }

        // GET: TblValOffices/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValOffice = await _context.TblValOffice
                .FirstOrDefaultAsync(m => m.OfficeId == id);
            if (tblValOffice == null)
            {
                return NotFound();
            }

            return View(tblValOffice);
        }

        // GET: TblValOffices/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficeId,OfficeDescription,DirSortOrder,AltDescription,EmailAlias,UseAsFormalTitle,WebPageTagLine,SupremeUrl")] TblValOffice tblValOffice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblValOffice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblValOffice);
        }

        // GET: TblValOffices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValOffice = await _context.TblValOffice.FindAsync(id);
            if (tblValOffice == null)
            {
                return NotFound();
            }
            return View(tblValOffice);
        }

        // POST: TblValOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficeId,OfficeDescription,DirSortOrder,AltDescription,EmailAlias,UseAsFormalTitle,WebPageTagLine,SupremeUrl")] TblValOffice tblValOffice)
        {
            if (id != tblValOffice.OfficeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblValOffice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblValOfficeExists(tblValOffice.OfficeId))
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
            return View(tblValOffice);
        }

        // GET: TblValOffices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValOffice = await _context.TblValOffice
                .FirstOrDefaultAsync(m => m.OfficeId == id);
            if (tblValOffice == null)
            {
                return NotFound();
            }

            return View(tblValOffice);
        }

        // POST: TblValOffices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblValOffice = await _context.TblValOffice.FindAsync(id);
            if (tblValOffice != null)
            {
                _context.TblValOffice.Remove(tblValOffice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblValOfficeExists(int id)
        {
            return _context.TblValOffice.Any(e => e.OfficeId == id);
        }
    }
}
