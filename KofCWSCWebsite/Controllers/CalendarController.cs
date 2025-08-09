using KofCWSCWebsite.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
//using System.Web.Mvc;

namespace KofCWSCWebsite.Controllers
{
    public class CalendarController : Controller
    {
        private readonly DataSetService _dataSetService;
        public CalendarController(DataSetService dataSetService)
        {
               _dataSetService = dataSetService;
        }
        /// <summary>
        // 8/8/2025 Tim Philomeno
        // The only thing this does is provide for getting the view to display

        /// all data is exchanged using the API inside the page
        /// </summary>
        /// <returns> the calendar view</returns>
        /// 
        [Authorize(Roles = "Admin,CalAdmin")]
        public IActionResult ViewCalendar()
        {
            var myBaseUrl = _dataSetService.GetAPIBaseAddress();
            ViewBag.apiBaseUrl = myBaseUrl;
            return View();
        }
    }
}
