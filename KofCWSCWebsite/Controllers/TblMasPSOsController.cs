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
    public class TblMasPSOsController : Controller
    {
        private DataSetService _dataSetService;

        public TblMasPSOsController(ApplicationDbContext context,DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblMasPSOs
        // everyone can see PSO
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/PSOs");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblMasPso> PSOs;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblMasPso>>();
                    readTask.Wait();
                    PSOs = readTask.Result;
                }
                else
                {
                    PSOs = Enumerable.Empty<TblMasPso>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(PSOs);
            }
        }

        // GET: TblMasPSOs/Details/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/PSO/" + id.ToString());

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasPso? PSO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    PSO = JsonConvert.DeserializeObject<TblMasPso>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    PSO = null;
                }
                return View(PSO);
            }
        }

        // GET: TblMasPSOs/Create
        [Authorize(Roles = "Admin,DataAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasPSOs/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Year,StateDeputy,StateSecretary,StateTreasurer,StateAdvocate,StateWarden")] TblMasPso tblMasPso)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/PSO");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblMasPso);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + ' ' + ex.InnerException);
                        return View(tblMasPso);
                    }
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblMasPSOs/Edit/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/PSO/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasPso? PSO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    PSO = JsonConvert.DeserializeObject<TblMasPso>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    PSO = null;
                }
                return View(PSO);
            }
        }

        // POST: TblMasPSOs/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Year,StateDeputy,StateSecretary,StateTreasurer,StateAdvocate,StateWarden")] TblMasPso tblMasPso)
        {
            if (id != tblMasPso.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/PSO/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblMasPso);
                        var returnValue = await response.Content.ReadAsAsync<List<TblMasPso>>();
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

        // GET: TblMasPSOs/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/PSO/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasPso? PSO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    PSO = JsonConvert.DeserializeObject<TblMasPso>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    PSO = null;
                }
                return View(PSO);
            }
        }

        // POST: TblMasPSOs/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/PSO/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblMasPso? PSO;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Member Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        PSO = JsonConvert.DeserializeObject<TblMasPso>(json);
                    }
                    else
                    {
                        Log.Information("Delete Member Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        PSO = null;
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

        //private bool TblMasPsoExists(int id)
        //{
        //    return _context.TblMasPsos.Any(e => e.Id == id);
        //}
    }
}
