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
    public class TblValCouncilsController : Controller
    {
        private DataSetService _dataSetService;

        public TblValCouncilsController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblValCouncils
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Councils");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblValCouncil> councils;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblValCouncil>>();
                    readTask.Wait();
                    councils = readTask.Result;
                }
                else
                {
                    councils = Enumerable.Empty<TblValCouncil>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(councils);
            }
        }

        // GET: TblValCouncils/Details/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Council/" + id.ToString());

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValCouncil? council;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    council = JsonConvert.DeserializeObject<TblValCouncil>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    council = null;
                }
                return View(council);
            }
        }

        // GET: TblValCouncils/Create
        [Authorize(Roles = "Admin,DataAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValCouncils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status")] TblValCouncil tblValCouncil)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Council");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblValCouncil);
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

        // GET: TblValCouncils/Edit/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Council/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValCouncil? council;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    council = JsonConvert.DeserializeObject<TblValCouncil>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    council = null;
                }
                return View(council);
            }
        }

        // POST: TblValCouncils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
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
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Council/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblValCouncil);
                        var returnValue = await response.Content.ReadAsAsync<List<TblValCouncil>>();
                        Log.Information("Update of Council ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Council ID " + id);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblValCouncils/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Council/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValCouncil? council;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    council = JsonConvert.DeserializeObject<TblValCouncil>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    council = null;
                }
                return View(council);
            }
        }

        // POST: TblValCouncils/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Council/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblValCouncil? council;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Member Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        council = JsonConvert.DeserializeObject<TblValCouncil>(json);
                    }
                    else
                    {
                        Log.Information("Delete Member Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        council = null;
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

        //private bool TblValCouncilExists(int id)
        //{
        //    return _context.TblValCouncils.Any(e => e.CNumber == id);
        //}
    }
}
