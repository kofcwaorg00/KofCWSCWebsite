using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using System.Text.Json;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Serilog;
using Microsoft.AspNetCore.Authorization;

namespace KofCWSCWebsite.Controllers
{
    public class TblSysTrxEventsController : Controller
    {
        private DataSetService _dataSetService;

        public TblSysTrxEventsController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblSysTrxEvents
        [Authorize(Roles = "Admin,CalAdmin")]
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Events");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblSysTrxEvent> events;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblSysTrxEvent>>();
                    readTask.Wait();
                    events = readTask.Result;
                }
                else
                {
                    events = Enumerable.Empty<TblSysTrxEvent>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(events);
            }
        }
        [Authorize]
        public IActionResult GetCalendarEvents(string start, string end)
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Events/" + start + "/" + end );

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblSysTrxEvent> calevents;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblSysTrxEvent>>();
                    readTask.Wait();
                    calevents = readTask.Result;
                }
                else
                {
                    calevents = Enumerable.Empty<TblSysTrxEvent>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(calevents);
            }
        }

        // GET: TblSysTrxEvents/Details/5
        [Authorize(Roles = "Admin,CalAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event/" + id);
            TblSysTrxEvent? myevent;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    myevent = JsonConvert.DeserializeObject<TblSysTrxEvent>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    myevent = null;
                }
                return View(myevent);
            }
        }

        // GET: TblSysTrxEvents/Create
        [Authorize(Roles = "Admin,CalAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblSysTrxEvents/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,CalAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Description,Begin,End,isAllDay,AttachUrl,AddedBy,DateAdded")] TblSysTrxEvent tblSysTrxEvent)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblSysTrxEvent);
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

        // GET: TblSysTrxEvents/Edit/5
        [Authorize(Roles = "Admin,CalAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblSysTrxEvent? edevent;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    edevent = JsonConvert.DeserializeObject<TblSysTrxEvent>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    edevent = null;
                }
                return View(edevent);
            }
        }

        // POST: TblSysTrxEvents/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,CalAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Description,Begin,End,isAllDay,AttachUrl,AddedBy,DateAdded")] TblSysTrxEvent tblSysTrxEvent)
        {
            if (id != tblSysTrxEvent.Id)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event/" + id.ToString());
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblSysTrxEvent);
                        var returnValue = await response.Content.ReadAsAsync<List<TblSysTrxEvent>>();
                        Log.Information("Update of Event ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Event ID " + id);
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblSysTrxEvents/Delete/5
        [Authorize(Roles = "Admin,CalAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblSysTrxEvent? devent;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    devent = JsonConvert.DeserializeObject<TblSysTrxEvent>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    devent = null;
                }
                return View(devent);
            }
        }

        // POST: TblSysTrxEvents/Delete/5
        [Authorize(Roles = "Admin,CalAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Event/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblSysTrxEvent? devent;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Event Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        devent = JsonConvert.DeserializeObject<TblSysTrxEvent>(json);
                    }
                    else
                    {
                        Log.Information("Delete Event Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        devent = null;
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

        //////private bool TblSysTrxEventExists(int id)
        //////{
        //////    return _context.TblSysTrxEvents.Any(e => e.Id == id);
        //////}
    }
}
