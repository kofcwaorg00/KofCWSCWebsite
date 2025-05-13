using Microsoft.AspNetCore.Mvc;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using KofCWSCWebsite.Services;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;

namespace KofCWSCWebsite.Controllers
{
    public class TblMasMembersController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        private readonly UserManager<KofCUser> _userManager;

        public TblMasMembersController(DataSetService dataSetService, ApiHelper apiHelper,UserManager<KofCUser> userManager)
        {
            Log.Information("Creating MembersController");
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
            _userManager = userManager;
        }

        [Authorize(Roles = "Admin,StateOfficer,DataAdmin,StateMembership")]
        public async Task<ActionResult> FormatMemberData(int id)
        {
            var results = await _apiHelper.GetAsync<string>($"/Members/FormatData/{id}");

            return RedirectToAction(nameof(Index), new { lastname = results });
        }

            [Authorize(Roles = "Admin,StateOfficer,DataAdmin,StateMembership")]
        public async Task<ActionResult> Index(string lastname)
        {
            //********************************************************************************
            // 11/1/2024 Tim Philomeno
            // create a session cookie, the MinValue does the trick
            HttpContext.Response.Cookies.Append("IAgreeSensitive", "true", new CookieOptions
            {
                //Expires = DateTime.MinValue,// DateTimeOffset.UtcNow.AddMinutes(30),
                Path = "/",
                HttpOnly = false, // Accessible only by the server
                IsEssential = true // Required for GDPR compliance
            });
            //----------------------------------------------------------------------------------
            Log.Information("Starting index with Lastname " + lastname);
            if (lastname.IsNullOrEmpty())
            {
                lastname = "aaa";
            }
            string myEndpoint = "";

            bool isNumber = int.TryParse(lastname, out var KofCID);
            //IEnumerable<TblMasMember> members;
            if (isNumber)
            {
                myEndpoint = $"Members/KofCID/{KofCID}";
            }
            else
            {
                myEndpoint = $"Members/LastName/{lastname}";
            }
            IEnumerable<TblMasMember> members;

            members = await _apiHelper.GetAsync<IEnumerable<TblMasMember>>(myEndpoint);

            //ViewData["NoMembers"] = "Found " + members.Count() + " Members";

            if (members is null)
            {
                members = Enumerable.Empty<TblMasMember>();
                ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
            }
            else
            {
                ViewData["NoMembers"] = "Found " + members.Count() + " Members";
            }

            return View(members);

        }

        // GET: TblMasMembers/Details/5
        // you have to be logged in to get details from various other pages
        [Authorize]
        public async Task<IActionResult> Details(int? id)
        {
            Log.Information("Starting Details of " + id);
            if (id == null)
            {
                return NotFound();
            }
            var member = await _apiHelper.GetAsync<TblMasMember>($"/Member/{id}");

            return View(member);
        }

