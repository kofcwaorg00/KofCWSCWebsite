﻿using System;
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
    public class CvnImpDelegatesLogsController : Controller
    {
        private readonly ApplicationDbContext _context;
        private DataSetService _dataSetService;

        public CvnImpDelegatesLogsController(ApplicationDbContext context, DataSetService dataSetService)
        {
            _context = context;
            _dataSetService = dataSetService;
        }

        // GET: CvnImpDelegatesLogs
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblCvnImpDelegatesLogs.ToListAsync());
        }

        // GET: CvnImpDelegatesLogs/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegatesLog = await _context.TblCvnImpDelegatesLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cvnImpDelegatesLog == null)
            {
                return NotFound();
            }

            return View(cvnImpDelegatesLog);
        }

        // GET: CvnImpDelegatesLogs/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CvnImpDelegatesLogs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Guid,Rundate,Type,MemberId,Data")] CvnImpDelegatesLog cvnImpDelegatesLog)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/CreateLogEntry");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, cvnImpDelegatesLog);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + ' ' + ex.InnerException);
                    }
                }
            }


            if (ModelState.IsValid)
            {
                _context.Add(cvnImpDelegatesLog);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cvnImpDelegatesLog);
        }

        // GET: CvnImpDelegatesLogs/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegatesLog = await _context.TblCvnImpDelegatesLogs.FindAsync(id);
            if (cvnImpDelegatesLog == null)
            {
                return NotFound();
            }
            return View(cvnImpDelegatesLog);
        }

        // POST: CvnImpDelegatesLogs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Guid,Rundate,Type,MemberId,Data")] CvnImpDelegatesLog cvnImpDelegatesLog)
        {
            if (id != cvnImpDelegatesLog.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cvnImpDelegatesLog);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CvnImpDelegatesLogExists(cvnImpDelegatesLog.Id))
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
            return View(cvnImpDelegatesLog);
        }

        // GET: CvnImpDelegatesLogs/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegatesLog = await _context.TblCvnImpDelegatesLogs
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cvnImpDelegatesLog == null)
            {
                return NotFound();
            }

            return View(cvnImpDelegatesLog);
        }

        // POST: CvnImpDelegatesLogs/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cvnImpDelegatesLog = await _context.TblCvnImpDelegatesLogs.FindAsync(id);
            if (cvnImpDelegatesLog != null)
            {
                _context.TblCvnImpDelegatesLogs.Remove(cvnImpDelegatesLog);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CvnImpDelegatesLogExists(int id)
        {
            return _context.TblCvnImpDelegatesLogs.Any(e => e.Id == id);
        }
    }
}