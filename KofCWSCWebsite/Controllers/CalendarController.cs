using KofCWSCWebsite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;

namespace KofCWSCWebsite.Controllers
{
    public class CalendarController : Controller
    {
        private readonly DataSetService _dataSetService;
        private readonly IWebHostEnvironment _env;

        public CalendarController(DataSetService dataSetService, IWebHostEnvironment env)
        {
            _dataSetService = dataSetService;
            _env = env;
        }
        /// <summary>
        // 8/8/2025 Tim Philomeno
        // The only thing this does is provide for getting the view to display

        /// all data is exchanged using the Javascript and the API inside the page
        /// we are setting baseUrl as the API url.  The page picks this up and the Javascript fetch() uses it
        /// </summary>
        /// <returns> the calendar view</returns>
        /// 
        //[Authorize(Roles = "Admin,CalAdmin")]
        public IActionResult ViewCalendar()
        {
            string myBaseUrl = Request.Host.Value.ToString();
            //if (_env.IsDevelopment())
            //{
            //    myBaseUrl = _dataSetService.GetAPIBaseAddress();
            //}
            //else
            //{
            //    myBaseUrl = Request.Host.Value.ToString();
            //    ///myBaseUrl = $"https://kofc-wa.org";
            //}
            ViewBag.apiBaseUrl = $"https://{myBaseUrl}";

            if (User.IsInRole("Admin") || User.IsInRole("CalAdmin"))
            {
                ViewBag.CanEdit = true;
            }
            else
            {
                ViewBag.CanEdit = false;
            }


                return View();
        }
    }
}
