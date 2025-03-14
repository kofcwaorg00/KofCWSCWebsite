using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using System.Net.Http;
using Microsoft.IdentityModel.Tokens;
using System.Collections;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using KofCWSCWebsite.Services;
using sun.rmi.server;

namespace KofCWSCWebsite.Controllers
{
    public class TblMasMembersController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;

        public TblMasMembersController(DataSetService dataSetService, ApiHelper apiHelper)
        {
            Log.Information("Creating MembersController");
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
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
    }
}
