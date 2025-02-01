using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using FastReport.Web;
using KofCAdmin.Models;


namespace KofCWSCWebsite.Controllers
{
    public class CvnMpdController : Controller
    {
        private readonly ApiHelper _apiHelper;
        readonly DataSetService _dataSetService;
        private IConfiguration _configuration;
        public CvnMpdController(ApiHelper apiHelper,DataSetService dataSetService,IConfiguration configuration)
        {
            _apiHelper = apiHelper;
            _dataSetService = dataSetService;
            _configuration = configuration;
        }

        // GET: CvnMpd
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> Index(int id)
        {
            try
            {
                if (id == 3) { ViewBag.Group = "District Deputies"; };
                if (id == 25) { ViewBag.Group = "Delegate"; };
                var result = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>($"MPD/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }
        [HttpGet("GetCheckBatch/{id}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> GetCheckBatch(int id)
        {
            try
            {
                Response.Cookies.Delete("councilFilter");
                if (id == 25)
                {
                    Response.Cookies.Append("typeFilter", "DEL");
                }
                ViewBag.GroupID = id;
                if (id == 3) { ViewBag.Group = "District Deputies"; };
                if (id == 25) { ViewBag.Group = "Delegate"; };

                var result = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>($"MPD/GetCheckBatch/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
        }
        [HttpGet("ToggleCouncilDays/{id}/{day}/{del}/{groupid}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> ToggleCouncilDays(int id, int day,string del, int groupid)
        {
            //**************************************************************************************
            // 1/5/2025 Tim PHilomeno
            // Need to deal with the redirect to action based on where we are toggling from
            var myAffectedRows = await _apiHelper.GetAsync<int>($"/ToggleCouncilDays/{id}/{day}/{del}");
            switch (groupid)
            {
                case 0: // from AttendeeDays
                    return RedirectToAction("GetAttendeeDays", "Convention", new { council = 0, groupid = 0 });
                case 3: // DDs from Check Batch only other place we can toggle days
                    return RedirectToAction("GetCheckBatch", "CvnMpd", new { id = 3 });
                case 25: // DDs from Check Batch only other place we can toggle days
                    return RedirectToAction("GetCheckBatch", "CvnMpd", new { id = 25 });
                default:
                    return RedirectToAction("GetAttendeeDays", "Convention", new { council = 0, groupid });
            }

        }
        [HttpGet("ToggleDelegateDaysMPD/{id}/{day}/{groupid}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> ToggleDelegateDaysMPD(int id, int day,int groupid)
        {
            //**************************************************************************************
            // 1/5/2025 Tim PHilomeno
            // Need to deal with the redirect to action based on where we are toggling from
            var myAffectedRows = await _apiHelper.GetAsync<int>($"/ToggleDelegateDays/{id}/{day}");
            switch (groupid) {
                case 0: // from AttendeeDays
                    return RedirectToAction("GetAttendeeDays", "Convention", new { council = 0, groupid = 0 });
                case 3: // DDs from Check Batch only other place we can toggle days
                    return RedirectToAction("GetCheckBatch", "CvnMpd", new { id = 3 });
                default:
                    return RedirectToAction("GetAttendeeDays", "Convention", new { council = 0, groupid});
            }
            
        }
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public IActionResult PrintChecks(int NextCheckNumber,bool PrintCheckNumber,int GroupID)
        {
            if(NextCheckNumber <= 0)
            {
                TempData["CheckNumberError"] = "Error...Check number must be > 0";
                //RedirectToPage("GetCheckBatch", 25);
                return RedirectToAction("GetCheckBatch","CvnMpd", new { id = GroupID } );
            }
            int myPCN = PrintCheckNumber ? 1 : 0;

            PrintMPDChecks model = new()
            {
                WebReport = new WebReport(),
            };
            var reportToLoad = "MPDChecksAPI";
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            _dataSetService.PrepareReport(model.WebReport.Report, _configuration, GroupID,NextCheckNumber,myPCN);
            return View(model);




            return RedirectToAction("GetCheckBatch", "CvnMpd", new { id = 25 });
        }
    }
}
