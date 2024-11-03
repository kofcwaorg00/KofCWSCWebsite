using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using com.sun.tools.javac.util;
using com.sun.tools.@internal.ws.processor.model;
using Microsoft.AspNetCore.Authorization;
using NuGet.Protocol;
using java.net;

namespace KofCWSCWebsite.Controllers
{
    public class FileStorageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileStorageController(ApplicationDbContext context)
        {
            _context = context;
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
                    _context.FileStorages.Add(fileModel);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }

        // not sure if this is going to be used
        public async Task<IActionResult> GetFiles()
        {
            var result = _context.Database
                .SqlQuery<FileStorageVM>($"uspWEB_GetFileStorageVM")
                .ToList();

            return View("ShowPDF",result);
        }



        // GET: TblWebFileStorages
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            //*****************************************************************************
            // 9/21/2024 Tim Philomeno
            // execute a select statement so we don't get the DATA stream, takes too long
            //-----------------------------------------------------------------------------
            var result = _context.Database
                .SqlQuery<FileStorageVM>($"uspWEB_GetFileStorageVM")
                .ToList();

            return View(result);
        }

        // GET: TblWebFileStorages/Details/5
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //********************************************************************************
            // 11/1/2024 Tim Philomeno
            // create a session cookie, the MinValue does the trick
            HttpContext.Response.Cookies.Append("IAgreeSensitive", "true", new CookieOptions
            {
                //Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path="/",
                HttpOnly = false, // Accessible only by the server
                IsEssential = true // Required for GDPR compliance
            });
            //----------------------------------------------------------------------------------
            var tblWebFileStorage = await _context.FileStorages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblWebFileStorage == null)
            {
                return NotFound();
            }
            return View("ShowPDF", tblWebFileStorage);
            
        }

        //// GET: TblWebFileStorages/Create
        [Authorize(Roles = "Admin")]
        public IActionResult Create()
        {
            return View();
        }

        // GET: TblWebFileStorages/Details/5
        [Authorize]
        [HttpPost]
        public JsonResult GetPDF(int? id)
        {
            if (id == null)
            {
                return Json(NotFound());
            }
            var tblWebFileStorage = _context.Database.SqlQuery<FileStorage>($"SELECT * FROM tblWEB_FileStorage where Id={id}").ToList();
            var tfs = tblWebFileStorage.FirstOrDefault();
            if (tblWebFileStorage == null)
            {
                return Json(NotFound());
            }

            return Json(new { FileName = tfs.FileName, ContentType = tfs.ContentType, Data = tfs.Data });
        }

        // GET: TblWebFileStorages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebFileStorage = await _context.FileStorages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblWebFileStorage == null)
            {
                return NotFound();
            }

            return View(tblWebFileStorage);
        }

        // POST: TblWebFileStorages/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblWebFileStorage = await _context.FileStorages.FindAsync(id);
            if (tblWebFileStorage != null)
            {
                _context.FileStorages.Remove(tblWebFileStorage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        //private bool TblWebFileStorageExists(int id)
        //{
        //    return _context.    FileStorages.Any(e => e.Id == id);
        //}
    }
}
