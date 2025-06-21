using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using Serilog;
using Microsoft.AspNetCore.Authorization;
//using Microsoft.AspNet.Identity;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using KofCWSCWebsite.Services;
using System.Text.Encodings.Web;

namespace KofCWSCWebsite.Controllers
{
    [Authorize(Roles = "Admin")]
    public class AspNetUsersController : Controller
    {
        private readonly ApiHelper _apiHelper;
        private readonly UserManager<KofCUser> _userManager;
        private readonly ISenderEmail _emailSender;
        public AspNetUsersController(ApiHelper apiHelper, UserManager<KofCUser> userManager, ISenderEmail senderEmail)
        {
            _apiHelper = apiHelper;
            _userManager = userManager;
            _emailSender = senderEmail;
        }

        // GET: AspNetUsers

        public async Task<IActionResult> Index()
        {
            var myAspNetUsers = await _apiHelper.GetAsync<IEnumerable<AspNetUser>>("AspNetUsers");
            return View(myAspNetUsers);
            //return View(await _context.AspNetUsers.ToListAsync());
        }

        // GET: AspNetUsers/Details/5

        public async Task<IActionResult> Details(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var aspNetUser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");

            //var aspNetUser = await _context.AspNetUsers
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        //////////// GET: AspNetUsers/Create
        //////////public IActionResult Create()
        //////////{
        //////////    return View();
        //////////}

        //////////// POST: AspNetUsers/Create
        //////////// To protect from overposting attacks, enable the specific properties you want to bind to.
        //////////// For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        //////////[HttpPost]
        //////////[ValidateAntiForgeryToken]
        //////////public async Task<IActionResult> Create([Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,KofCmemberId,LastName,ProfilePictureUrl,Address,City,State,PostalCode,Wife,Council,MemberVerified,MemberhipCardUrl")] AspNetUser aspNetUser)
        //////////{
        //////////    if (ModelState.IsValid)
        //////////    {
        //////////        _context.Add(aspNetUser);
        //////////        await _context.SaveChangesAsync();
        //////////        return RedirectToAction(nameof(Index));
        //////////    }
        //////////    return View(aspNetUser);
        //////////}

        // GET: AspNetUsers/Edit/5
        public async Task<IActionResult> Edit(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            var aspNetUser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        public async Task<IActionResult> ApproveNewMember(string id)
        {
            ////////////---------------------------------------------------------------------------
            //////////// First get the data
            //////////var aspnetuser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");
            ////////////---------------------------------------------------------------------------
            //////////// Add the data to tbl_MasMembers
            //////////var tblmasmember = new TblMasMember
            //////////{
            //////////    FirstName = aspnetuser.FirstName,
            //////////    LastName = aspnetuser.LastName,
            //////////    Email = aspnetuser.Email,
            //////////    Address = aspnetuser.Address,
            //////////    City = aspnetuser.City,
            //////////    State = aspnetuser.State,
            //////////    PostalCode = aspnetuser.PostalCode,
            //////////    KofCid = aspnetuser.KofCmemberId,
            //////////    Council = (int)aspnetuser.Council
            //////////};
            //////////var results = await _apiHelper.PostAsync<TblMasMember, TblMasMember>("Member", tblmasmember);
            ////////////---------------------------------------------------------------------------
            //////////// Set MemberVerfied to 1 in AspNetUsers
            //////////aspnetuser.MemberVerified = true;
            //////////var results1 = await _apiHelper.PutAsync<AspNetUser, AspNetUser>($"AspNetUsers/{id}", aspnetuser);
            ////////////---------------------------------------------------------------------------
            //////////// Add the member to the MEMBER role
            //////////var user = await _userManager.FindByNameAsync(aspnetuser.UserName);
            //////////await _userManager.AddToRoleAsync(user, "Member");
            //////////// Send confirmation email to the new Member
            //////////await _emailSender.SendEmailAsync(aspnetuser.Email, "Your KofC WSC Registration is complete.",
            //////////           $"Your registration has been verified and approved. You may now login and use the site as a Member<br /><br />" +
            //////////           $"If you have any questions please email support@kofc-wa.org.");
            ////////////---------------------------------------------------------------------------
            return View();
        }
        public async Task<IActionResult> RejectNewMember(string id,string reason)
        {
            // Get the profile information
            var aspnetuser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");
            // set the MemberVerified = 0
            // Email the member with the reason
            ViewBag.Reason = reason;
            ViewBag.Email = aspnetuser.Email;
            return View();
        }
        public async Task<IActionResult> GetNewMemberApproval(string id)
        {
            if (id == null)
            {
                return NotFound();
            }

            //var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            var aspNetUser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");
            if (aspNetUser == null)
            {
                return NotFound();
            }
            return View(aspNetUser);
        }

        // POST: AspNetUsers/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(string id, [Bind("Id,UserName,NormalizedUserName,Email,NormalizedEmail,EmailConfirmed,PasswordHash,SecurityStamp,ConcurrencyStamp,PhoneNumber,PhoneNumberConfirmed,TwoFactorEnabled,LockoutEnd,LockoutEnabled,AccessFailedCount,FirstName,KofCmemberId,LastName,ProfilePictureUrl,Address,City,State,PostalCode,Wife,Council,MemberVerified,MembershipCardUrl")] AspNetUser aspNetUser)
        {
            if (id != aspNetUser.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    await _apiHelper.PutAsync<AspNetUser, AspNetUser>($"AspNetUsers/{id}", aspNetUser);
                    //_context.Update(aspNetUser);
                    //await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException ex)
                {
                    Log.Error(ex.Message, ex);
                }
                return RedirectToAction(nameof(Index));
            }
            return View(aspNetUser);
        }

        // GET: AspNetUsers/Delete/5
        public async Task<IActionResult> Delete(string id)
        {
            if (id == null)
            {
                return NotFound();
            }
            var aspNetUser = await _apiHelper.GetAsync<AspNetUser>($"AspNetUsers/{id}");
            //var aspNetUser = await _context.AspNetUsers
            //    .FirstOrDefaultAsync(m => m.Id == id);
            if (aspNetUser == null)
            {
                return NotFound();
            }

            return View(aspNetUser);
        }

        //POST: AspNetUsers/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            //var aspNetUser = await _context.AspNetUsers.FindAsync(id);
            //if (aspNetUser != null)
            //{
            //    _context.AspNetUsers.Remove(aspNetUser);
            //}

            //await _context.SaveChangesAsync();
            await _apiHelper.DeleteAsync($"AspNetUsers/{id}");
            return RedirectToAction(nameof(Index));
        }

        //private bool AspNetUserExists(string id)
        //{
        //    return _context.AspNetUsers.Any(e => e.Id == id);
        //}
    }
}
