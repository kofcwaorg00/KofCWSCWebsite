using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class CvnMpdController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public CvnMpdController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper; 
        }

        // GET: CvnMpd
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>("MPD");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }

        [HttpGet("ToggleDelegateDaysMPD/{id}/{day}")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> ToggleDelegateDaysMPD(int id, int day)
        {
            var myAffectedRows = _apiHelper.GetAsync<int>($"/ToggleDelegateDays/{id}/{day}");
            //var myViewT1 = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>("MPD");
            return RedirectToAction("Index");

        }
        public async Task<ActionResult> PrintChecks()
        {
            return View();
        }

        ////////////// GET: CvnMpd/Details/5
        ////////////public async Task<IActionResult> Details(int? id)
        ////////////{
        ////////////    if (id == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    var cvnMpd = await _context.TblCvnTrxMpds
        ////////////        .FirstOrDefaultAsync(m => m.Id == id);
        ////////////    if (cvnMpd == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    return View(cvnMpd);
        ////////////}

        ////////////// GET: CvnMpd/Create
        ////////////public IActionResult Create()
        ////////////{
        ////////////    return View();
        ////////////}

        ////////////// POST: CvnMpd/Create
        ////////////// To protect from overposting attacks, enable the specific properties you want to bind to.
        ////////////// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ////////////[HttpPost]
        ////////////[ValidateAntiForgeryToken]
        ////////////public async Task<IActionResult> Create([Bind("Id,MemberId,Council,District,Group,Office,Payee,CheckNumber,CheckDate,Day1,Day2,Day3,Day1G,Day2G,Day3G,Miles,CheckTotal,Location")] CvnMpd cvnMpd)
        ////////////{
        ////////////    if (ModelState.IsValid)
        ////////////    {
        ////////////        _context.Add(cvnMpd);
        ////////////        await _context.SaveChangesAsync();
        ////////////        return RedirectToAction(nameof(Index));
        ////////////    }
        ////////////    return View(cvnMpd);
        ////////////}

        ////////////// GET: CvnMpd/Edit/5
        ////////////public async Task<IActionResult> Edit(int? id)
        ////////////{
        ////////////    if (id == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    var cvnMpd = await _context.TblCvnTrxMpds.FindAsync(id);
        ////////////    if (cvnMpd == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }
        ////////////    return View(cvnMpd);
        ////////////}

        ////////////// POST: CvnMpd/Edit/5
        ////////////// To protect from overposting attacks, enable the specific properties you want to bind to.
        ////////////// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ////////////[HttpPost]
        ////////////[ValidateAntiForgeryToken]
        ////////////public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,Council,District,Group,Office,Payee,CheckNumber,CheckDate,Day1,Day2,Day3,Day1G,Day2G,Day3G,Miles,CheckTotal,Location")] CvnMpd cvnMpd)
        ////////////{
        ////////////    if (id != cvnMpd.Id)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    if (ModelState.IsValid)
        ////////////    {
        ////////////        try
        ////////////        {
        ////////////            _context.Update(cvnMpd);
        ////////////            await _context.SaveChangesAsync();
        ////////////        }
        ////////////        catch (DbUpdateConcurrencyException)
        ////////////        {
        ////////////            if (!CvnMpdExists(cvnMpd.Id))
        ////////////            {
        ////////////                return NotFound();
        ////////////            }
        ////////////            else
        ////////////            {
        ////////////                throw;
        ////////////            }
        ////////////        }
        ////////////        return RedirectToAction(nameof(Index));
        ////////////    }
        ////////////    return View(cvnMpd);
        ////////////}

        ////////////// GET: CvnMpd/Delete/5
        ////////////public async Task<IActionResult> Delete(int? id)
        ////////////{
        ////////////    if (id == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    var cvnMpd = await _context.TblCvnTrxMpds
        ////////////        .FirstOrDefaultAsync(m => m.Id == id);
        ////////////    if (cvnMpd == null)
        ////////////    {
        ////////////        return NotFound();
        ////////////    }

        ////////////    return View(cvnMpd);
        ////////////}

        ////////////// POST: CvnMpd/Delete/5
        ////////////[HttpPost, ActionName("Delete")]
        ////////////[ValidateAntiForgeryToken]
        ////////////public async Task<IActionResult> DeleteConfirmed(int id)
        ////////////{
        ////////////    var cvnMpd = await _context.TblCvnTrxMpds.FindAsync(id);
        ////////////    if (cvnMpd != null)
        ////////////    {
        ////////////        _context.TblCvnTrxMpds.Remove(cvnMpd);
        ////////////    }

        ////////////    await _context.SaveChangesAsync();
        ////////////    return RedirectToAction(nameof(Index));
        ////////////}

        ////////////private bool CvnMpdExists(int id)
        ////////////{
        ////////////    return _context.TblCvnTrxMpds.Any(e => e.Id == id);
        ////////////}
    }
}
