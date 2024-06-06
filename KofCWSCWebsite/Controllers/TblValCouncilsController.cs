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
    public class TblValCouncilsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblValCouncilsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblValCouncils
        public async Task<IActionResult> Index()
        {
            var filteredSortedList = await _context.TblValCouncils
                .Where(x => x.CNumber > 0)
                .OrderBy(x => x.CNumber)
                .ToListAsync();
            return View(filteredSortedList);
            //return View(await _context.TblValCouncils.OrderBy(x => x.CNumber).ToListAsync());
        }

        // GET: TblValCouncils/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValCouncil = await _context.TblValCouncils
                .FirstOrDefaultAsync(m => m.CNumber == id);
            if (tblValCouncil == null)
            {
                return NotFound();
            }

            return View(tblValCouncil);
        }

        // GET: TblValCouncils/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValCouncils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status")] TblValCouncil tblValCouncil)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblValCouncil);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblValCouncil);
        }

        // GET: TblValCouncils/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValCouncil = await _context.TblValCouncils.FindAsync(id);
            if (tblValCouncil == null)
            {
                return NotFound();
            }
            return View(tblValCouncil);
        }

        // POST: TblValCouncils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status")] TblValCouncil tblValCouncil)
        {
            if (id != tblValCouncil.CNumber)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblValCouncil);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblValCouncilExists(tblValCouncil.CNumber))
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
            return View(tblValCouncil);
        }

        // GET: TblValCouncils/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblValCouncil = await _context.TblValCouncils
                .FirstOrDefaultAsync(m => m.CNumber == id);
            if (tblValCouncil == null)
            {
                return NotFound();
            }

            return View(tblValCouncil);
        }

        // POST: TblValCouncils/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblValCouncil = await _context.TblValCouncils.FindAsync(id);
            if (tblValCouncil != null)
            {
                _context.TblValCouncils.Remove(tblValCouncil);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblValCouncilExists(int id)
        {
            return _context.TblValCouncils.Any(e => e.CNumber == id);
        }
    }
}