        // GET: TblMasMembers/Create
        [Authorize(Roles = "Admin,DataAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,KofCid,Prefix,PrefixUpdated,PrefixUpdatedBy,FirstName,FirstNameUpdated,FirstNameUpdatedBy,NickName,NickNameUpdated,NickNameUpdatedBy,Mi,Miupdated,MiupdatedBy,LastName,LastNameUpdated,LastNameUpdatedBy,Suffix,SuffixUpdated,SuffixUpdatedBy,AddInfo1,AddInfo1Updated,AddInfo1UpdatedBy,Address,AddressUpdated,AddressUpdatedBy,City,CityUpdated,CityUpdatedBy,State,StateUpdated,StateUpdatedBy,PostalCode,PostalCodeUpdated,PostalCodeUpdatedBy,Phone,PhoneUpdated,PhoneUpdatedBy,WifesName,WifesNameUpdated,WifesNameUpdatedBy,AddInfo2,AddInfo2Updated,AddInfo2UpdatedBy,FaxNumber,FaxNumberUpdated,FaxNumberUpdatedBy,Council,CouncilUpdated,CouncilUpdatedBy,Assembly,AssemblyUpdated,AssemblyUpdatedBy,Circle,CircleUpdated,CircleUpdatedBy,Email,EmailUpdated,EmailUpdatedBy,Deceased,DeceasedUpdated,DeceasedUpdatedBy,CellPhone,CellPhoneUpdated,CellPhoneUpdatedBy,LastUpdated,SeatedDelegateDay1,SeatedDelegateDay2,SeatedDelegateDay3,PaidMpd,Bulletin,BulletinUpdated,BulletinUpdatedBy,UserId,Data,DataChanged,LastLoggedIn,CanEditAdmUi,DoNotEmail,HidePersonalInfo,WhyDoNotEmail")] TblMasMember tblMasMember)
        {
            if (ModelState.IsValid)
            {
                // format the phone number
                tblMasMember.Phone = Utils.FormatPhoneNumber(tblMasMember.Phone);
                // Save the new member
                var results = await _apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", tblMasMember);

                return RedirectToAction(nameof(Index), new { lastname = tblMasMember.LastName });
            }
            return View(tblMasMember);
        }
        [Authorize(Roles = "Admin,DataAdmin,ConventionAdmin")]
        [HttpPost("AddOrUpdateFromDelImp")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddOrUpdateFromDelImp(CvnImpDelegate cvnImpDelegate,string action,int Id)
        {
            string[] myactionin = action.Split(" ");
            string myaction = myactionin[0];
            string mydel = myactionin[1];
            TblMasMember tblMasMember = null;
            if (myaction == "Add")
            {
                switch(mydel){
                    case "D1":
                        Utils.FillD1(ref tblMasMember, cvnImpDelegate);
                        await _apiHelper.PostAsync<TblMasMember, TblMasMember>($"Member", tblMasMember);
                        break;
                    case "D2":
                        Utils.FillD2(ref tblMasMember, cvnImpDelegate);
                        await _apiHelper.PostAsync<TblMasMember, TblMasMember>($"Member", tblMasMember);
                        break;
                    case "A1":
                        Utils.FillA1(ref tblMasMember, cvnImpDelegate);
                        await _apiHelper.PostAsync<TblMasMember, TblMasMember>($"Member", tblMasMember);
                        break;
                    case "A2":
                        Utils.FillA2(ref tblMasMember, cvnImpDelegate);
                        await _apiHelper.PostAsync<TblMasMember, TblMasMember>($"Member", tblMasMember);
                        break;
                }
            }
            else if (myaction == "Update")
            {
                switch (mydel)
                {
                    case "D1":
                        tblMasMember = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{cvnImpDelegate.D1MemberID}");
                        Utils.FillD1( ref tblMasMember, cvnImpDelegate);
                        CvnImpDelegateIMP cvnImpDelegateIMPD1 = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}");
                        Utils.FillD1ImpDel(ref cvnImpDelegateIMPD1, cvnImpDelegate);
                        await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"Member/{tblMasMember.MemberId}", tblMasMember);
                        await _apiHelper.PutAsync<CvnImpDelegateIMP, CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}", cvnImpDelegateIMPD1);
                        break;
                    case "D2":
                        tblMasMember = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{cvnImpDelegate.D2MemberID}");
                        Utils.FillD2(ref tblMasMember, cvnImpDelegate);
                        CvnImpDelegateIMP cvnImpDelegateIMPD2 = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}");
                        Utils.FillD2ImpDel(ref cvnImpDelegateIMPD2, cvnImpDelegate);
                        await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"Member/{tblMasMember.MemberId}", tblMasMember);
                        await _apiHelper.PutAsync<CvnImpDelegateIMP, CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}", cvnImpDelegateIMPD2);
                        break;
                    case "A1":
                        tblMasMember = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{cvnImpDelegate.A1MemberID}");
                        Utils.FillA1(ref tblMasMember, cvnImpDelegate);
                        CvnImpDelegateIMP cvnImpDelegateIMPA1 = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}");
                        Utils.FillA1ImpDel(ref cvnImpDelegateIMPA1, cvnImpDelegate);
                        await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"Member/{tblMasMember.MemberId}", tblMasMember);
                        await _apiHelper.PutAsync<CvnImpDelegateIMP, CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}", cvnImpDelegateIMPA1);
                        break;
                    case "A2":
                        tblMasMember = await _apiHelper.GetAsync<TblMasMember>($"Member/KofCID/{cvnImpDelegate.A2MemberID}");
                        Utils.FillA2(ref tblMasMember, cvnImpDelegate);
                        CvnImpDelegateIMP cvnImpDelegateIMPA2 = await _apiHelper.GetAsync<CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}");
                        Utils.FillA2ImpDel(ref cvnImpDelegateIMPA2, cvnImpDelegate);
                        await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"Member/{tblMasMember.MemberId}", tblMasMember);
                        await _apiHelper.PutAsync<CvnImpDelegateIMP, CvnImpDelegateIMP>($"CvnImpDelegate/{cvnImpDelegate.Id}", cvnImpDelegateIMPA2);
                        break;
                }
            }
            else
            {
                return BadRequest("No action defined");
            }

            return RedirectToAction("Index", "CvnImpDelegates");
        }
        

        // GET: TblMasMembers/Edit/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var member = await _apiHelper.GetAsync<TblMasMember>($"/Member/{id}");

            return View(member);
        }

        // POST: TblMasMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,KofCid,Prefix,PrefixUpdated,PrefixUpdatedBy,FirstName,FirstNameUpdated,FirstNameUpdatedBy,NickName,NickNameUpdated,NickNameUpdatedBy,Mi,Miupdated,MiupdatedBy,LastName,LastNameUpdated,LastNameUpdatedBy,Suffix,SuffixUpdated,SuffixUpdatedBy,AddInfo1,AddInfo1Updated,AddInfo1UpdatedBy,Address,AddressUpdated,AddressUpdatedBy,City,CityUpdated,CityUpdatedBy,State,StateUpdated,StateUpdatedBy,PostalCode,PostalCodeUpdated,PostalCodeUpdatedBy,Phone,PhoneUpdated,PhoneUpdatedBy,WifesName,WifesNameUpdated,WifesNameUpdatedBy,AddInfo2,AddInfo2Updated,AddInfo2UpdatedBy,FaxNumber,FaxNumberUpdated,FaxNumberUpdatedBy,Council,CouncilUpdated,CouncilUpdatedBy,Assembly,AssemblyUpdated,AssemblyUpdatedBy,Circle,CircleUpdated,CircleUpdatedBy,Email,EmailUpdated,EmailUpdatedBy,Deceased,DeceasedUpdated,DeceasedUpdatedBy,CellPhone,CellPhoneUpdated,CellPhoneUpdatedBy,LastUpdated,SeatedDelegateDay1,SeatedDelegateDay2,SeatedDelegateDay3,PaidMpd,Bulletin,BulletinUpdated,BulletinUpdatedBy,UserId,Data,DataChanged,LastLoggedIn,CanEditAdmUi,DoNotEmail,HidePersonalInfo,WhyDoNotEmail")] TblMasMember tblMasMember)
        {
            if (id != tblMasMember.MemberId)
            {
                Log.Fatal("Member ID not found " + id);
                return NotFound();
            }
            // for return
            string lastname = tblMasMember.LastName;
            
            if (ModelState.IsValid)
            {
                // validate phone number
                tblMasMember.Phone = Utils.FormatPhoneNumber(tblMasMember.Phone);
                tblMasMember.LastUpdated = DateTime.Now;

                tblMasMember.LastUpdated = DateTime.Now;
                var userId = User.Identity.Name;
                var user = await _userManager.FindByIdAsync(userId);
                tblMasMember.LastUpdatedBy = await Utils.GetUserProp<int>(User, _userManager, "KofCMemberID");

                // update the record
                var results = await _apiHelper.PutAsync<TblMasMember, TblMasMember>($"/Member/{id}", tblMasMember);

                return RedirectToAction(nameof(Index), new { lastname = lastname });
            }
            return View(tblMasMember);
        }

        // GET: TblMasMembers/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            Log.Information("Starting Delete " + id);
            if (id == null)
            {
                return NotFound();
            }
            var member = await _apiHelper.GetAsync<TblMasMember>($"/Member/{id}");

            return View(member);
        }

        // POST: TblMasMembers/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            Log.Information("Starting DeleteConfirmed of " + id);
            if (id == null)
            {
                Log.Fatal("Delect Member Not found " + id);
                return NotFound();
            }

            //            Uri myURI = new(_dataSetService.GetAPIBaseAddress() + "/Member/" + id);
            try
            {
                await _apiHelper.DeleteAsync($"/Member/{id}");

                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                Log.Fatal(ex.Message + " " + ex.InnerException);
                return NoContent();
            }
        }
        //private void CopyMemberFromImpModel(CvnImpDelegate cvnImpDelegate,ref TblMasMember tblMasMember, string del,int memberid = 0)
        //{
        //    string myUpd = "Updated by Delegate Import";
        //    DateTime myDate = DateTime.Now;
        //    if (del == "D1")
        //    {
        //        if (memberid != 0) { tblMasMember.MemberId = memberid; }
        //        tblMasMember.KofCid = (int)cvnImpDelegate.D1MemberID;

        //        tblMasMember.FirstName = cvnImpDelegate.D1FirstName;
        //        tblMasMember.FirstNameUpdated = myDate;
        //        tblMasMember.FirstNameUpdatedBy = myUpd;
                
        //        tblMasMember.Mi = cvnImpDelegate.D1MiddleName;
        //        tblMasMember.Miupdated = myDate;
        //        tblMasMember.MiupdatedBy = myUpd;
                
        //        tblMasMember.LastName = cvnImpDelegate.D1LastName;
        //        tblMasMember.LastNameUpdated = myDate;
        //        tblMasMember.LastNameUpdatedBy = myUpd;

        //        tblMasMember.Suffix = cvnImpDelegate.D1Suffix;
        //        tblMasMember.SuffixUpdated = myDate;
        //        tblMasMember.SuffixUpdatedBy = myUpd;

        //        tblMasMember.Address = cvnImpDelegate.D1Address1;
        //        tblMasMember.AddressUpdated = myDate;
        //        tblMasMember.AddressUpdatedBy = myUpd;

        //        tblMasMember.City = cvnImpDelegate.D1City;
        //        tblMasMember.CityUpdated = myDate;
        //        tblMasMember.CityUpdatedBy = myUpd;

        //        tblMasMember.State = cvnImpDelegate.D1State;
        //        tblMasMember.StateUpdated = myDate;
        //        tblMasMember.StateUpdatedBy = myUpd;

        //        tblMasMember.PostalCode = cvnImpDelegate.D1ZipCode;
        //        tblMasMember.PostalCodeUpdated = myDate;
        //        tblMasMember.PostalCodeUpdatedBy = myUpd;

        //        tblMasMember.Phone = cvnImpDelegate.D1Phone;
        //        tblMasMember.PhoneUpdated = myDate;
        //        tblMasMember.PhoneUpdatedBy = myUpd;

        //        tblMasMember.Email = cvnImpDelegate.D1Email;
        //        tblMasMember.EmailUpdated = myDate;
        //        tblMasMember.EmailUpdatedBy = myUpd;
        //    }
        //    if (del == "D2")
        //    {
        //        if (memberid != 0) { tblMasMember.MemberId = memberid; }
        //        tblMasMember.KofCid = (int)cvnImpDelegate.D2MemberID;
        //        tblMasMember.FirstName = cvnImpDelegate.D2FirstName;
        //        tblMasMember.Mi = cvnImpDelegate.D2MiddleName;
        //        tblMasMember.LastName = cvnImpDelegate.D2LastName;
        //        tblMasMember.Suffix = cvnImpDelegate.D2Suffix;
        //        tblMasMember.Address = cvnImpDelegate.D2Address1;
        //        tblMasMember.City = cvnImpDelegate.D2City;
        //        tblMasMember.State = cvnImpDelegate.D2State;
        //        tblMasMember.PostalCode = cvnImpDelegate.D2ZipCode;
        //        tblMasMember.Phone = cvnImpDelegate.D2Phone;
        //        tblMasMember.Email = cvnImpDelegate.D2Email;
        //    }
        //    if (del == "A1")
        //    {
        //        if (memberid != 0) { tblMasMember.MemberId = memberid; }
        //        tblMasMember.KofCid = (int)cvnImpDelegate.A1MemberID;
        //        tblMasMember.FirstName = cvnImpDelegate.A1FirstName;
        //        tblMasMember.Mi = cvnImpDelegate.A1MiddleName;
        //        tblMasMember.LastName = cvnImpDelegate.A1LastName;
        //        tblMasMember.Suffix = cvnImpDelegate.A1Suffix;
        //        tblMasMember.Address = cvnImpDelegate.A1Address1;
        //        tblMasMember.City = cvnImpDelegate.A1City;
        //        tblMasMember.State = cvnImpDelegate.A1State;
        //        tblMasMember.PostalCode = cvnImpDelegate.A1ZipCode;
        //        tblMasMember.Phone = cvnImpDelegate.A1Phone;
        //        tblMasMember.Email = cvnImpDelegate.A1Email;
        //    }
        //    if (del == "A2")
        //    {
        //        if (memberid != 0) { tblMasMember.MemberId = memberid; }
        //        tblMasMember.KofCid = (int)cvnImpDelegate.A2MemberID;
        //        tblMasMember.FirstName = cvnImpDelegate.A2FirstName;
        //        tblMasMember.Mi = cvnImpDelegate.A2MiddleName;
        //        tblMasMember.LastName = cvnImpDelegate.A2LastName;
        //        tblMasMember.Suffix = cvnImpDelegate.A2Suffix;
        //        tblMasMember.Address = cvnImpDelegate.A2Address1;
        //        tblMasMember.City = cvnImpDelegate.A2City;
        //        tblMasMember.State = cvnImpDelegate.A2State;
        //        tblMasMember.PostalCode = cvnImpDelegate.A2ZipCode;
        //        tblMasMember.Phone = cvnImpDelegate.A2Phone;
        //        tblMasMember.Email = cvnImpDelegate.A2Email;
        //    }
        //}
    }
}
