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
    public class CvnImpDelegatesController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly ApiHelper _apiHelper;

        public CvnImpDelegatesController(ApplicationDbContext context, ApiHelper apiHelper)
        {
            _context = context;
            _apiHelper = apiHelper;
        }

        // GET: CvnImpDelegates - This gets the main index view for edit
        public async Task<ActionResult<IEnumerable<CvnImpDelegate>>> Index()
        {
            ViewBag.Message = TempData["Message"];
            var results = await _apiHelper.GetAsync<IEnumerable<CvnImpDelegate>>("CvnImpDelegates");
            return View(results.OrderBy(e => e.CouncilNumber));
        }
        // NOT USED ***
        //public async Task<ActionResult<IEnumerable<CvnImpDelegateIMP>>> IndexIMP()
        //{
        //    var results = await _apiHelper.GetAsync<IEnumerable<CvnImpDelegateIMP>>("CvnImpDelegatesIMP");
        //    return View(results.OrderBy(e => e.CouncilNumber));
        //    //return View(await _context.Database
        //    //        .SqlQuery<CvnImpDelegate>($"EXECUTE uspCVN_GetImpDelegates")
        //    //        .ToListAsync());
        //    //return View(await _context.CvnImpDelegates.ToListAsync());
        //}

        // GET: CvnImpDelegates/Details/5 - Not Used ***
        //public async Task<IActionResult> Details(int? id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var cvnImpDelegate = await _context.CvnImpDelegates
        //        .FirstOrDefaultAsync(m => m.Id == id);
        //    if (cvnImpDelegate == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(cvnImpDelegate);
        //}

        // GET: CvnImpDelegates/Create nont used ***
        //public IActionResult Create()
        //{
        //    return View();
        //}

        // POST: CvnImpDelegates/Create - This is used to create a raw record GREEN Create a Manual Council Import Record ***
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("SubmissionDate,FormSubmitterSEmail,CouncilName,CouncilNumber,D1FirstName,D1MiddleName,D1LastName,D1Suffix,D1MemberID,D1Address1,D1Address2,D1City,D1State,D1ZipCode,D1Phone,D1Email,D2FirstName,D2MiddleName,D2LastName,D2Suffix,D2MemberID,D2Address1,D2Address2,D2City,D2State,D2ZipCode,D2Phone,D2Email,A1FirstName,A1MiddleName,A1LastName,A1Suffix,A1MemberID,A1Address1,A1Address2,A1City,A1State,A1ZipCode,A1Phone,A1Email,A2FirstName,A2MiddleName,A2LastName,A2Suffix,A2MemberID,A2Address1,A2Address2,A2City,A2State,A2ZipCode,A2Phone,A2Email,Id,RecType")] CvnImpDelegateIMP cvnImpDelegateIMP)
        {
            if (ModelState.IsValid)
            {
                var results = await _apiHelper.PostAsync<CvnImpDelegateIMP, string>("CvnImpDelegate", cvnImpDelegateIMP);

                ViewBag.ImpCreateMessage = results;

                return View("~/Views/CvnImpDelegates/Failed.cshtml");
            }
            else
            {
                return View(cvnImpDelegateIMP);
            }
            
        }

        //[HttpGet("Edit/{id}")] - duplicate we are using EditIMP so this never gets called
        //public async Task<ActionResult<CvnImpDelegateIMP>> Edit(int? id)
        //{
        //    var results = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{id}");
            
        //    return View("EditIMP",results);
            
        //}
        // Orange Edit Raw button ***
        public async Task<ActionResult<CvnImpDelegateIMP>> EditIMP(int? id)
        {
            // this is being called 2 times, 1 with and id and 1 with a null id
            // not sure why
            if (id is not null)
            {
                var results = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{id}");

                return View("EditIMP", results);
            }
            return View();
        }
        // GET: CvnImpDelegates/Edit/5 used to edit imported and existing data combined Blue Edit Button ***
        [HttpGet("Edit/{id}/{validate}/{validate2}")]
        public async Task<ActionResult<CvnImpDelegate>> Edit(int? id,string? validate,string? validate2)
        {
            ViewBag.Validate = validate;
            ViewBag.Validate2 = validate2;
            try
            {
                var results = _context.Database
                       .SqlQuery<CvnImpDelegate>($"EXECUTE uspCVN_GetImpDelegatesByID {id}")
                       .AsEnumerable()
                       .FirstOrDefault();
                return View(results);

            }
            catch (Exception ex)
            {
                return View();
            }
        }

        // POST: CvnImpDelegates/Edit/5 - this gets called on edit raw data BLUE button Save Delegate Changes ***
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        //[ValidateAntiForgeryToken]
        public async Task<IActionResult> EditIMP(int id, [Bind("SubmissionDate,FormSubmitterSEmail,CouncilName,CouncilNumber,D1FirstName,D1MiddleName,D1LastName,D1Suffix,D1MemberID,D1Address1,D1Address2,D1City,D1State,D1ZipCode,D1Phone,D1Email,D2FirstName,D2MiddleName,D2LastName,D2Suffix,D2MemberID,D2Address1,D2Address2,D2City,D2State,D2ZipCode,D2Phone,D2Email,A1FirstName,A1MiddleName,A1LastName,A1Suffix,A1MemberID,A1Address1,A1Address2,A1City,A1State,A1ZipCode,A1Phone,A1Email,A2FirstName,A2MiddleName,A2LastName,A2Suffix,A2MemberID,A2Address1,A2Address2,A2City,A2State,A2ZipCode,A2Phone,A2Email,Id,RecType")] CvnImpDelegateIMP cvnImpDelegate)
        {
            if (id != cvnImpDelegate.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    var results = await _apiHelper.PutAsync<CvnImpDelegateIMP, CvnImpDelegateIMP>($"CvnImpDelegate/{id}", cvnImpDelegate);
                    // a successful PUT will return JSon("SUCCESS") and be serialized back as a CvnImpDelegateIMP model with all nulls
                    // a failed PUT will return a NULL results
                    if (results == null)
                    {
                        throw new Exception("Edit Failed");
                    }
                }
                catch (Exception ex)
                {
                    Log.Error(Utils.FormatLogEntry(this, ex, "Edit Failed"));
                }
                return RedirectToAction(nameof(Index));
            }
            return RedirectToAction("CvnImpDelegates", "Index");
        }

        // GET: CvnImpDelegates/Delete/5 Red Delete Button ***
        public async Task<IActionResult> DeleteIMP(int? id)
        {
            var results = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{id}");

            return View("Delete", results);

        }

        // POST: CvnImpDelegates/Delete/5 - after confirm from Red Delete Button ***
        [HttpPost, ActionName("DeleteIMP")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            await _apiHelper.DeleteAsync($"CvnImpDelegate/{id}");
            return RedirectToAction(nameof(Index));
        }

        //private bool CvnImpDelegateExists(int id)
        //{
        //    return _context.CvnImpDelegates.Any(e => e.Id == id);
        //}
    }
}
