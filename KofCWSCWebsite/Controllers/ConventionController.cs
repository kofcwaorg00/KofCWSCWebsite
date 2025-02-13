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
using Microsoft.AspNetCore.Authorization;

namespace KofCWSCWebsite.Controllers
{
    public class ConventionController : Controller
    {
        private readonly DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        public ConventionController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
            _apiHelper = new ApiHelper(_dataSetService);
        }
        [HttpGet("GetAttendeeDays/{council}/{groupid}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> GetAttendeeDays(int council,int groupid)
        {
            ViewBag.Council = council;
            switch (groupid)
            {
                case 3:
                    Response.Cookies.Append("typeFilter", "DD");
                    break;
                case 25:
                    Response.Cookies.Append("typeFilter", "DEL");
                    break;
                default:
                    //Response.Cookies.Delete("typeFilter");
                    break;
            }

            var myViewD = await _apiHelper.GetAsync<IEnumerable<CvnDelegateDays>>($"/GetAttendeeDays");
            return View("Views/Convention/AttendeeDays.cshtml", myViewD);
        }
        [HttpGet("ToggleDelegateDays/{id}/{day}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> ToggleDelegateDays(int id, int day)
        {
            var myAffectedRows = _apiHelper.GetAsync<int>($"/ToggleDelegateDays/{id}/{day}");
            var myViewT1 = await _apiHelper.GetAsync<IEnumerable<CvnDelegateDays>>($"/GetDelegateDays");
            //return View("Views/Convention/DelegateDays.cshtml", myViewT1);
            return RedirectToAction("Index");

        }
        [HttpGet("ResetDelegates")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult> ResetDelegates()
        {
            try
            {
                var myAffectedRows = await _apiHelper.GetAsync<int>($"/ResetDelegates");
                string myMess = $"The Reset of Delegate Data Succeeded.  Updated {myAffectedRows} Rows";
                ViewBag.RowsUpdated = myMess;
                return View("Views/Convention/ResetDelegatesSuccessFail.cshtml");
                //string pagePath = Url.Content("/Convention/ResetDelegatesSuccessFail");
                //return Redirect(pagePath);

            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                string myMess = $"The Reset of Delegate Data Failed with error - {ex.Message}";
                ViewBag.RowsUpdated = myMess;
                //string pagePath = Url.Content("/Convention/ResetDelegatesSuccessFail"); 
                //return Redirect(pagePath);
                return View("Views/Convention/ResetDelegatesSuccessFail.cshtml");
            }
        }
    }
}
