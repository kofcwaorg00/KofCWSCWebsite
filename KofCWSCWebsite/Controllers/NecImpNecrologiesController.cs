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
using System.Formats.Asn1;
using javax.print.attribute.standard;
using Microsoft.AspNetCore.Authorization;
using Serilog;
using Microsoft.AspNetCore.Diagnostics;
using System.Diagnostics;

namespace KofCWSCWebsite.Controllers
{
    public class NecImpNecrologiesController : Controller
    {
        private ApiHelper _apiHelper;

        public NecImpNecrologiesController(ApiHelper apiHelper)
        {
            _apiHelper = apiHelper;
        }

        // GET: NecImpNecrologies
        public async Task<IActionResult> Index()
        {
            // 5/12/2025 Tim Philomeno New STD
            // no error processing is necessary here on a normal index.  If there is no data and the model is null then
            // the view can respond accordingly
            // if we get an exception back from the API, we need to handle it and display the error view with the information
            try
            {
                var results = await _apiHelper.GetAsync<List<NecImpNecrologyVM>>("Necrology");
                return View(results);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                ViewBag.Error = ex;
                var errorModel = new ErrorViewModel
                {
                    RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                    Message = ex.Message,
            };

                return View("Error",errorModel);
            }
            
        }

        [HttpGet("UpdateDeceased/{id}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<ActionResult<IEnumerable<CvnDelegateDays>>> UpdateDeceased(int id)
        {
            var myAffectedRows = await _apiHelper.GetAsync<int>($"/UpdateDeceased/{id}");
            return RedirectToAction("Index");

        }
    }
}
