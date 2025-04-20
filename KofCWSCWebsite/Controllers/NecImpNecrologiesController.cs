using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using System.Formats.Asn1;
using javax.print.attribute.standard;
using Microsoft.AspNetCore.Authorization;

namespace KofCWSCWebsite.Controllers
{
    public class NecImpNecrologiesController : Controller
    {
        private ApiHelper _apiHelper;

        public NecImpNecrologiesController(ApplicationDbContext context, ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        // GET: NecImpNecrologies
        public async Task<IActionResult> Index()
        {
            var results = await _apiHelper.GetAsync<List<NecImpNecrologyVM>>("Necrology");
            return View(results);
        }

        [HttpGet("UpdateDeceased/{id}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> UpdateDeceased(int id)
        {
            var myAffectedRows = await _apiHelper.GetAsync<int>($"/UpdateDeceased/{id}");
            return RedirectToAction("Index");

        }




        //////////// GET: NecImpNecrologies/Details/5
        //////////public async Task<IActionResult> Details(int? id)
        //////////{
        //////////    if (id == null)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    var necImpNecrology = await _context.NecImpNecrology
        //////////        .FirstOrDefaultAsync(m => m.Id == id);
        //////////    if (necImpNecrology == null)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    return View(necImpNecrology);
        //////////}

        //////////// GET: NecImpNecrologies/Create
        //////////public IActionResult Create()
        //////////{
        //////////    return View();
        //////////}

        //////////// POST: NecImpNecrologies/Create
        //////////// To protect from overposting attacks, enable the specific properties you want to bind to.
        //////////// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //////////[HttpPost]
        //////////[ValidateAntiForgeryToken]
        //////////public async Task<IActionResult> Create([Bind("SubDate,SubFname,SubLname,SubEmail,SubRole,CouncilId,DecPrefix,DecFname,DecMname,DecLname,DecSuffix,DecFmorKn,Fmprefix,Fmfname,Fmmname,Fmlname,Fmsuffix,Relationship,Dod,DecMemberId,MemberType,DecOfficesHeld,AssemblyId,Nokprefix,Nokfname,Nokmname,Noklname,Noksuffix,Nokrelate,Nokaddress1,Nokaddress2,Nokcity,Nokstate,Nokzip,Nokcountry,Comments,Id")] NecImpNecrology necImpNecrology)
        //////////{
        //////////    if (ModelState.IsValid)
        //////////    {
        //////////        _context.Add(necImpNecrology);
        //////////        await _context.SaveChangesAsync();
        //////////        return RedirectToAction(nameof(Index));
        //////////    }
        //////////    return View(necImpNecrology);
        //////////}

        //////////// GET: NecImpNecrologies/Edit/5
        //////////public async Task<IActionResult> Edit(int? id)
        //////////{
        //////////    if (id == null)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    var necImpNecrology = await _context.NecImpNecrology.FindAsync(id);
        //////////    if (necImpNecrology == null)
        //////////    {
        //////////        return NotFound();
        //////////    }
        //////////    return View(necImpNecrology);
        //////////}

        //////////// POST: NecImpNecrologies/Edit/5
        //////////// To protect from overposting attacks, enable the specific properties you want to bind to.
        //////////// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //////////[HttpPost]
        //////////[ValidateAntiForgeryToken]
        //////////public async Task<IActionResult> Edit(int id, [Bind("SubDate,SubFname,SubLname,SubEmail,SubRole,CouncilId,DecPrefix,DecFname,DecMname,DecLname,DecSuffix,DecFmorKn,Fmprefix,Fmfname,Fmmname,Fmlname,Fmsuffix,Relationship,Dod,DecMemberId,MemberType,DecOfficesHeld,AssemblyId,Nokprefix,Nokfname,Nokmname,Noklname,Noksuffix,Nokrelate,Nokaddress1,Nokaddress2,Nokcity,Nokstate,Nokzip,Nokcountry,Comments,Id")] NecImpNecrology necImpNecrology)
        //////////{
        //////////    if (id != necImpNecrology.Id)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    if (ModelState.IsValid)
        //////////    {
        //////////        try
        //////////        {
        //////////            _context.Update(necImpNecrology);
        //////////            await _context.SaveChangesAsync();
        //////////        }
        //////////        catch (DbUpdateConcurrencyException)
        //////////        {
        //////////            if (!NecImpNecrologyExists(necImpNecrology.Id))
        //////////            {
        //////////                return NotFound();
        //////////            }
        //////////            else
        //////////            {
        //////////                throw;
        //////////            }
        //////////        }
        //////////        return RedirectToAction(nameof(Index));
        //////////    }
        //////////    return View(necImpNecrology);
        //////////}

        //////////// GET: NecImpNecrologies/Delete/5
        //////////public async Task<IActionResult> Delete(int? id)
        //////////{
        //////////    if (id == null)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    var necImpNecrology = await _context.NecImpNecrology
        //////////        .FirstOrDefaultAsync(m => m.Id == id);
        //////////    if (necImpNecrology == null)
        //////////    {
        //////////        return NotFound();
        //////////    }

        //////////    return View(necImpNecrology);
        //////////}

        //////////// POST: NecImpNecrologies/Delete/5
        //////////[HttpPost, ActionName("Delete")]
        //////////[ValidateAntiForgeryToken]
        //////////public async Task<IActionResult> DeleteConfirmed(int id)
        //////////{
        //////////    var necImpNecrology = await _context.NecImpNecrology.FindAsync(id);
        //////////    if (necImpNecrology != null)
        //////////    {
        //////////        _context.NecImpNecrology.Remove(necImpNecrology);
        //////////    }

        //////////    await _context.SaveChangesAsync();
        //////////    return RedirectToAction(nameof(Index));
        //////////}

        //////////private bool NecImpNecrologyExists(int id)
        //////////{
        //////////    return _context.NecImpNecrology.Any(e => e.Id == id);
        //////////}
    }
}
