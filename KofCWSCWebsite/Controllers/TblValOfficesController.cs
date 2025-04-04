﻿using System;
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
    public class TblValOfficesController : Controller
    {
        private DataSetService _dataSetService;

        public TblValOfficesController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblValOffices
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {

            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Offices");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblValOffice> offices;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblValOffice>>();
                    readTask.Wait();
                    offices = readTask.Result;
                }
                else
                {
                    offices = Enumerable.Empty<TblValOffice>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(offices);
            }
            //changed
            //return View(await _context.TblValOffices.OrderBy(x => x.OfficeDescription).ToListAsync());
        }

        // GET: TblValOffices/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
        {

            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Office/" + id.ToString());

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValOffice? office;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    office = JsonConvert.DeserializeObject<TblValOffice>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    office = null;
                }
                return View(office);
            }
            //var tblValOffice = await _context.TblValOffices
            //    .FirstOrDefaultAsync(m => m.OfficeId == id);
            //if (tblValOffice == null)
            //{
            //    return NotFound();
            //}

            //return View(tblValOffice);
        }

        // GET: TblValOffices/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("OfficeId,OfficeDescription,DirSortOrder,AltDescription,EmailAlias,UseAsFormalTitle,WebPageTagLine,SupremeUrl")] TblValOffice tblValOffice)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Office");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblValOffice);
                    try
                    {
                        response.EnsureSuccessStatusCode();
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + ' ' + ex.InnerException);
                    }
                }

                //_context.Add(tblValOffice);
                //await _context.SaveChangesAsync();
                //return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
            //return View(tblValOffice);
        }

        // GET: TblValOffices/Edit/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Office/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValOffice? office;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    office = JsonConvert.DeserializeObject<TblValOffice>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    office = null;
                }
                return View(office);
            }

            //var tblValOffice = await _context.TblValOffices.FindAsync(id);
            //if (tblValOffice == null)
            //{
            //    return NotFound();
            //}
            //return View(tblValOffice);
        }

        // POST: TblValOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("OfficeId,OfficeDescription,DirSortOrder,AltDescription,EmailAlias,UseAsFormalTitle,WebPageTagLine,SupremeUrl,GroupId,Copy2NewYear,ExchangeMailType")] TblValOffice tblValOffice)
        {
            if (id != tblValOffice.OfficeId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Office/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblValOffice);
                        var returnValue = await response.Content.ReadAsAsync<List<TblValOffice>>();
                        Log.Information("Update of Office ID " + id + "Returned " + returnValue);
                    }
                    //_context.Update(tblValOffice);
                    //await _context.SaveChangesAsync();
                }
                catch (Exception ex)
                
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Office ID " + id);
                //if (!TblValOfficeExists(tblValOffice.OfficeId))
                //{
                //    return NotFound();
                //}
                //else
                //{
                //    throw;
                //}
            }
            return RedirectToAction(nameof(Index));
             //return View(tblValOffice);
        }

        // GET: TblValOffices/Delete/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Office/" + id);
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblValOffice? office;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    office = JsonConvert.DeserializeObject<TblValOffice>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    office = null;
                }
                return View(office);
            }

            //var tblValOffice = await _context.TblValOffices
            //    .FirstOrDefaultAsync(m => m.OfficeId == id);
            //if (tblValOffice == null)
            //{
            //    return NotFound();
            //}

            //return View(tblValOffice);
        }

        // POST: TblValOffices/Delete/5
        [Authorize(Roles = "Admin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Office/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblValOffice? office;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Office Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        office = JsonConvert.DeserializeObject<TblValOffice>(json);
                        return RedirectToAction(nameof(Index));
                    }
                    else
                    {
                        //**************************************************************************
                        // 09/26/2024 Tim Philomeno
                        // not sure if this is the way to handle errors but I know in the API if we
                        // have an FK error, I am expliciting setting the return code to a 450 so
                        // we can give feedback to the user
                        //**************************************************************************
                        if (result.StatusCode.ToString() == "450")
                        {
                            Log.Information("Delete Failed because of FK Constraint " + id);
                            ViewData["DelError"] = "***Delete failed bacause this office is in use by another Member";
                            ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                            return View();
                        }
                        else
                        {
                            Log.Information("Delete failed, reason is unknown " + id);
                            ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                            return View();
                        }

                    }
                    
                }
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message + " " + ex.InnerException);
                return NoContent();
            }
            //var tblValOffice = await _context.TblValOffices.FindAsync(id);
            //if (tblValOffice != null)
            //{
            //    _context.TblValOffices.Remove(tblValOffice);
            //}

            //await _context.SaveChangesAsync();
            //return RedirectToAction(nameof(Index));
        }

        //private bool TblValOfficeExists(int id)
        //{
        //    return _context.TblValOffices.Any(e => e.OfficeId == id);
        //}
    }
}
