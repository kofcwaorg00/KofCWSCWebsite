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
    public class TblWebSelfPublishesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblWebSelfPublishesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblWebSelfPublishes
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblWebSelfPublishes.ToListAsync());
        }

        // GET: TblWebSelfPublishes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            ////////if (id == null)
            ////////{
            ////////    return NotFound();
            ////////}

            var tblWebSelfPublish = await _context.TblWebSelfPublishes
                .FirstOrDefaultAsync(m => m.Url == id);
            if (tblWebSelfPublish == null)
            {
                return NotFound();
            }
            return View(tblWebSelfPublish);
        }

        // GET: TblWebSelfPublishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblWebSelfPublishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Url,Data")] TblWebSelfPublish tblWebSelfPublish)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblWebSelfPublish);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblWebSelfPublish);
        }

        // GET: TblWebSelfPublishes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebSelfPublish = await _context.TblWebSelfPublishes.FindAsync(id);
            if (tblWebSelfPublish == null)
            {
                return NotFound();
            }
            return View(tblWebSelfPublish);
        }

        // POST: TblWebSelfPublishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Url,Data,OID")] TblWebSelfPublish tblWebSelfPublish)
        {
            if (id != tblWebSelfPublish.Url)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblWebSelfPublish);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblWebSelfPublishExists(tblWebSelfPublish.Url))
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
            return View(tblWebSelfPublish);
        }

        // GET: TblWebSelfPublishes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebSelfPublish = await _context.TblWebSelfPublishes
                .FirstOrDefaultAsync(m => m.Url == id);
            if (tblWebSelfPublish == null)
            {
                return NotFound();
            }

            return View(tblWebSelfPublish);
        }

        // POST: TblWebSelfPublishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var tblWebSelfPublish = await _context.TblWebSelfPublishes.FindAsync(id);
            if (tblWebSelfPublish != null)
            {
                _context.TblWebSelfPublishes.Remove(tblWebSelfPublish);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblWebSelfPublishExists(string id)
        {
            return _context.TblWebSelfPublishes.Any(e => e.Url == id);
        }
    }
}
