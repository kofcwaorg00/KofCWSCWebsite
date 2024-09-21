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

namespace KofCWSCWebsite.Controllers
{
    public class FileStorageController : Controller
    {
        private readonly ApplicationDbContext _context;

        public FileStorageController(ApplicationDbContext context)
        {
            _context = context;
        }

        [Authorize(Roles ="Admin")]
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





        // GET: TblWebFileStorages
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Index()
        {
            // 9/21/2024 Tim Philomeno
            // execute a select statement so we don't get the DATA stream, takes too long
            //return  _context.Database
            //  .SqlQuery<FileStorageVM>($"SELECT Id,Length,ContentType,FileName FROM tblWEB_FileStorage")
            //.ToList();
            //return  _context.Set<FileStorageVM>($"SELECT Id,Length,ContentType,FileName FROM tblWEB_FileStorage").ToList();
                var result = _context.Database
                    .SqlQuery<FileStorageVM>($"uspWEB_GetFileStorageVM")
                    .ToList();

                return View(result);
        }

        // GET: TblWebFileStorages/Details/5
        [Authorize(Roles = "Admin")]
        public async Task<IActionResult> Details(int? id)
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

        //// GET: TblWebFileStorages/Create
        [Authorize(Roles ="Admin")]
        public IActionResult Create()
        {
            return View();
        }

        //// POST: TblWebFileStorages/Create
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Create([Bind("Id,FileName,Data,Length,ContentType")] FileStorage tblWebFileStorage)
        //{
        //    if (ModelState.IsValid)
        //    {
        //        _context.Add(tblWebFileStorage);
        //        await _context.SaveChangesAsync();
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(tblWebFileStorage);
        //}

        //// GET: TblWebFileStorages/Edit/5
        //public async Task<IActionResult> Edit(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var tblWebFileStorage = await _context.FileStorages.FindAsync(id);
        //    if (tblWebFileStorage == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(tblWebFileStorage);
        //}

        //// POST: TblWebFileStorages/Edit/5
        //// To protect from overposting attacks, enable the specific properties you want to bind to.
        //// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //[HttpPost]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,Length,Data,ContentType")] FileStorage tblWebFileStorage)
        //{
        //    if (id != tblWebFileStorage.Id)
        //    {
        //        return NotFound();
        //    }
            
        //    if (ModelState.IsValid)
        //    {
        //        try
        //        {
        //            _context.Update(tblWebFileStorage);
        //            await _context.SaveChangesAsync();
        //        }
        //        catch (DbUpdateConcurrencyException)
        //        {
        //            if (!TblWebFileStorageExists(tblWebFileStorage.Id))
        //            {
        //                return NotFound();
        //            }
        //            else
        //            {
        //                throw;
        //            }
        //        }
        //        return RedirectToAction(nameof(Index));
        //    }
        //    return View(tblWebFileStorage);
        //}

        //// GET: TblWebFileStorages/Delete/5
        //public async Task<IActionResult> Delete(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var tblWebFileStorage = await _context.FileStorages
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (tblWebFileStorage == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(tblWebFileStorage);
        //}

        //// POST: TblWebFileStorages/Delete/5
        //[HttpPost, ActionName("Delete")]
        //[ValidateAntiForgeryToken]
        //public async Task<IActionResult> DeleteConfirmed(int id)
        //{
        //    var tblWebFileStorage = await _context.FileStorages.FindAsync(id);
        //    if (tblWebFileStorage != null)
        //    {
        //        _context.FileStorages.Remove(tblWebFileStorage);
        //    }

        //    await _context.SaveChangesAsync();
        //    return RedirectToAction(nameof(Index));
        //}

        //private bool TblWebFileStorageExists(int id)
        //{
        //    return _context.    FileStorages.Any(e => e.Id == id);
        //}
    }
}
