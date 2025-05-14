using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Services;
using Serilog;
using Newtonsoft.Json;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace KofCWSCWebsite.Controllers
{
    public class TblCorrMemberOfficesController : Controller
    {
        //private readonly ApplicationDbContext _context;
        private DataSetService _dataSetService;
        private ApiHelper _apiHelper;
        private readonly UserManager<KofCUser> _userManager;


        //*********************************************************************************
        // 8/25/2024 Tim Philomeno
        // NOTE: the API equivelent is just MemberOffices
        //*********************************************************************************
        public TblCorrMemberOfficesController(DataSetService dataSetService, ApiHelper apiHelper,UserManager<KofCUser> userManager)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper; 
            _userManager = userManager;
        }


        public async Task<ActionResult<IEnumerable<TblCorrMemberOfficeVM>>> CFMDMACD()
        {
            //var _fratyear = await _apiHelper.GetAsync<int>("GetFratYear/0");
            //var results = await _apiHelper.GetAsync<int>($"/ClearDelegates/{_fratyear}");
            //CheckForMissingDelegateMembersAndCreateDelegates
            var myDelegates = await _apiHelper.GetAsync<IEnumerable<TblCorrMemberOfficeVM>>("CheckForMissingDelegateMembersAndCreateDelegates");
            if (myDelegates.Count() == 0)
            {
                
                TempData["Message"] = "Delegate Records have been Added";
                return RedirectToAction("Index", "CvnImpDelegates");
            }
            else
            {
                ViewBag.Message = "The Member Records for these Imported Delegates are missing." + Environment.NewLine + "Please add them and then run the process again.";
                return View("Views/TblCorrMemberOffices/MissingDelegates.cshtml", myDelegates);
            }
            

        }



        // GET: TblCorrMemberOffices
        public async Task<ActionResult<IEnumerable<TblCorrMemberOfficeVM>>> Index(int id)
        {
            try
            {
                Uri myURIMO = new Uri(_dataSetService.GetAPIBaseAddress() + "/MemberOffice/" + id.ToString());
                IEnumerable<TblCorrMemberOfficeVM> CMO;
                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(myURI);
                    var responseTask = client.GetAsync(myURIMO);
                    responseTask.Wait();
                    var result = responseTask.Result;

                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsAsync<IList<TblCorrMemberOfficeVM>>();
                        readTask.Wait();
                        CMO = readTask.Result;
                    }
                    else
                    {
                        CMO = Enumerable.Empty<TblCorrMemberOfficeVM>();
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    }
                }

                ViewBag.MemberID = id;

                // Setup to get the mameber name
                Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetMemberName/" + id.ToString());

                using (var client = new HttpClient())
                {
                    //client.BaseAddress = new Uri(myURI);
                    var responseTask = client.GetAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    if (result.IsSuccessStatusCode)
                    {
                        var readTask = result.Content.ReadAsStringAsync();
                        readTask.Wait();
                        ViewBag.MemberName = readTask.Result.ToString();
                    }
                    else
                    {
                        ViewBag.MemberName = "Member Not Found";
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    }
                }

                return View(CMO);
            }
            catch (Exception ex)
            {
                Log.Error(string.Concat("Error getting Member Name from ", id.ToString(), " - ", ex.Message, ex.InnerException));
                throw new Exception("Error getting Member Name");
            }
        }

        // GET: TblCorrMemberOffices/Details/5
        //******************************************************************************
        // 8/24/2024 Tim Philomeno
        // in our current application Details never gets called.  Only Index, Create and Delete
        //******************************************************************************
        public async Task<TblCorrMemberOffice> Details(int? id)
        {
            if (id == null)
            {
                return null;
            }

            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/MemberOffice/Details/" + id.ToString());

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblCorrMemberOffice? CMO;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    CMO = JsonConvert.DeserializeObject<TblCorrMemberOffice>(json);
                }
                else
                {
                    CMO = null;
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return CMO;
            }
        }

        // GET: TblCorrMemberOffices/Create
        public IActionResult Create(int id)
        {
            //******************************************************************************
            // 8/24/2024 Tim Philomeno
            // in our current application this Create gets called to fill the Offices List
            //******************************************************************************
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/Offices");
            IEnumerable<TblValOffice> offices;
            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;

                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblValOffice>>();
                    readTask.Wait();
                    offices = readTask.Result;
                }
                else
                {
                    offices = Enumerable.Empty<TblValOffice>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                ViewBag.MemberID = id;
                ViewBag.ListOfOffices = new SelectList(offices.OrderBy(x => x.OfficeDescription).ToList(), "OfficeId", "OfficeDescription");
                // allow the default frat year to be next year if we are in April, May or June
                int myY = (DateTime.Now.Month >= 4 && DateTime.Now.Month <= 6) ? 1:0;
                ViewBag.FratYear = GetFratYear(myY).Result;
            }
            return View();
        }

        // POST: TblCorrMemberOffices/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //******************************************************************************
        // 8/24/2024 Tim Philomeno
        //******************************************************************************
        public async Task<IActionResult> Create([Bind("MemberId,OfficeId,PrimaryOffice,Year,Council,Assembly,District")] TblCorrMemberOffice tblCorrMemberOffice)
        {
            if (ModelState.IsValid)
            {
                tblCorrMemberOffice.Updated = DateTime.Now;
                var userId = User.Identity.Name;
                var user = await _userManager.FindByIdAsync(userId);
                tblCorrMemberOffice.UpdatedBy = await Utils.GetUserProp<int>(User, _userManager, "KofCMemberID");
                var results = await _apiHelper.PostAsync<TblCorrMemberOffice, TblCorrMemberOffice>("MemberOffice",tblCorrMemberOffice);
            }
            return RedirectToAction(nameof(Index), new { id = tblCorrMemberOffice.MemberId });
            //return View(tblCorrMemberOffice);
        }

        // GET: TblCorrMemberOffices/Edit/5
        //***********************************************************************************************
        // 8/25/2024 Tim Philomeno
        // we will never call edit on this table
        //***********************************************************************************************
        public async Task<IActionResult> Edit(int? id)
        {
            return NotFound();
            ////////////if (id == null)
            ////////////{
            ////////////    return NotFound();
            ////////////}

            ////////////List<TblValOffice> officeList = new List<TblValOffice>();
            ////////////ViewBag.ListOfOffices = new SelectList(_context.TblValOffices.ToList(), "OfficeId", "OfficeDescription");

            ////////////var tblCorrMemberOffice = await _context.TblCorrMemberOffices.FindAsync(id);
            ////////////if (tblCorrMemberOffice == null)
            ////////////{
            ////////////    return NotFound();
            ////////////}
            ////////////return View(tblCorrMemberOffice);
        }

        // POST: TblCorrMemberOffices/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        //***********************************************************************************************
        // 8/25/2024 Tim Philomeno
        // we will never call edit on this table
        //***********************************************************************************************
        public async Task<IActionResult> Edit(int id, [Bind("Id,MemberId,OfficeId,PrimaryOffice,Year,Council,Assembly,District")] TblCorrMemberOffice tblCorrMemberOffice)
        {
            return View(tblCorrMemberOffice);
            ////////////if (id != tblCorrMemberOffice.Id)
            ////////////{
            ////////////    return NotFound();
            ////////////}

            ////////////if (ModelState.IsValid)
            ////////////{
            ////////////    try
            ////////////    {
            ////////////        _context.Update(tblCorrMemberOffice);
            ////////////        await _context.SaveChangesAsync();
            ////////////    }
            ////////////    catch (DbUpdateConcurrencyException)
            ////////////    {
            ////////////        if (!TblCorrMemberOfficeExists(tblCorrMemberOffice.Id))
            ////////////        {
            ////////////            return NotFound();
            ////////////        }
            ////////////        else
            ////////////        {
            ////////////            throw;
            ////////////        }
            ////////////    }
            ////////////    return RedirectToAction(nameof(Index), new { id = tblCorrMemberOffice.MemberId });
            ////////////}
            ////////////return View(tblCorrMemberOffice);
        }

        // GET: TblCorrMemberOffices/Delete/5
        //**********************************************************************************************
        // 8/25/2024 Tim Philomeno
        // this delete runs and gets the details and presents them in a view to then be deleted
        //**********************************************************************************************
        public async Task<IActionResult> Delete(int? id,int MemberId)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/MemberOffice/Details/" + id);
            TblCorrMemberOffice? CMO;
            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    CMO = JsonConvert.DeserializeObject<TblCorrMemberOffice>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    CMO = null;
                }
            }
                return View(CMO);
        }

        // POST: TblCorrMemberOffices/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        //**********************************************************************************************
        // 8/25/2024 Tim Philomeno
        // this delete runs the actual delete
        //**********************************************************************************************
        public async Task<IActionResult> DeleteConfirmed(int id,int MemberId)
        {
            try
            {
                var myCMO = Details(id);
                
                Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/MemberOffice/" + id);
                using (var client = new HttpClient())
                {
                    var responseTask = client.DeleteAsync(myURI);
                    responseTask.Wait();
                    var result = responseTask.Result;
                    TblCorrMemberOffice? CMO;
                    if (result.IsSuccessStatusCode)
                    {
                        Log.Information("Delete MemberOffice Success " + id);
                        string json = await result.Content.ReadAsStringAsync();
                        CMO = JsonConvert.DeserializeObject<TblCorrMemberOffice>(json);
                    }
                    else
                    {
                        Log.Information("Delete MemberOffice Failed " + id);
                        ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                        CMO = null;
                    }
                    return RedirectToAction(nameof(Index), new { id = myCMO.Result.MemberId });
                }
            }
            catch (Exception ex)
            {

                Log.Fatal(ex.Message + " " + ex.InnerException);
                return NoContent();
            }
        }

        private bool TblCorrMemberOfficeExists(int id)
        {
            return true; // _context.TblCorrMemberOffices.Any(e => e.Id == id);
        }
        private async Task<string> GetFratYear(int NextYear)
        {
            Uri myURI = new Uri(_dataSetService.GetAPIBaseAddress() + "/GetFratYear/" + NextYear.ToString());

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                string? FY;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    FY = json;
                }
                else
                {
                    FY = null;
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return FY;
            }
        }
    }
}
