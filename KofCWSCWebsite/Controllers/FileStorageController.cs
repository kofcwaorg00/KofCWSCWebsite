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
using NuGet.Protocol;
using java.net;
using Newtonsoft.Json;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class FileStorageController : Controller
    {
        private DataSetService _dataSetService;

        public FileStorageController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        [Authorize(Roles = "Admin")]
        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileModel = new FileStorage
                    {
                        FileName = file.FileName,
                        Data = memoryStream.ToArray(),
                        Length = file.Length,
                        ContentType = file.ContentType
                    };
                    if (ModelState.IsValid)
                    {
                        Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Files");
                        using (var client = new HttpClient())
                        {
                            client.BaseAddress = myURI;
                            var response = await client.PostAsJsonAsync(myURI, fileModel);
                        }

                    }
                }
            }
            return RedirectToAction("Index");
        }

        // not sure if this is going to be used
        //public async Task<IActionResult> GetFiles()
        //{
        //    var result = _context.Database
        //        .SqlQuery<FileStorageVM>($"uspWEB_GetFileStorageVM")
        //        .ToList();

        //    return View("ShowPDF",result);
        //}



        // GET: TblWebFileStorages
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            //*****************************************************************************
            // 9/21/2024 Tim Philomeno
            // execute a select statement so we don't get the DATA stream, takes too long
            //-----------------------------------------------------------------------------
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Files");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<FileStorageVM> FSOs;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<FileStorageVM>>();
                    readTask.Wait();
                    FSOs = readTask.Result;
                }
                else
                {
                    FSOs = Enumerable.Empty<FileStorageVM>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(FSOs);
            }
        }

        // GET: TblWebFileStorages/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return null;
            }
            //********************************************************************************
            // 11/1/2024 Tim Philomeno
            // create a session cookie, the MinValue does the trick
            HttpContext.Response.Cookies.Append("IAgreeSensitive", "true", new CookieOptions
            {
                //Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/",
                HttpOnly = false, // Accessible only by the server
                IsEssential = true // Required for GDPR compliance
            });
            //----------------------------------------------------------------------------------
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Files/" + id.ToString());

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                FileStorage? FSO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    FSO = JsonConvert.DeserializeObject<FileStorage>(json);
                }
                else
                {
                    FSO = null;
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("ShowPDF", FSO);
            }
        }

        //// GET: TblWebFileStorages/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        //*****************************************************************************
        // 11/7/2024 Tim Philomeno
        // GetPDF supports the Ajax call in FileStorage Details
        // Security is set to only authenicated users [Authorize]
        [Authorize]
        [HttpPost]
        public async Task<JsonResult> GetPDF(int? id)
        {
            if (id == null)
            {
                return Json(NotFound());
            }
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Files/" + id.ToString());
            FileStorage? FSO;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    FSO = JsonConvert.DeserializeObject<FileStorage>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    FSO = null;
                }

            }
            return Json(new { FileName = FSO.FileName, ContentType = FSO.ContentType, Data = FSO.Data });
        }

        // GET: TblWebFileStorages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Files/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                FileStorage? FSO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    FSO = JsonConvert.DeserializeObject<FileStorage>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    FSO = null;
                }
                return View(FSO);
            }
        }

        // POST: TblWebFileStorages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Files/" + id);
            try
            {
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    FileStorage? FSO;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete File Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        FSO = JsonConvert.DeserializeObject<FileStorage>(json);
                    }
                    else
                    {
                        Log.Information("Delete File Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        FSO = null;
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
    }
}
