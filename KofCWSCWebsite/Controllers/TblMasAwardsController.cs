using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Data;
using Serilog;
using Newtonsoft.Json;

namespace KofCWSCWebsite.Controllers
{
    public class TblMasAwardsController : Controller
    {
        private DataSetService _dataSetService;

        public TblMasAwardsController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblMasAwards
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Awards");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblMasAward> awards;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblMasAward>>();
                    readTask.Wait();
                    awards = readTask.Result;
                }
                else
                {
                    awards = Enumerable.Empty<TblMasAward>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(awards);
            }
        }

        // GET: TblMasAwards/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Award/" + id.ToString());

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasAward? award;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    award = JsonConvert.DeserializeObject<TblMasAward>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    award = null;
                }
                return View(award);
            }
        }

        // GET: TblMasAwards/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasAwards/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,AwardName,AwardDescription,AwardDueDate,LinkToTheAwardForm,AwardSubmissionEmailAddress")] TblMasAward tblMasAward)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Award");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblMasAward);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + ' ' + ex.InnerException);
                        return View(tblMasAward);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblMasAwards/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Award/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasAward? award;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    award = JsonConvert.DeserializeObject<TblMasAward>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    award = null;
                }
                return View(award);
            }
        }

        // POST: TblMasAwards/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,AwardName,AwardDescription,AwardDueDate,LinkToTheAwardForm,AwardSubmissionEmailAddress")] TblMasAward tblMasAward)
        {
            if (id != tblMasAward.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Award/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblMasAward);
                        var returnValue = await response.Content.ReadAsAsync<List<TblMasMember>>();
                        Log.Information("Update of Award ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Member ID " + id);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblMasAwards/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Award/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasAward? award;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    award = JsonConvert.DeserializeObject<TblMasAward>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    award = null;
                }
                return View(award);
            }
        }

        // POST: TblMasAwards/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Award/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblMasAward? award;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Member Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        award = JsonConvert.DeserializeObject<TblMasAward>(json);
                    }
                    else
                    {
                        Log.Information("Delete Member Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        award = null;
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

        //private bool TblMasAwardExists(int id)
        //{
        //    return _context.TblMasAwards.Any(e => e.Id == id);
        //}
    }
}
