using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace KofCWSCWebsite.Controllers
{
    public class TblValAssysController : Controller
    {
        private DataSetService _dataSetService;

        public TblValAssysController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;   
        }

        // GET: TblValAssys
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Assys");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblValAssy> assys;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblValAssy>>();
                    readTask.Wait();
                    assys = readTask.Result;
                }
                else
                {
                    assys = Enumerable.Empty<TblValAssy>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(assys);
            }
        }

        // GET: TblValAssys/Details/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValAssy? assy;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    assy = JsonConvert.DeserializeObject<TblValAssy>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    assy = null;
                }
                return View(assy);
            }
        }

        // GET: TblValAssys/Create
        [Authorize(Roles = "Admin,DataAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValAssys/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("ANumber,ALocation,AName,AddInfo1,AddInfo2,AddInfo3,WebSiteUrl,MasterLoc,Status")] TblValAssy tblValAssy)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblValAssy);
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
            return RedirectToAction(nameof(Index));
        }

        // GET: TblValAssys/Edit/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValAssy? assy;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    assy = JsonConvert.DeserializeObject<TblValAssy>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    assy = null;
                }
                return View(assy);
            }
        }

        // POST: TblValAssys/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("ANumber,ALocation,AName,AddInfo1,AddInfo2,AddInfo3,WebSiteUrl,MasterLoc,Status")] TblValAssy tblValAssy)
        {
            if (id != tblValAssy.ANumber)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblValAssy);
                        var returnValue = await response.Content.ReadAsAsync<List<TblValAssy>>();
                        Log.Information("Update of Assembly ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Assembly ID " + id);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblValAssys/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValAssy? assy;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    assy = JsonConvert.DeserializeObject<TblValAssy>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    assy = null;
                }
                return View(assy);
            }
        }

        // POST: TblValAssys/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Assy/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblValAssy? assy;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Member Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        assy = JsonConvert.DeserializeObject<TblValAssy>(json);
                    }
                    else
                    {
                        Log.Information("Delete Member Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        assy = null;
                    }
                    return RedirectToAction(nameof(Index));
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message + " " + ex.InnerException);
                return NoContent();
            }
        }

        //private bool TblValAssyExists(int id)
        //{
        //    return _context.TblValAssys.Any(e => e.ANumber == id);
        //}
    }
}
