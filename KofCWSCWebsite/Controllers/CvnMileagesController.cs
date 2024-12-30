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
    public class CvnMileagesController : Controller
    {
        private readonly ApiHelper _apiHelper;

        public CvnMileagesController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        // GET: CvnMileages
        public async Task<IActionResult> Index()
        {
            try
            {
                var result = await _apiHelper.GetAsync<IEnumerable<CvnMileage>>("Mileage");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }

        // GET: CvnMileages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            try
            {
                var result = await _apiHelper.GetAsync<CvnMileage>($"Mileage/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }

        // GET: CvnMileages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CvnMileages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Council,Location,Mileage")] CvnMileage cvnMileage)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _apiHelper.PostAsync<CvnMileage, CvnMileage>("/Mileage/", cvnMileage);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
            }
            return View(cvnMileage);
        }

        // GET: CvnMileages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            try
            {
                var result = await _apiHelper.GetAsync<CvnMileage>($"Mileage/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }

        // POST: CvnMileages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Council,Location,Mileage")] CvnMileage cvnMileage)
        {
            if (id != cvnMileage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var result = await _apiHelper.PutAsync<CvnMileage, CvnMileage>($"/Council/{id}", cvnMileage);
                }
                catch (Exception ex)
                {
                    //***************************************************************************************************
                    // 12/05/2024 Tim Philomeno
                    // I want handle these consistantly so we are returning an http response that can be caught here
                    // the response message should appear in the ex.Message, then Log it and allow the method to
                    // finish refreshing the index page3
                    ModelState.AddModelError(string.Empty, ex.Message);
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    //------------------------------------------------------------------------------------------------------
                }
                return RedirectToAction(nameof(Index));
            }
            return View(cvnMileage);
        }

        // GET: CvnMileages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnMileage = await _apiHelper.GetAsync<CvnMileage>($"/Mileage/{id}");

            if (cvnMileage == null)
            {
                return NotFound();
            }

            return View(cvnMileage);
        }

        // POST: CvnMileages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiHelper.DeleteAsync($"/Mileage/{id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
