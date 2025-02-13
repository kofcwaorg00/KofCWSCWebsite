using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Newtonsoft.Json;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class CvnControlsController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;

        public CvnControlsController(DataSetService dataSetService, ApiHelper apiHelper)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper; 
        }

        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var control = await _apiHelper.GetAsync<CvnControl>($"Control/{id}");

            // we need to fetch a list of LOCATIONS and stuff them into ViewBag for use in the VIEW
            var locations = await _apiHelper.GetAsync<List<CvnLocation>>("Locations");
            ViewBag.ListOfLocations = new SelectList(locations.OrderBy(x => x.Location).ToList(), "Location","Location");

            return View(control);
        }

        // POST: CvnControls/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,LocationString,Address,City,State,ZipCode,MPDDay,MPDMile,Location")] CvnControl cvnControl)
        {
            if (id != cvnControl.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                var control = await _apiHelper.PutAsync<CvnControl, CvnControl>($"Control/{id}",cvnControl);

                //Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Control/" + id);
                //try
                //{
                //    using (var client = new HttpClient())
                //    {
                //        client.BaseAddress = myURI;
                //        var response = await client.PutAsJsonAsync(myURI, cvnControl);
                //        var returnValue = await response.Content.ReadAsAsync<List<CvnControl>>();
                //        Log.Information("Update of CVN Control ID " + id + "Returned " + returnValue);
                //    }
                //}
                //catch (Exception ex)
                //{
                //    Log.Fatal(ex.Message);
                //}
            }
            return RedirectToAction("Index","Home");
        }
    }
}
