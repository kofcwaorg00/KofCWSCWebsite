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
    public class TblWebTrxAoisController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblWebTrxAoisController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblWebTrxAois
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblWebTrxAois.ToListAsync());
        }

        // GET: TblWebTrxAois/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebTrxAoi = await _context.TblWebTrxAois
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblWebTrxAoi == null)
            {
                return NotFound();
            }

            return View(tblWebTrxAoi);
        }

        // GET: TblWebTrxAois/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblWebTrxAois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Title,GraphicUrl,Text,LinkUrl,PostedDate,Expired")] TblWebTrxAoi tblWebTrxAoi)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblWebTrxAoi);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblWebTrxAoi);
        }

        // GET: TblWebTrxAois/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebTrxAoi = await _context.TblWebTrxAois.FindAsync(id);
            if (tblWebTrxAoi == null)
            {
                return NotFound();
            }
            return View(tblWebTrxAoi);
        }

        // POST: TblWebTrxAois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Title,GraphicUrl,Text,LinkUrl,PostedDate,Expired")] TblWebTrxAoi tblWebTrxAoi)
        {
            if (id != tblWebTrxAoi.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblWebTrxAoi);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblWebTrxAoiExists(tblWebTrxAoi.Id))
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
            return View(tblWebTrxAoi);
        }

        // GET: TblWebTrxAois/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebTrxAoi = await _context.TblWebTrxAois
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblWebTrxAoi == null)
            {
                return NotFound();
            }

            return View(tblWebTrxAoi);
        }

        // POST: TblWebTrxAois/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblWebTrxAoi = await _context.TblWebTrxAois.FindAsync(id);
            if (tblWebTrxAoi != null)
            {
                _context.TblWebTrxAois.Remove(tblWebTrxAoi);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblWebTrxAoiExists(int id)
        {
            return _context.TblWebTrxAois.Any(e => e.Id == id);
        }
    }
}
