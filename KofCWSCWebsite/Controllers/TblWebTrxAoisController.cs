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
using Microsoft.CodeAnalysis.VisualBasic.Syntax;

namespace KofCWSCWebsite.Controllers
{
    public class TblWebTrxAoisController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;

        public TblWebTrxAoisController(DataSetService dataSetService, ApiHelper apiHelper)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
        }

        // GET: TblWebTrxAois
        public async Task<IActionResult> Index()
        {
            var aois = await _apiHelper.GetAsync<IList<TblWebTrxAoi>>("/Aois");
            // NOTE if aois is null, that usually means there we got a "NoContent" back
            // from the API.  Our view has an if block that handles this
            return View(aois);
        }

        // GET: TblWebTrxAois/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebTrxAoi? aoi;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    aoi = JsonConvert.DeserializeObject<TblWebTrxAoi>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    aoi = null;
                }
                return View(aoi);
            }
        }

        // GET: TblWebTrxAois/Create
        [Authorize(Roles = "Admin,AOIAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblWebTrxAois/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,AOIAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Type,Title,GraphicUrl,Text,LinkUrl,PostedDate,Expired")] TblWebTrxAoi tblWebTrxAoi)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblWebTrxAoi);
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

        // GET: TblWebTrxAois/Edit/5
        [Authorize(Roles = "Admin,AOIAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebTrxAoi? aoi;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    aoi = JsonConvert.DeserializeObject<TblWebTrxAoi>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    aoi = null;
                }
                return View(aoi);
            }
        }

        // POST: TblWebTrxAois/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,AOIAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Type,Title,GraphicUrl,Text,LinkUrl,PostedDate,Expired")] TblWebTrxAoi tblWebTrxAoi)
        {
            if (id != tblWebTrxAoi.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblWebTrxAoi);
                        var returnValue = await response.Content.ReadAsAsync<List<TblWebTrxAoi>>();
                        Log.Information("Update of AOI ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success AOI ID " + id);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblWebTrxAois/Delete/5
        [Authorize(Roles = "Admin,AOIAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebTrxAoi? aoi;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    aoi = JsonConvert.DeserializeObject<TblWebTrxAoi>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    aoi = null;
                }
                return View(aoi);
            }
        }

        // POST: TblWebTrxAois/Delete/5
        [Authorize(Roles = "Admin,AOIAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Aoi/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblWebTrxAoi? aoi;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete AOI Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        aoi = JsonConvert.DeserializeObject<TblWebTrxAoi>(json);
                    }
                    else
                    {
                        Log.Information("Delete AOI Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        aoi = null;
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

        //private bool TblWebTrxAoiExists(int id)
        //{
        //    return _context.TblWebTrxAois.Any(e => e.Id == id);
        //}
    }
}
