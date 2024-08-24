using com.sun.xml.@internal.bind.v2.model.core;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Serilog;

namespace KofCWSCWebsite.Controllers
{
    public class SPController : Controller
    {
        //******************************************************************************************
        // 8/24/2024 Tim Philomeno
        // changed this controller over to only use the API
        // so DbContext is no longer needed
        //private readonly ApplicationDbContext _context;
        //******************************************************************************************
        private IConfiguration _configuration;
        private DataSetService _dataSetService;

        public SPController(ApplicationDbContext context, IConfiguration configuration, DataSetService dataSetService)
        {
            Log.Information("Initializing SPController");
            //_context = context;
            _configuration = configuration;
            _dataSetService = dataSetService;
        }

        // GET: GetAssys
        [Route("GetAssys")]
        public IActionResult GetAssys()
        {
            // each one of these should follow the same pattern
            // set the URI, setup thew client, call the client, return the results
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetAssys");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetAssysView> assys;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetAssysView>>();
                    readTask.Wait();
                    assys = readTask.Result;
                }
                else
                {
                    assys = Enumerable.Empty<SPGetAssysView>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/GetAssys.cshtml", assys);
            }
        }
        // GET: GetCouncils
        [Route("GetCouncils")]
        public IActionResult GetCouncils()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetCouncils");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetCouncilsView> councils;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetCouncilsView>>();
                    readTask.Wait();
                    councils = readTask.Result;
                }
                else
                {
                    councils = Enumerable.Empty<SPGetCouncilsView>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/GetCouncils.cshtml", councils);
            }
        }

        // GET: GetSOS
        [Route("GetSOSView")]
        public IActionResult GetSOSView()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetSOSView");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetSOSView> sosview;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetSOSView>>();
                    readTask.Wait();
                    sosview = readTask.Result;
                }
                else
                {
                    sosview = Enumerable.Empty<SPGetSOSView>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                ViewData["myHost"] = HttpContext.Request.Host.Value;
                return View("Views/StateFamily/GetSOSView.cshtml", sosview);
            }
        }

        // GET: Bulletins
        [Route("GetBulletins")]
        public IActionResult GetBulletins()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetBulletins");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetBulletins> bulletins;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetBulletins>>();
                    readTask.Wait();
                    bulletins = readTask.Result;
                }
                else
                {
                    bulletins = Enumerable.Empty<SPGetBulletins>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/Home/GetBulletins.cshtml", bulletins);
            }
        }

        // GET: EmailAlias
        [Route("GetEmailAlias")]
        public IActionResult GetEmailAlias()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetEmailAlias");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetEmailAlias> emailalias;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetEmailAlias>>();
                    readTask.Wait();
                    emailalias = readTask.Result;
                }
                else
                {
                    emailalias = Enumerable.Empty<SPGetEmailAlias>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/GetEmailAlias.cshtml", emailalias);
            }
        }
        // GET: Chairmen
        [Route("GetChairmen")]
        public IActionResult GetChairmen()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetChairmen");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetChairmen> chairmen;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetChairmen>>();
                    readTask.Wait();
                    chairmen = readTask.Result;
                }
                else
                {
                    chairmen = Enumerable.Empty<SPGetChairmen>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/GetChairmen.cshtml", chairmen);
            }
        }
        // GET: ChairmanInfoBlock
        [Route("GetChairmanInfoBlock")]
        public IActionResult GetChairmanInfoBlock(int id)
        {
            if (id == 0)
            {
                return NotFound();
            }
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetChairmanInfoBlock/"+id.ToString());

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetChairmanInfoBlock> cib;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetChairmanInfoBlock>>();
                    readTask.Wait();
                    cib = readTask.Result;
                }
                else
                {
                    cib = Enumerable.Empty<SPGetChairmanInfoBlock>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/ChairmanDetails.cshtml", cib);
            }
        }
        
        // GET: Chairmen
        [Route("GetDDs")]
        public IActionResult GetDDs()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetDDs");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetDDs> dds;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetDDs>>();
                    readTask.Wait();
                    dds = readTask.Result;
                }
                else
                {
                    dds = Enumerable.Empty<SPGetDDs>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/GetDDs.cshtml", dds);
            }
        }

        // GET: FourthDegreeOfficers
        [Route("FourthDegreeOfficers")]
        public IActionResult GetFourthDegreeOfficers()
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/FourthDegreeOfficers");

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                IEnumerable<SPGetChairmanInfoBlock> fourthdeg;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<SPGetChairmanInfoBlock>>();
                    readTask.Wait();
                    fourthdeg = readTask.Result;
                }
                else
                {
                    fourthdeg = Enumerable.Empty<SPGetChairmanInfoBlock>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View("Views/StateFamily/FourthDegreeOfficers.cshtml", fourthdeg);
            }
        }
    }
}
