using com.sun.xml.@internal.bind.v2.model.core;
using FastReport.Web;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class ReportsController : Controller
    {
        readonly DataSetService _dataSetService;
        readonly IWebHostEnvironment? _hostingEnvironment;
        private IConfiguration _configuration;

        public ReportsController(IWebHostEnvironment hostingEnvironment, DataSetService dataSetService, IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _dataSetService = dataSetService;
            _configuration = configuration;
        }

        [Authorize(Roles = "Admin")]
        [Route("GetLabelByGroup")]
        public IActionResult GetLabelByGroup(int ID)
        {
            GetLabelByGroupModel model = new()
            {
                WebReport = new WebReport(),
            };
            //****************************************************************************************************
            // To get the link between report parameter and stored procedure parameter, create a report parameter
            // @InOffice.  Then go to the stored procedure properties parameters list and link this new parameter
            // to the stored procedure parameter using the expression box.  Then the value here is transferred
            // to the call of the stored proceudre
            //****************************************************************************************************
            var reportToLoad = "GetLabelByGroupAPI";
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            _dataSetService.PrepareReport(model.WebReport.Report, _configuration, ID);

            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [Route("GetLabelByOffice")]
        public IActionResult GetLabelByOffice(int ID)
        {
            GetLabelByOfficeModel model = new()
            {
                WebReport = new WebReport(),
            };
            //****************************************************************************************************
            // To get the link between report parameter and stored procedure parameter, create a report parameter
            // @InOffice.  Then go to the stored procedure properties parameters list and link this new parameter
            // to the stored procedure parameter using the expression box.  Then the value here is transferred
            // to the call of the stored proceudre
            //****************************************************************************************************
            var reportToLoad = "GetLabelByOfficeAPI";
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            _dataSetService.PrepareReport(model.WebReport.Report, _configuration, ID);
            return View(model);
        }

        [Authorize(Roles = "Admin")]
        [Route("GetDirectory/{Id}")]
        public IActionResult GetDirectory(int Id)
        {
            //**********************************************************************************************
            // 10/05/2024 Tim Philomeno
            // Type will containe 1-4 to be able to set the 4 combinations of ShortForm and NextYear
            //----------------------------------------------------------------------------------------------
            int ShortForm=0;
            int NextYear=0;
            if (Id < 0 || Id > 4) { ShortForm = 0;NextYear = 0; }
            if (Id == 1) { ShortForm = 0;NextYear = 0; } //Full Current Year
            if (Id == 2) { ShortForm = 1; NextYear = 0; } //Short Current Year
            if (Id == 3) { ShortForm = 0; NextYear = 1; } //Full Next Year
            if (Id == 4) { ShortForm = 1; NextYear = 1; } //Short Next Year
            DirMain model = new()
            {
                WebReport = new WebReport(),
            };
            var reportToLoad = "DirectoryAPI";
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            _dataSetService.PrepareReport(model.WebReport.Report, _configuration, ShortForm, NextYear);
            return View(model);
        }
        [Authorize(Roles = "Admin")]
        [Route("GetRollCallSheets/{Id}")]
        public IActionResult GetRollCallSheets(int Id)
        {
            
            RollCallSheets model = new()
            {
                WebReport = new WebReport(),
            };
            var reportToLoad = "RollCallSheetsAPI";
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            _dataSetService.PrepareReport(model.WebReport.Report, _configuration, Id);
            return View(model);
        }
    }
}
