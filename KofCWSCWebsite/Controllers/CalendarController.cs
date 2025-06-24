using Microsoft.AspNetCore.Mvc;

namespace KofCWSCWebsite.Controllers
{
    public class CalendarController : Controller
    {
        public IActionResult FullView()
        {
            return View();
        }
    }
}
