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
using Microsoft.AspNetCore.Authorization;
using KofCWSC.API.Models;
using Microsoft.IdentityModel.Tokens;

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
        [Authorize(Roles = "Admin, ConventionAdmin")]
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
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: CvnLocations/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Create([Bind("Id,Location,Address,City,State,ZipCode")] CvnLocation cvnLocation)
        {
            if (ModelState.IsValid)
            {
                try
                {
                    // first add the new location
                    var result = await _apiHelper.PostAsync<CvnLocation, CvnLocation>("Locations", cvnLocation);

                    // then get all councils and spin through them adding to the mileage table for all
                    var councils = await _apiHelper.GetAsync<IEnumerable<TblValCouncil>>($"Councils");
                    foreach (var council in councils)
                    {
                        await AddorUpdateMileageTable(council, cvnLocation);
                    }
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
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
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
            //------------------------------------------------------------------------------------------------------
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Location,Address,City,State,ZipCode")] CvnLocation cvnLocation)
        {
            if (id != cvnLocation.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //*****************************************************************************************************
                // 12/05/2024 Tim Philomeno
                // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                // call the API
                try
                {
                    var result = await _apiHelper.PutAsync<CvnLocation, CvnLocation>($"/Locations/{id}", cvnLocation);
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
            }
            return RedirectToAction(nameof(Index));
        }
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<CvnLocation>($"/Locations/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
            //------------------------------------------------------------------------------------------------------

        }
        [Authorize(Roles = "Admin, ConventionAdmin")]
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
        [Authorize(Roles = "Admin, ConventionAdmin")]
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

        private async Task<bool> AddorUpdateMileageTable(TblValCouncil council,CvnLocation cvnLocation)
        {
            try
            {
                //*****************************************************************************************************
                // 2/6/2025 Tim Philomeno
                // we need to be able to use the address as well as city, state and zip if possible
                var venueAddress = string.Concat(cvnLocation.Address,", ", cvnLocation.City,", ", cvnLocation.State," ", cvnLocation.ZipCode);
                var councilAddress = string.Concat(council.PhyAddress,", ", council.PhyCity,", ", council.PhyState," ",council.PhyPostalCode);
                // First see if the council already has a mileage entry for the incoming location
                var existingCouncil = await _apiHelper.GetAsync<CvnMileage>($"MileageForCouncil/{council.CNumber}/{cvnLocation.Location}");
                var distance = await _apiHelper.GetAsync<AzureMapsDistance>($"DriveDistance/{venueAddress}/{councilAddress}");
                if (distance.DistanceInKilometers < 0)
                {
                    // This is an error condition, probably one of the addresses is blank or invalid
                    Exception ex = new Exception($"Council {council.CNumber} is missing Physical Address");
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    return false;
                }

                if (existingCouncil is null)
                {
                    // Add New
                    return true;
                }
                else
                {
                    // Update
                    return true;
                }


            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return false;
            }

        }
    }
}
