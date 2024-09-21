using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;

namespace KofCWSCWebsite.Controllers
{
    public class TblWebFileStoragesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblWebFileStoragesController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost]
        public async Task<IActionResult> Upload(IFormFile file)
        {
            if (file != null && file.Length > 0)
            {
                using (var memoryStream = new MemoryStream())
                {
                    await file.CopyToAsync(memoryStream);
                    var fileModel = new TblWebFileStorage
                    {
                        FileName = file.FileName,
                        Data = memoryStream.ToArray(),
                        Length = file.Length,
                        ContentType = file.ContentType
                    };
                    _context.TblWebFileStorages.Add(fileModel);
                    await _context.SaveChangesAsync();
                }
            }
            return RedirectToAction("Index");
        }





        // GET: TblWebFileStorages
        public async Task<IActionResult> Index()
        {
            return View(await _context.TblWebFileStorages.ToListAsync());
        }

        // GET: TblWebFileStorages/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebFileStorage = await _context.TblWebFileStorages
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblWebFileStorage == null)
            {
                return NotFound();
            }

            return View(tblWebFileStorage);
        }

        // GET: TblWebFileStorages/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblWebFileStorages/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,FileName,Data,Length,ContentType")] TblWebFileStorage tblWebFileStorage)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblWebFileStorage);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(tblWebFileStorage);
        }

        // GET: TblWebFileStorages/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebFileStorage = await _context.TblWebFileStorages.FindAsync(id);
            if (tblWebFileStorage == null)
            {
                return NotFound();
            }
            return View(tblWebFileStorage);
        }

        // POST: TblWebFileStorages/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FileName,Data,Length,ContentType")] TblWebFileStorage tblWebFileStorage)
        {
            if (id != tblWebFileStorage.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblWebFileStorage);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblWebFileStorageExists(tblWebFileStorage.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(tblWebFileStorage);
        }

        // GET: TblWebFileStorages/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblWebFileStorage = await _context.TblWebFileStorages
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
            var tblWebFileStorage = await _context.TblWebFileStorages.FindAsync(id);
            if (tblWebFileStorage != null)
            {
                _context.TblWebFileStorages.Remove(tblWebFileStorage);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool TblWebFileStorageExists(int id)
        {
            return _context.TblWebFileStorages.Any(e => e.Id == id);
        }
    }
}
