using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
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

        public IActionResult Index()
        {
            //*****************************************************************************************************
            // 6/6/2024 Tim Philomeno
            //  We need to be able to give feedback as to which environment we are in during development
            //  and implementation
            //*****************************************************************************************************
            if (_context.Database.GetDbConnection().ConnectionString.Contains("2k2201"))
            {
                ViewData["ConnectString"] = "Using DASP DEVELOPMENT DATABASE";
            }
            else if (_context.Database.GetDbConnection().ConnectionString.Contains("KofCWSCWebDEV"))
            {
                ViewData["ConnectString"] = "Using AZURE DEVELOPMENT DATABASE";
            }
            else if (_context.Database.GetDbConnection().ConnectionString.Contains("KofCWSCWeb"))
            {
                ViewData["ConnectString"] = "Using AZURE PRODUCTION DATABASE";
            }
            else
            {
                ViewData["ConnectString"] = "Using DASP PRODUCTION DATABASE";
            }
            ViewData["APIURL"] = "and the APIURL is using " + _dataSetService.GetAPIBaseAddress();
            ViewData["ENV"] = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            //*****************************************************************************************************
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Home");

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var myresult = responseTask.Result;
                IEnumerable<HomePageViewModel> home;
                if (myresult.IsSuccessStatusCode)
                {
                    var readTask = myresult.Content.ReadAsAsync<IList<HomePageViewModel>>();
                    readTask.Wait();
                    home = readTask.Result;
                }
                else
                {
                    home = Enumerable.Empty<HomePageViewModel>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(home);
            }
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
