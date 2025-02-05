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
using FastReport.Export.PdfSimple;
using FastReport;
using System.Text;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;


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
            if (id == 0)
            {
                throw new Exception($"GetCheckBatch was called with a 0 from {Request.Path}");
            }
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
            ViewBag.NextCheckNumber = NextCheckNumber;
            ViewBag.PrintCheckNumber = PrintCheckNumber;
            ViewBag.GroupID = GroupID;
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
        }

        public IActionResult Pdf(int NextCheckNumber, bool PrintCheckNumber, int GroupID)
        {
            PrintMPDChecks model = new()
            {
                WebReport = new WebReport(),
            };
            int myPCN = PrintCheckNumber ? 1 : 0;

            var reportToLoad = "MPDChecksAPI";

            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));

            var myReport = _dataSetService.PrepareReport(model.WebReport.Report, _configuration, GroupID, NextCheckNumber, myPCN);

            myReport.Prepare();

            PDFSimpleExport pdfExport = new PDFSimpleExport();

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    pdfExport.Export(myReport, ms);
                    ms.Position = 0; // Reset stream position
                    // Check if memory stream contains any data
                    if (ms.Length > 0)
                    {
                        // Create a file content result
                        var fileBytes = ms.ToArray(); // Copy the content to a byte array
                        return File(fileBytes, "application/pdf", "CheckBatch.pdf");
                    }
                    else
                    {
                        return BadRequest("Failed to generate the PDF. The report might be empty or not properly prepared.");
                    }
                }
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return BadRequest("Error in Memory Stream");
            }
            
        }

        public async Task<IActionResult> ExportCSV(int GroupID)
        {
            var mydata = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>($"GetMPDChecks/{GroupID}");

            var csv = new StringBuilder();

            var properties = typeof(CvnMpd).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var item in mydata)
            {
                var values = properties.Select(p => QuoteField(p.GetValue(item)?.ToString() ?? string.Empty));
                csv.AppendLine(string.Join(",", values));
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());

            return File(buffer, "text/csv", "CheckBatch.csv");
        }


        public async Task<IActionResult> ExportQB(int GroupID)
        {
            var mydata = await _apiHelper.GetAsync<IEnumerable<CvnMpd>>($"GetMPDChecks/{GroupID}");

            var csv = new StringBuilder();

            var properties = typeof(CvnMpd).GetProperties(System.Reflection.BindingFlags.Public | System.Reflection.BindingFlags.Instance);

            csv.AppendLine(string.Join(",", properties.Select(p => p.Name)));

            foreach (var item in mydata)
            {
                var values = properties.Select(p => QuoteField(p.GetValue(item)?.ToString() ?? string.Empty));
                csv.AppendLine(string.Join(",", values));
            }

            byte[] buffer = Encoding.UTF8.GetBytes(csv.ToString());

            return File(buffer, "text/csv", "CheckBatch.csv");
        }

        public async Task<IActionResult> ArchiveCheckBatch(int GroupID)
        {
            var mydata = await _apiHelper.GetAsync<int>($"MPD/ArchiveMPD/{GroupID}");
            return RedirectToAction( "Index","Home");
        }

        private static string QuoteField(string field)
        {
            if (field.Contains(","))
            {
                field = $"\"{field.Replace("\"", "\"\"")}\"";
            }
            return field;
        }
    }
    

}
