using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class MemberSuspensionsController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;

        public MemberSuspensionsController(ApplicationDbContext context,DataSetService dataSetService)
        {
            //_context = context;
            _dataSetService = dataSetService;
            _apiHelper = new ApiHelper(_dataSetService);
        }

        // GET: MemberSuspensions
        public async Task<ActionResult<IEnumerable<MemberSuspensionVM>>> Index()
        {
            return View(await _apiHelper.GetAsync<IEnumerable<MemberSuspensionVM>>("/Suspensions"));

            //return View(await _context.Database.SqlQuery<MemberSuspensionVM>($"uspSYS_GetSuspendedMembers").ToListAsync());
            //return View(await _context.TblSysMasMemberSuspensions.ToListAsync());
        }

        // GET: MemberSuspensions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var memberSuspension = await _apiHelper.GetAsync<MemberSuspension>($"/Suspensions/{id}");
            //var memberSuspension = await _context.TblSysMasMemberSuspensions
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (memberSuspension == null)
            {
                return NotFound();
            }

            return View(memberSuspension);
        }

        // GET: MemberSuspensions/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: MemberSuspensions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> CreateFrom([Bind("Id,KofCid,Comment")] MemberSuspension memberSuspension)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiHelper.PostAsync<MemberSuspension, MemberSuspension>($"/Suspensions/CreateFrom", memberSuspension);
                    //_context.Add(memberSuspension);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return RedirectToAction(nameof(Index));

            }
            catch (Exception)
            {
                ModelState.AddModelError("", "Error, probably a duplicate entry");
                return RedirectToAction(nameof(Index));
            }
        }


        // POST: MemberSuspensions/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,KofCid,Comment")] MemberSuspension memberSuspension)
        {
            try
            {
                if (ModelState.IsValid)
                {
                    await _apiHelper.PostAsync<MemberSuspension, MemberSuspension>($"/Suspensions", memberSuspension);
                    //_context.Add(memberSuspension);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                return View(memberSuspension);
            }
            catch (Exception)
            {
                ModelState.AddModelError("","Member Does Not exist.");
                return View();
            }

        }

        // GET: MemberSuspensions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var memberSuspension = await _apiHelper.GetAsync<MemberSuspension>($"/Suspensions/{id}");
            //var memberSuspension = await _context.TblSysMasMemberSuspensions.FindAsync(id);
            if (memberSuspension == null)
            {
                return NotFound();
            }
            return View(memberSuspension);
        }

        // POST: MemberSuspensions/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,KofCid,Comment")] MemberSuspension memberSuspension)
        {
            if (id != memberSuspension.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _apiHelper.PutAsync<MemberSuspension, MemberSuspension>($"/Suspensions/{id}", memberSuspension);
                    //_context.Update(memberSuspension);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!MemberSuspensionExists(memberSuspension.Id))
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
            return View(memberSuspension);
        }

        // GET: MemberSuspensions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var memberSuspension = await _apiHelper.GetAsync<MemberSuspension>($"/Suspensions/{id}");
            //var memberSuspension = await _context.TblSysMasMemberSuspensions
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (memberSuspension == null)
            {
                return NotFound();
            }

            return View(memberSuspension);
        }

        // POST: MemberSuspensions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiHelper.DeleteAsync($"/Suspensions/{id}");
                return RedirectToAction(nameof(Index));

            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
        }

        private bool MemberSuspensionExists(int id)
        {
            var result = _apiHelper.GetAsync<MemberSuspension>($"/Suspension/{id}");
            if (result != null)
            {
                return true;
            }
            else
            {
                return false;
            }
            //return _context.TblSysMasMemberSuspensions.Any(e => e.Id == id);
        }
    }
}
