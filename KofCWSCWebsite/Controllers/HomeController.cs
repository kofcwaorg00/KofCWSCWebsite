using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.Diagnostics;

namespace KofCWSCWebsite.Controllers
{
    public class HomeController : Controller
    {
        private readonly ILogger<HomeController> _logger;

        private readonly ApplicationDbContext _context;

        public HomeController(ApplicationDbContext context, ILogger<HomeController> logger)
        {
            _context = context;
            _logger = logger;
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
            //*****************************************************************************************************
            var result = _context.Database
                .SqlQuery<SPGetSOS>($"uspWEB_GetSOS 0")
                .ToList();
            return View(result);
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
