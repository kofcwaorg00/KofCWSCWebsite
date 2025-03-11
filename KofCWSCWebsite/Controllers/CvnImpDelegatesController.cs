using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using com.sun.xml.@internal.bind.v2.model.core;

namespace KofCWSCWebsite.Controllers
{
    public class CvnImpDelegatesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public CvnImpDelegatesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: CvnImpDelegates
        public async Task<ActionResult<IEnumerable<CvnImpDelegate>>> Index()
        {
            ViewBag.Message = TempData["Message"];
            return View(await _context.Database
                    .SqlQuery<CvnImpDelegate>($"EXECUTE uspCVN_GetImpDelegates")
                    .ToListAsync());
            //return View(await _context.CvnImpDelegates.ToListAsync());
        }

        // GET: CvnImpDelegates/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegate = await _context.CvnImpDelegates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cvnImpDelegate == null)
            {
                return NotFound();
            }

            return View(cvnImpDelegate);
        }

        // GET: CvnImpDelegates/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: CvnImpDelegates/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubmissionDate,FormSubmitterSEmail,CouncilName,CouncilNumber,D1FirstName,D1MiddleName,D1LastName,D1Suffix,D1MemberID,D1Address1,D1Address2,D1City,D1State,D1ZipCode,D1Phone,D1Email,D2FirstName,D2MiddleName,D2LastName,D2Suffix,D2MemberID,D2Address1,D2Address2,D2City,D2State,D2ZipCode,D2Phone,D2Email,A1FirstName,A1MiddleName,A1LastName,A1Suffix,A1MemberID,A1Address1,A1Address2,A1City,A1State,A1ZipCode,A1Phone,A1Email,A2FirstName,A2MiddleName,A2LastName,A2Suffix,A2MemberID,A2Address1,A2Address2,A2City,A2State,A2ZipCode,A2Phone,A2Email,Id,RecType")] CvnImpDelegate cvnImpDelegate)
        {
            if (ModelState.IsValid)
            {
                _context.Add(cvnImpDelegate);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(cvnImpDelegate);
        }

        // GET: CvnImpDelegates/Edit/5
        [HttpGet("Edit/{id}/{validate}")]
        public async Task<IActionResult> Edit(int? id,string? validate)
        {
            ViewBag.Validate = validate;
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegate = await _context.CvnImpDelegates.FindAsync(id);
            if (cvnImpDelegate == null)
            {
                return NotFound();
            }
            return View(cvnImpDelegate);
        }

        // POST: CvnImpDelegates/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("SubmissionDate,FormSubmitterSEmail,CouncilName,CouncilNumber,D1FirstName,D1MiddleName,D1LastName,D1Suffix,D1MemberID,D1Address1,D1Address2,D1City,D1State,D1ZipCode,D1Phone,D1Email,D2FirstName,D2MiddleName,D2LastName,D2Suffix,D2MemberID,D2Address1,D2Address2,D2City,D2State,D2ZipCode,D2Phone,D2Email,A1FirstName,A1MiddleName,A1LastName,A1Suffix,A1MemberID,A1Address1,A1Address2,A1City,A1State,A1ZipCode,A1Phone,A1Email,A2FirstName,A2MiddleName,A2LastName,A2Suffix,A2MemberID,A2Address1,A2Address2,A2City,A2State,A2ZipCode,A2Phone,A2Email,Id,RecType")] CvnImpDelegate cvnImpDelegate)
        {
            if (id != cvnImpDelegate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(cvnImpDelegate);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!CvnImpDelegateExists(cvnImpDelegate.Id))
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
            return View(cvnImpDelegate);
        }

        // GET: CvnImpDelegates/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var cvnImpDelegate = await _context.CvnImpDelegates
                .FirstOrDefaultAsync(m => m.Id == id);
            if (cvnImpDelegate == null)
            {
                return NotFound();
            }

            return View(cvnImpDelegate);
        }

        // POST: CvnImpDelegates/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var cvnImpDelegate = await _context.CvnImpDelegates.FindAsync(id);
            if (cvnImpDelegate != null)
            {
                _context.CvnImpDelegates.Remove(cvnImpDelegate);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        private bool CvnImpDelegateExists(int id)
        {
            return _context.CvnImpDelegates.Any(e => e.Id == id);
        }
    }
}
