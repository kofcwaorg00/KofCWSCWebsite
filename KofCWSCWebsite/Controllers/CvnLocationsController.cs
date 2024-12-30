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
    public class CvnLocationsController : Controller
    {
        private readonly ApiHelper _apiHelper;
        public CvnLocationsController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper; 
        }

        // GET: CvnLocations
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiHelper.GetAsync<IEnumerable<CvnLocation>>("Locations");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }

        public IActionResult Create()
        {
            return View();
        }

        // POST: CvnLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Location")] CvnLocation cvnLocation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _apiHelper.PostAsync<CvnLocation, CvnLocation>("Locations", cvnLocation);
                }
                catch (Exception ex)
                {
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _apiHelper.GetAsync<CvnLocation>($"/Locations/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: CvnLocations/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiHelper.DeleteAsync($"/Locations/{id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ViewBag.DelLocError = "Delete Failed probably due to a Location being in use for other Mileage Entries!";
                Log.Error(Utils.FormatLogEntry(this, ex));
                return View();
            }
        }
    }
}
