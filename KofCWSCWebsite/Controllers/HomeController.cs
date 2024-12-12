using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.EntityFrameworkCore;
using Serilog;
using System.Diagnostics;

namespace KofCWSCWebsite.Controllers
{
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;
        private DataSetService _dataSetService;

        public HomeController(ApplicationDbContext context, DataSetService dataSetService)
        {
            _dataSetService = dataSetService; 
            _context = context;
        }

        public async Task<IActionResult> Index()
        {
            //*****************************************************************************************************
            // 6/6/2024 Tim Philomeno
            //  We need to be able to give feedback as to which environment we are in during development
            //  and implementation
            //*****************************************************************************************************
            if (_context.Database.GetDbConnection().ConnectionString.Contains("local") || _context.Database.GetDbConnection().ConnectionString.Contains("EXPRESS"))
            {
                ViewData["ConnectString"] = "Using LOCAL DATABASE";
            }
            else if (_context.Database.GetDbConnection().ConnectionString.Contains("KofCWSCWebDev"))
            {
                ViewData["ConnectString"] = "Using AZURE DEVELOPMENT DATABASE";
            }
            else if (_context.Database.GetDbConnection().ConnectionString.Contains("KofCWSCWeb"))
            {
                ViewData["ConnectString"] = "Using AZURE PRODUCTION DATABASE";
            }
            else
            {
                ViewData["ConnectString"] = "Using UNKNOWN DATABASE";
            }
            ViewData["APIURL"] = "and the APIURL is using " + _dataSetService.GetAPIBaseAddress();
            ViewData["ENV"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            try
            {
                var apiHelper = new ApiHelper(_dataSetService);
                var result = await apiHelper.GetAsync<IEnumerable<HomePageViewModel>>("/Home");
                return View(result);
                
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return BadRequest(ex.Message + $" Check to Make sure you are NOT hitting the Production API.  You should only run against a local API {ViewData["APIURL"]}");
            }
            
            //------------------------------------------------------------------------------------------------------

            //}
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
