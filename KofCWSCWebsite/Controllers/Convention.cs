using com.sun.xml.@internal.bind.v2.model.core;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using NuGet.ContentModel;
using System.Windows.Forms;
using Serilog;
using FastReport.Utils;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KofCWSCWebsite.Controllers
{
    public class ConventionController : Controller
    {
        private readonly ApplicationDbContext _context;
        private readonly DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        public ConventionController(ApplicationDbContext context, DataSetService dataSetService)
        {
            _context = context;
            _dataSetService = dataSetService;
            _apiHelper = new ApiHelper(_dataSetService);
        }
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> Index()
        {
            var myViewD = await _apiHelper.GetAsync<IEnumerable<CvnDelegateDays>>($"/GetDelegateDays");
            return View("Views/Convention/DelegateDays.cshtml", myViewD);
        }
        [HttpGet("ToggleDelegateDays/{id}/{day}")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> ToggleDelegateDays(int id, int day)
        {
            var myAffectedRows = _apiHelper.GetAsync<int>($"/ToggleDelegateDays/{id}/{day}");
            var myViewT1 = await _apiHelper.GetAsync<IEnumerable<CvnDelegateDays>>($"/GetDelegateDays");
            //return View("Views/Convention/DelegateDays.cshtml", myViewT1);
            return RedirectToAction("Index");

        }
        [HttpGet("ResetDelegates")]
        public async Task<ActionResult> ResetDelegates()
        {
            try
            {
                var myAffectedRows = await _apiHelper.GetAsync<int>($"/ResetDelegates");
                string myMess = $"<h5>The Reset of Delegate Data Succeeded.  Updated {myAffectedRows} Rows</h5>";
                ViewBag.RowsUpdated = myMess;
                string pagePath = Url.Content("/Convention/ResetDelegatesSuccessFail");
                return Redirect(pagePath);

            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                string myMess = $"<h5>The Reset of Delegate Data Failed with error - {ex.Message}</h5>";
                ViewBag.RowsUpdated = myMess;
                string pagePath = Url.Content("/Convention/ResetDelegatesSuccessFail"); 
                return Redirect(pagePath);
            }
        }
    }
}
