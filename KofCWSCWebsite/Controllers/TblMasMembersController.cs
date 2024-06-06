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
using com.sun.xml.@internal.bind.v2.model.core;

namespace KofCWSCWebsite.Controllers
{
    public class TblMasMembersController : Controller
    {
        private readonly ApplicationDbContext _context;

        private string myBaseAddress = "https://dev.kofc-wa.org/API";
        //private string myBaseAddress = "https://localhost:7078";
        public TblMasMembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        public ActionResult Index(string lastname)
        {
            IEnumerable<TblMasMember> members = null;

            if (lastname.IsNullOrEmpty())
            {
                lastname = "aaa";
            }
            Uri myURI = new Uri(myBaseAddress + "/GetMembers/ByLastName/" + lastname);

            using (var client = new HttpClient())
            {
                //client.BaseAddress = new Uri(myURI);
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                if (result.IsSuccessStatusCode)
                {
                    var readTask = result.Content.ReadAsAsync<IList<TblMasMember>>();
                    readTask.Wait();
                    members = readTask.Result;
                }
                else
                {
                    members = Enumerable.Empty<TblMasMember>();
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                }
                return View(members);
            }
        }

        // GET: TblMasMembers/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(myBaseAddress + "/GetMember/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasMember? member;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    member = JsonConvert.DeserializeObject<TblMasMember>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    member = null;
                }
                return View(member);
            }
        }

        // GET: TblMasMembers/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblMasMembers/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("MemberId,KofCid,Prefix,PrefixUpdated,PrefixUpdatedBy,FirstName,FirstNameUpdated,FirstNameUpdatedBy,NickName,NickNameUpdated,NickNameUpdatedBy,Mi,Miupdated,MiupdatedBy,LastName,LastNameUpdated,LastNameUpdatedBy,Suffix,SuffixUpdated,SuffixUpdatedBy,AddInfo1,AddInfo1Updated,AddInfo1UpdatedBy,Address,AddressUpdated,AddressUpdatedBy,City,CityUpdated,CityUpdatedBy,State,StateUpdated,StateUpdatedBy,PostalCode,PostalCodeUpdated,PostalCodeUpdatedBy,Phone,PhoneUpdated,PhoneUpdatedBy,WifesName,WifesNameUpdated,WifesNameUpdatedBy,AddInfo2,AddInfo2Updated,AddInfo2UpdatedBy,FaxNumber,FaxNumberUpdated,FaxNumberUpdatedBy,Council,CouncilUpdated,CouncilUpdatedBy,Assembly,AssemblyUpdated,AssemblyUpdatedBy,Circle,CircleUpdated,CircleUpdatedBy,Email,EmailUpdated,EmailUpdatedBy,Deceased,DeceasedUpdated,DeceasedUpdatedBy,CellPhone,CellPhoneUpdated,CellPhoneUpdatedBy,LastUpdated,SeatedDelegateDay1,SeatedDelegateDay2,SeatedDelegateDay3,PaidMpd,Bulletin,BulletinUpdated,BulletinUpdatedBy,UserId,Data,DataChanged,LastLoggedIn,CanEditAdmUi,DoNotEmail,HidePersonalInfo,WhyDoNotEmail")] TblMasMember tblMasMember)
        {
            if (ModelState.IsValid)
            {
                Uri myURI = new(myBaseAddress + "/NewMember");
                string lastname = tblMasMember.LastName;
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PostAsJsonAsync(myURI, tblMasMember);
                        var returnValue = await response.Content.ReadAsAsync<List<TblMasMember>>();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMasMemberExists(tblMasMember.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index), new { lastname = lastname });
            }
            return View(tblMasMember);
        }

        // GET: TblMasMembers/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(myBaseAddress + "/GetMember/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasMember? member;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    member = JsonConvert.DeserializeObject<TblMasMember>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    member = null;
                }
                return View(member);
            }
        }

        // POST: TblMasMembers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("MemberId,KofCid,Prefix,PrefixUpdated,PrefixUpdatedBy,FirstName,FirstNameUpdated,FirstNameUpdatedBy,NickName,NickNameUpdated,NickNameUpdatedBy,Mi,Miupdated,MiupdatedBy,LastName,LastNameUpdated,LastNameUpdatedBy,Suffix,SuffixUpdated,SuffixUpdatedBy,AddInfo1,AddInfo1Updated,AddInfo1UpdatedBy,Address,AddressUpdated,AddressUpdatedBy,City,CityUpdated,CityUpdatedBy,State,StateUpdated,StateUpdatedBy,PostalCode,PostalCodeUpdated,PostalCodeUpdatedBy,Phone,PhoneUpdated,PhoneUpdatedBy,WifesName,WifesNameUpdated,WifesNameUpdatedBy,AddInfo2,AddInfo2Updated,AddInfo2UpdatedBy,FaxNumber,FaxNumberUpdated,FaxNumberUpdatedBy,Council,CouncilUpdated,CouncilUpdatedBy,Assembly,AssemblyUpdated,AssemblyUpdatedBy,Circle,CircleUpdated,CircleUpdatedBy,Email,EmailUpdated,EmailUpdatedBy,Deceased,DeceasedUpdated,DeceasedUpdatedBy,CellPhone,CellPhoneUpdated,CellPhoneUpdatedBy,LastUpdated,SeatedDelegateDay1,SeatedDelegateDay2,SeatedDelegateDay3,PaidMpd,Bulletin,BulletinUpdated,BulletinUpdatedBy,UserId,Data,DataChanged,LastLoggedIn,CanEditAdmUi,DoNotEmail,HidePersonalInfo,WhyDoNotEmail")] TblMasMember tblMasMember)
        {
            if (id != tblMasMember.MemberId)
            {
                return NotFound();
            }
            // for return
            string lastname = tblMasMember.LastName;
            if (ModelState.IsValid)
            {
                Uri myURI = new(myBaseAddress + "/UpdMember/" + id);
                try
                {
                    using (var client = new HttpClient())
                    {
                        client.BaseAddress = myURI;
                        var response = await client.PutAsJsonAsync(myURI, tblMasMember);
                        var returnValue = await response.Content.ReadAsAsync<List<TblMasMember>>();
                    }
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!TblMasMemberExists(tblMasMember.MemberId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index),new { lastname = lastname });
            }
            return View(tblMasMember);
        }

        // GET: TblMasMembers/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(myBaseAddress + "/GetMember/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasMember? member;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    member = JsonConvert.DeserializeObject<TblMasMember>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    member = null;
                }
                return View(member);
            }
        }

        // POST: TblMasMembers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (id == null)
            {
                return NotFound();
            }

            Uri myURI = new(myBaseAddress + "/DelMember/" + id);

            using (var client = new HttpClient())
            {
                var responseTask = client.GetAsync(myURI);
                responseTask.Wait();
                var result = responseTask.Result;
                TblMasMember? member;
                if (result.IsSuccessStatusCode)
                {
                    string json = await result.Content.ReadAsStringAsync();
                    member = JsonConvert.DeserializeObject<TblMasMember>(json);
                }
                else
                {
                    ModelState.AddModelError(string.Empty, "Server Error.  Please contact administrator.");
                    member = null;
                }
                return RedirectToAction(nameof(Index));
            }
        }

        private bool TblMasMemberExists(int id)
        {
            return _context.TblMasMembers.Any(e => e.MemberId == id);
        }
    }
}
