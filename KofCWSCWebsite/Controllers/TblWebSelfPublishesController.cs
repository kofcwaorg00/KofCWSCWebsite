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

namespace KofCWSCWebsite.Controllers
{
    public class TblWebSelfPublishesController : Controller
    {
        private DataSetService _dataSetService;

        public TblWebSelfPublishesController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: TblWebSelfPublishes
        public async Task<IActionResult> Index()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/SelfPubs");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<TblWebSelfPublish> selfpubs;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblWebSelfPublish>>();
                    readTask.Wait();
                    selfpubs = readTask.Result;
                }
                else
                {
                    selfpubs = Enumerable.Empty<TblWebSelfPublish>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(selfpubs);
            }
        }

        // GET: TblWebSelfPublishes/Details/5
        public async Task<IActionResult> Details(string id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebSelfPublish? selfpub;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    selfpub = JsonConvert.DeserializeObject<TblWebSelfPublish>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    selfpub = null;
                }
                return View(selfpub);
            }
        }

        /// <summary>
        /// changed
        /// Added Display to support the "Read More.." on in the carosel.  TblWebSelfPublishs does not provide additional data that 
        /// is used in the display of the message
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        public async Task<IActionResult> DisplayAsync(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/Display/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var myresult = responseTask.Result;
                IEnumerable<SPGetSOS> selfpub;
                if (myresult.IsSuccessStatusCode)
                {
                    var readTask = myresult.Content.ReadAsAsync<IList<SPGetSOS>>();
                    readTask.Wait();
                    selfpub = readTask.Result;
                }
                else
                {
                    selfpub = Enumerable.Empty<SPGetSOS>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(selfpub);
            }
        }

        // GET: TblWebSelfPublishes/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblWebSelfPublishes/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Url,Data")] TblWebSelfPublish tblWebSelfPublish)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub");
                using (var client = new HttpClient())
                {
                    client.BaseAddress = myURI;
                    var response = await client.PostAsJsonAsync(myURI, tblWebSelfPublish);
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

        // GET: TblWebSelfPublishes/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null || id == "favicon.ico")
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/str/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebSelfPublish? selfpub;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    selfpub = JsonConvert.DeserializeObject<TblWebSelfPublish>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    selfpub = null;
                }
                return View(selfpub);
            }
        }

        public async Task<IActionResult> EditInt(int id)
        {
            //if (id == null || id == "favicon.ico")
            //{
            //    return NotFound();
            //}
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/int/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebSelfPublish? selfpub;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    selfpub = JsonConvert.DeserializeObject<TblWebSelfPublish>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    selfpub = null;
                }
                return View("Edit",selfpub);
            }
        }

        // POST: TblWebSelfPublishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> EditInt(int id, [Bind("Url,Data,OID")] TblWebSelfPublish tblWebSelfPublish)
        {
            // since we are using Url as the unique id, id may not be sent here but OID will always be correct
            //if (id != tblWebSelfPublish.OID)
            //{
            //    return NotFound();
            //}
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + $"/SelfPubInt/{tblWebSelfPublish.OID}" );
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblWebSelfPublish);
                        var returnValue = await response.Content.ReadAsAsync<List<TblWebSelfPublish>>();
                        Log.Information("Update of Message ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Message ID " + id);
            }
            //return RedirectToAction(nameof(Index));
            return View("Edit", tblWebSelfPublish);
        }



        // POST: TblWebSelfPublishes/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Url,Data,OID")] TblWebSelfPublish tblWebSelfPublish)
        {
            if (id != tblWebSelfPublish.Url)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblWebSelfPublish);
                        var returnValue = await response.Content.ReadAsAsync<List<TblWebSelfPublish>>();
                        Log.Information("Update of Message ID " + id + "Returned " + returnValue);
                    }
                }
                catch (Exception ex)
                {
                    Log.Fatal(ex.Message);
                }
                Log.Information("Update Success Message ID " + id);
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblWebSelfPublishes/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblWebSelfPublish? selfpub;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    selfpub = JsonConvert.DeserializeObject<TblWebSelfPublish>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    selfpub = null;
                }
                return View(selfpub);
            }
        }

        // POST: TblWebSelfPublishes/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/SelfPub/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblWebSelfPublish? assy;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete Message Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        assy = JsonConvert.DeserializeObject<TblWebSelfPublish>(json);
                    }
                    else
                    {
                        Log.Information("Delete Message Failed " + id);
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

        //private bool TblWebSelfPublishExists(string id)
        //{
        //    return _context.TblWebSelfPublishes.Any(e => e.Url == id);
        //}
    }
}
