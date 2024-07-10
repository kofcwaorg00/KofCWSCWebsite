using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Data;

namespace KofCWSCWebsite.Controllers
{
    public class TblCorrMemberOfficesController : Controller
    {
        private readonly ApplicationDbContext _context;

        public TblCorrMemberOfficesController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: TblCorrMemberOffices
        public async Task<ActionResult<IEnumerable<TblCorrMemberOfficeVM>>> Index(int id)
        {
            try
            {
                // Run Sproc
                var CMO = _context.Database
                    .SqlQuery<TblCorrMemberOfficeVM>($"EXECUTE uspSYS_GetOfficesForMemberID {id}")
                    .ToList();

                //var CMO = await _context.TblCorrMemberOffices
                //    .Where(x => x.MemberId == id)
                //    .ToListAsync();
                //var member = await _context.TblMasMembers.FindAsync(id);

                ViewBag.MemberID = id;
                ViewBag.MemberName = _context.funSYS_BuildName.FromSqlInterpolated($"SELECT dbo.funSYS_BuildName({id},0,'N') as MemberName").FirstOrDefault().MemberName;

                //ViewBag.MemberName = member.FirstName + " " + member.LastName;

                return View(CMO);
            }
            catch (Exception ex)
            {

                throw new Exception(ex.Message);
            }
        }

        // GET: TblCorrMemberOffices/Details/5
        public async Task<ActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCorrMemberOffice = await _context.TblCorrMemberOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            //var CMO = _context.Database
            //        .SqlQuery<TblCorrMemberOfficeVM>($"EXECUTE uspSYS_GetOfficesForMemberID {id}")
            //        .ToList();

            if (tblCorrMemberOffice == null)
            {
                return NotFound();
            }

            return View(tblCorrMemberOffice);
        }

        // GET: TblCorrMemberOffices/Create
        public IActionResult Create(int id)
        {
            ViewBag.MemberID = id;
            ViewBag.ListOfOffices = new SelectList(_context.TblValOffices.OrderBy(x => x.OfficeDescription).ToList(), "OfficeId", "OfficeDescription");
            return View();
        }

        // POST: TblCorrMemberOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,OfficeId,PrimaryOffice,Year")] TblCorrMemberOffice tblCorrMemberOffice)
        {
            if (ModelState.IsValid)
            {
                _context.Add(tblCorrMemberOffice);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index),new { id = tblCorrMemberOffice.MemberId });
            }
            return View(tblCorrMemberOffice);
        }

        // GET: TblCorrMemberOffices/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
           
            List<TblValOffice> officeList = new List<TblValOffice>();
            //officeList = (from OfficeID,OfficeDescription in _context.TblValOffices select OfficeID,OfficeDescription).ToList();
            //officeList = (from offices in _context.TblValOffices select offices).ToList();
            //var officeList = _context.TblValOffices.ToList(); ;
            
            //officeList.Insert(0, new TblValOffice { OfficeId = 0, OfficeDescription = "Select" });
            

            ViewBag.ListOfOffices =  new SelectList(_context.TblValOffices.ToList(), "OfficeId", "OfficeDescription");
            
                       

            var tblCorrMemberOffice = await _context.TblCorrMemberOffices.FindAsync(id);
            if (tblCorrMemberOffice == null)
            {
                return NotFound();
            }
            return View(tblCorrMemberOffice);
        }

        // POST: TblCorrMemberOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,OfficeId,PrimaryOffice,Year")] TblCorrMemberOffice tblCorrMemberOffice)
        {
            if (id != tblCorrMemberOffice.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(tblCorrMemberOffice);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblCorrMemberOfficeExists(tblCorrMemberOffice.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { id = tblCorrMemberOffice.MemberId });
            }
            return View(tblCorrMemberOffice);
        }

        // GET: TblCorrMemberOffices/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var tblCorrMemberOffice = await _context.TblCorrMemberOffices
                .FirstOrDefaultAsync(m => m.Id == id);
            if (tblCorrMemberOffice == null)
            {
                return NotFound();
            }

            return View(tblCorrMemberOffice);
        }

        // POST: TblCorrMemberOffices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var tblCorrMemberOffice = await _context.TblCorrMemberOffices.FindAsync(id);
            if (tblCorrMemberOffice != null)
            {
                _context.TblCorrMemberOffices.Remove(tblCorrMemberOffice);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = tblCorrMemberOffice.MemberId });
        }

        private bool TblCorrMemberOfficeExists(int id)
        {
            return _context.TblCorrMemberOffices.Any(e => e.Id == id);
        }
    }
}
