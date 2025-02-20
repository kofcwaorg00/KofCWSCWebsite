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
                    List<TblValCouncil> myMissMil = new List<TblValCouncil>();
                    // first add the new location
                    var result = await _apiHelper.PostAsync<CvnLocation, CvnLocation>("Locations", cvnLocation);

                    // then get all councils and spin through them adding to the mileage table for all
                    var councils = await _apiHelper.GetAsync<IEnumerable<TblValCouncil>>($"Councils");
                    foreach (var council in councils)
                    {
                        if (!await AddorUpdateMileageTable(council, cvnLocation))
                        {
                            myMissMil.Add(council);
                            // allow this to continue and log the missing council information
                            //var ex = new Exception($"Council {council.CNumber} is missing mileage entry for {cvnLocation.Location}");
                            //Log.Information(Utils.FormatLogEntry(this, ex));
                        };
                    }
                    // after we are done then report back councils that did not make it
                    return View("CouncilsMissingMileage", myMissMil);
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
                // Need to check to see if there is a valid location address
                USPSAddress myAddr = new USPSAddress();
                myAddr.Address = new Address 
                { 
                    StreetAddress = cvnLocation.Address,
                    City = cvnLocation.City,
                    State = cvnLocation.State,
                    ZIPCode = cvnLocation.ZipCode
                };


                var myValidAddr = await _apiHelper.PostAsync<USPSAddress,USPSAddress>($"ValidateAddress",myAddr);
                if (myValidAddr.Address == null)
                {
                    ModelState.AddModelError(string.Empty,"Address is invalid");
                    return View(cvnLocation);
                }
                else
                {
                    // go ahead and update what was entered with what we got back
                    // from the USPS
                    cvnLocation.Address = myValidAddr.Address.StreetAddress;
                    cvnLocation.City = myValidAddr.Address.City;
                    cvnLocation.State = myValidAddr.Address.State;
                    cvnLocation.ZipCode = myValidAddr.Address.ZIPCode;
                }
                //*****************************************************************************************************
                // 12/05/2024 Tim Philomeno
                // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                // call the API
                try
                {
                    var result = await _apiHelper.PutAsync<CvnLocation, CvnLocation>($"/Locations/{id}", cvnLocation);
                    // ok now that we know we have a valid address and it has been posted, now update the mileage

                    List<TblValCouncil> myMissMil = new List<TblValCouncil>();

                    // then get all councils and spin through them adding to the mileage table for all
                    var councils = await _apiHelper.GetAsync<IEnumerable<TblValCouncil>>($"Councils");
                    foreach (var council in councils)
                    {
                        if (!await AddorUpdateMileageTable(council, cvnLocation))
                        {
                            myMissMil.Add(council);
                            // allow this to continue and log the missing council information
                            //var ex = new Exception($"Council {council.CNumber} is missing mileage entry for {cvnLocation.Location}");
                            //Log.Information(Utils.FormatLogEntry(this, ex));
                        };
                    }
                    // after we are done then report back councils that did not make it
                    return View("CouncilsMissingMileage", myMissMil);
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

        private async Task<bool> AddorUpdateMileageTable(TblValCouncil council, CvnLocation cvnLocation)
        {
            try
            {
                //*****************************************************************************************************
                // 2/6/2025 Tim Philomeno
                // we need to be able to use the address as well as city, state and zip if possible
                var venueEP = GetEndPointFromAddress(cvnLocation.Address, cvnLocation.City, cvnLocation.State, cvnLocation.ZipCode);
                var councilEP = GetEndPointFromAddress(council.PhyAddress, council.PhyCity, council.PhyState, council.PhyPostalCode);
                // First see if the council already has a mileage entry for the incoming location
                var existingCmil = await _apiHelper.GetAsync<CvnMileage>($"MileageForCouncil/{council.CNumber}/{cvnLocation.Location}");
                var distance = await _apiHelper.GetAsync<AzureMapsDistance>($"DriveDistance/{venueEP}/{councilEP}");
                int rounddistance = (int)Math.Ceiling(distance.DistanceInMiles);
                if (distance.DistanceInMiles < 0)
                {
                    // This is an error condition, probably one of the addresses is blank or invalid
                    //Exception ex = new Exception($"Council {council.CNumber} is missing Physical Address");
                    //Log.Error(Utils.FormatLogEntry(this, ex));
                    return false;
                }

                if (existingCmil is null)
                {
                    // Add New a new one
                    var newMileage = new CvnMileage
                    {
                        Council = council.CNumber,
                        Location = cvnLocation.Location,
                        Mileage = rounddistance
                    };
                    var addmil = await _apiHelper.PostAsync<CvnMileage, CvnMileage>($"Mileage", newMileage);
                    return true;
                }
                else
                {
                    // Update the existing one
                    existingCmil.Mileage = rounddistance;
                    var updmil = await _apiHelper.PutAsync<CvnMileage, CvnMileage>($"Mileage/{existingCmil.Id}",existingCmil);
                    return true;
                }


            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return false;
            }
        }
        private string GetEndPointFromAddress(string address, string city, string state, string zipcode)
        {
            //***************************************************************************************
            // 2/7/2025 Tim Philomeno
            //  We need a consistant way to deal with endpoints(address) to pass to the mapping API
            //  so the incomming 
            if (address.IsNullOrEmpty())
            {
                if (city.IsNullOrEmpty())
                {
                    if (state.IsNullOrEmpty())
                    {
                        if (zipcode.IsNullOrEmpty())
                        {
                            return "z";
                        }
                        return string.Concat(zipcode);
                    }
                    return string.Concat(state," ", zipcode);
                }
                return string.Concat(city,",",state," ", zipcode);
            }
            return string.Concat(address,", ",city,", ",state," ",zipcode);
        }
    }
}
