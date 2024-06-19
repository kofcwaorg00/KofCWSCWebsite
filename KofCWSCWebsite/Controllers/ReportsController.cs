using FastReport.Web;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class ReportsController : Controller
    {
        readonly DataSetService _dataSetService;
        private readonly ApplicationDbContext? _context;
        readonly IWebHostEnvironment? _hostingEnvironment;
        private IConfiguration _configuration;

        public ReportsController(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment, DataSetService dataSetService, IConfiguration configuration)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _dataSetService = dataSetService;
            _configuration = configuration;
        }
        [Route("GetLabelByOffice")]
        public IActionResult GetLabelByOffice(string ID)
        {
            Log.Information("Starting GetLabelByOffice");
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
            string myMethod = "GetLabelByOffice/";
            Log.Information("Before Load Report");
            model.WebReport.Report.Load(Path.Combine(_dataSetService.ReportsPath, $"{reportToLoad}.frx"));
            Log.Information("Before Prepare Report");
            _dataSetService.PrepareReport(model.WebReport.Report, myMethod, _configuration, ID);
            Log.Information("Before Return");
            return View(model);
        }
    }
}
