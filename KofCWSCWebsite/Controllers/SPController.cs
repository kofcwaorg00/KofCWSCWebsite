using com.sun.xml.@internal.bind.v2.model.core;
using javax.swing.text;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using Newtonsoft.Json;
using NuGet.Protocol;
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
        private ApiHelper _apiHelper;
        public SPController(IConfiguration configuration, DataSetService dataSetService, ApiHelper apiHelper)
        {
            _configuration = configuration;
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
        }

        // GET: GetAssys
        [Route("GetAssys/{nextyear}")]
        public async Task<IActionResult> GetAssys(int nextyear = 0)
        {
            ViewData["HostName"] = _dataSetService.GetMyHost();
            var assys = await _apiHelper.GetAsync<IList<SPGetAssysView>>($"GetAssys/{nextyear}");
            // each one of these should follow the same pattern
            // set the URI, setup thew client, call the client, return the results
            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetAssys");

            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetAssysView> assys;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetAssysView>>();
            //        readTask.Wait();
            //        assys = readTask.Result;
            //    }
            //    else
            //    {
            //        assys = Enumerable.Empty<SPGetAssysView>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //    return View("Views/StateFamily/GetAssys.cshtml", assys);
            //}
            return View("Views/StateFamily/GetAssys.cshtml", assys);
        }
        // GET: GetCouncils
        [Route("GetCouncils/{nextyear}")]
        public async Task<IActionResult> GetCouncils(int nextyear = 0)
        {
            ViewData["HostName"] = _dataSetService.GetMyHost();
            var councils = await _apiHelper.GetAsync<IList<SPGetCouncilsView>>($"GetCouncils/{nextyear}");
            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetCouncils");

            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetCouncilsView> councils;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetCouncilsView>>();
            //        readTask.Wait();
            //        councils = readTask.Result;
            //    }
            //    else
            //    {
            //        councils = Enumerable.Empty<SPGetCouncilsView>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //}
            return View("Views/StateFamily/GetCouncils.cshtml", councils);
        }

        // GET: GetSOS
        [Route("GetSOSView/{NextYear}")]
        public async Task<IActionResult> GetSOSView(int NextYear = 0)
        {
            ViewData["HostName"] = _dataSetService.GetMyHost();
            var sosview = await _apiHelper.GetAsync<IList<SPGetSOSView>>($"GetSOSView/{NextYear}");
            ViewData["myHost"] = HttpContext.Request.Host.Value;
            return View("Views/StateFamily/GetSOSView.cshtml", sosview);


            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetSOSView");

            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetSOSView> sosview;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetSOSView>>();
            //        readTask.Wait();
            //        sosview = readTask.Result;
            //    }
            //    else
            //    {
            //        sosview = Enumerable.Empty<SPGetSOSView>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //    ViewData["myHost"] = HttpContext.Request.Host.Value;
            //    return View("Views/StateFamily/GetSOSView.cshtml", sosview);
            //}
        }

        // GET: Bulletins - OBSOLETE
        //[Route("GetBulletins")]
        //public IActionResult GetBulletins()
        //{
        //    Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetBulletins");

        //    using (var client = new HttpClient())
        //    {
        //        //client.BaseAddress = new Uri(myURI);
        //        var responseTask = client.GetAsync(myURI);
        //        responseTask.Wait();
        //        var result = responseTask.Result;
        //        IEnumerable<SPGetBulletins> bulletins;
        //        if (result.IsSuccessStatusCode)
        //        {
        //            var readTask = result.Content.ReadAsAsync<IList<SPGetBulletins>>();
        //            readTask.Wait();
        //            bulletins = readTask.Result;
        //        }
        //        else
        //        {
        //            bulletins = Enumerable.Empty<SPGetBulletins>();
        //            ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
        //        }
        //        return View("Views/Home/GetBulletins.cshtml", bulletins);
        //    }
        //}

        // GET: EmailAlias
        [Authorize]
        [Route("GetEmailAlias")]
        public IActionResult GetEmailAlias()
        {
            //********************************************************************************
            // 11/1/2024 Tim Philomeno
            // create a session cookie, the MinValue does the trick
            HttpContext.Response.Cookies.Append("IAgreeSensitive", "true", new CookieOptions
            {
                //Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/",
                HttpOnly = false, // Accessible only by the server
                IsEssential = true // Required for GDPR compliance
            });
            //----------------------------------------------------------------------------------
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
        [Route("GetChairmen/{nextyear}")]
        public async Task<IActionResult> GetChairmen(int nextyear = 0)
        {
            ViewData["HostName"] = _dataSetService.GetMyHost();
            var chairmen = await _apiHelper.GetAsync<IList<SPGetChairmen>>($"GetChairmen/{nextyear}");
            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetChairmen");

            //using (var client = new HttpClient())
            //{

            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetChairmen> chairmen;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetChairmen>>();
            //        readTask.Wait();
            //        chairmen = readTask.Result;
            //    }
            //    else
            //    {
            //        chairmen = Enumerable.Empty<SPGetChairmen>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //    return View("Views/StateFamily/GetChairmen.cshtml", chairmen);
            //}
            return View("Views/StateFamily/GetChairmen.cshtml", chairmen);
        }
        // GET: ChairmanInfoBlock
        [Route("GetChairmanInfoBlock/{id}/{nextyear}")]
        public async Task<IActionResult> GetChairmanInfoBlock(int id,int nextyear = 0)
        {
            if (id == 0)
            {
                return NotFound();
            }
            var cib = await _apiHelper.GetAsync<IList<SPGetChairmanInfoBlock>>($"GetChairmanInfoBlock/{id}/{nextyear}");
            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetChairmanInfoBlock/" + id.ToString());

            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetChairmanInfoBlock> cib;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetChairmanInfoBlock>>();
            //        readTask.Wait();
            //        cib = readTask.Result;
            //    }
            //    else
            //    {
            //        cib = Enumerable.Empty<SPGetChairmanInfoBlock>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //    return View("Views/StateFamily/ChairmanDetails.cshtml", cib);
            //}
            return View("Views/StateFamily/ChairmanDetails.cshtml", cib);
        }

        // GET: Chairmen
        [Route("GetDDs/{nextyear}")]
        public async Task<IActionResult> GetDDs(int nextyear = 0)
        {
            ViewData["HostName"] = _dataSetService.GetMyHost();
            var dds = await _apiHelper.GetAsync<IList<SPGetDDs>>($"GetDDs/{nextyear}");
            //Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetDDs");

            //using (var client = new HttpClient())
            //{
            //    //client.BaseAddress = new Uri(myURI);
            //    var responseTask = client.GetAsync(myURI);
            //    responseTask.Wait();
            //    var result = responseTask.Result;
            //    IEnumerable<SPGetDDs> dds;
            //    if (result.IsSuccessStatusCode)
            //    {
            //        var readTask = result.Content.ReadAsAsync<IList<SPGetDDs>>();
            //        readTask.Wait();
            //        dds = readTask.Result;
            //    }
            //    else
            //    {
            //        dds = Enumerable.Empty<SPGetDDs>();
            //        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            //    }
            //    return View("Views/StateFamily/GetDDs.cshtml", dds);
            //}
            return View("Views/StateFamily/GetDDs.cshtml", dds);
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
        [Route("GetNextTempID")]
        public IActionResult GetNextTempID()
        {
            try
            {
                Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetNextTempID");
                using (var client = new HttpClient())
                {
                    var responseTask = client.GetAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        //**********************************************************
                        // 11/7/2024 Tim PHilomeno
                        // there has to be a better way to do this.  I just want the
                        // string that is returned.  I suspect that it has to do with
                        // what the API returning 
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        var myID = readTask.Result;
                        myID = myID.Replace("[", "");
                        myID = myID.Replace("]", "");
                        ViewBag.NextTempID = myID;
                        //------------------------------------------------------------
                    }
                    else
                    {
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        ViewBag.NextTempID = null;
                    }
                    return View("Views/NextTempID/NextTempID.cshtml");
                }
            }

            catch (Exception ex)
            {

                Log.Fatal(ex.Message + " " + ex.InnerException);
                return Json(string.Empty);
            }

        }
    }
}
