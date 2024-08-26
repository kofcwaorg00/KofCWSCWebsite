using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using System.Text.Json;

namespace KofCWSCWebsite.Controllers
{
    public class TblSysTrxEventsController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblSysTrxEventsController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblSysTrxEvents
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblSysTrxEvents.ToListAsync());
        }

        public IActionResult GetCalendarEvents(string start, string end)
        {
            DateTime startdate = DateTime.Parse(start);
            DateTime enddate = DateTime.Parse(end);
            List<TblSysTrxEvent> events = _context
                .TblSysTrxEvents
                .Where(l => l.Begin >= startdate && l.End <= enddate)
                .ToList();
            
            return Json(events);
        }

        // GET: TblSysTrxEvents/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSysTrxEvent = await _context.TblSysTrxEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblSysTrxEvent == null)
            {
                return NotFound();
            }

            return View(tblSysTrxEvent);
        }

        // GET: TblSysTrxEvents/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblSysTrxEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Begin,End,isAllDay,AttachUrl,AddedBy,DateAdded")] TblSysTrxEvent tblSysTrxEvent)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblSysTrxEvent);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblSysTrxEvent);
        }

        // GET: TblSysTrxEvents/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSysTrxEvent = await _context.TblSysTrxEvents.FindAsync(id);
            if (tblSysTrxEvent == null)
            {
                return NotFound();
            }
            return View(tblSysTrxEvent);
        }

        // POST: TblSysTrxEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Begin,End,isAllDay,AttachUrl,AddedBy,DateAdded")] TblSysTrxEvent tblSysTrxEvent)
        {
            if (id != tblSysTrxEvent.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblSysTrxEvent);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblSysTrxEventExists(tblSysTrxEvent.Id))
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
            return View(tblSysTrxEvent);
        }

        // GET: TblSysTrxEvents/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblSysTrxEvent = await _context.TblSysTrxEvents
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblSysTrxEvent == null)
            {
                return NotFound();
            }

            return View(tblSysTrxEvent);
        }

        // POST: TblSysTrxEvents/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblSysTrxEvent = await _context.TblSysTrxEvents.FindAsync(id);
            if (tblSysTrxEvent != null)
            {
                _context.TblSysTrxEvents.Remove(tblSysTrxEvent);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblSysTrxEventExists(int id)
        {
            return _context.TblSysTrxEvents.Any(e => e.Id == id);
        }
    }
}
