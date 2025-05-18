using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity;
using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System.Diagnostics;
using com.sun.xml.@internal.bind.v2.model.core;
using NuGet.Versioning;


namespace KofCWSCWebsite.Controllers
{
    public class AspNetUserRolesController : Controller
    {
        private RoleManager<IdentityRole> roleManager;
        private UserManager<KofCUser> userManager;
        private ApiHelper _apiHelper;
        //private ApplicationDbContext _context;

        public AspNetUserRolesController(ApplicationDbContext context, RoleManager<IdentityRole> roleMgr, UserManager<KofCUser> userMrg, ApiHelper apiHelper)
        {
            roleManager = roleMgr;
            userManager = userMrg;
            _apiHelper = apiHelper;
        }

        // GET: AspNetUserRoles
        public async Task<ActionResult> Index(int id)
        {
            var userid = await userManager.Users.Where(u => u.KofCMemberID == id).FirstOrDefaultAsync();
            if (userid == null)
            {
                return View();
            }
            var myroles = await userManager.GetRolesAsync(userid);
            ViewBag.UserID = userid;
            ViewBag.USERGUID = userid.Id;
            ViewBag.UserName = string.Concat(userid.FirstName, " ", userid.LastName);
            return View(myroles);
        }

        //// GET: AspNetUserRoles/Details/5
        //public async Task<IActionResult> Details(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aspNetUserRole = await _context.AspNetUserRoles
        //        .FirstOrDefaultAsync(m => m.UserId == id);
        //    if (aspNetUserRole == null)
        //    {
        //        return NotFound();
        //    }

        //    return View(aspNetUserRole);
        //}

        // GET: AspNetUserRoles/Create
        public async Task<IActionResult> Create(string userguid)
        {
            var user = await userManager.Users.Where(u => u.Id == userguid).FirstOrDefaultAsync();

            // Get all roles
            var allRoles = roleManager.Roles
                .Select(r => new { r.Id, r.Name })
                .ToList();

            // Get roles already assigned to the user
            var assignedRoles = await userManager.GetRolesAsync(user);

            // Filter out assigned roles
            ViewBag.ListOfRole = allRoles
                .Where(r => !assignedRoles.Contains(r.Name))
                .Select(r => new SelectListItem
                {
                    Value = r.Id, // Assign Role ID as value
                    Text = r.Name // Display Role Name
                }).ToList();

            //ViewBag.ListOfRole = roleManager.Roles
            //                .Select(r => new SelectListItem { Value = r.Id, Text = r.Name })
            //                .ToList();

            ViewBag.USERGUID = userguid;

            ViewBag.KofCMemberID = user.KofCMemberID;
            return View();
        }

        // POST: AspNetUserRoles/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("UserId,RoleId")] AspNetUserRole aspNetUserRole)
        {
            var savedmodel = aspNetUserRole;
            if (ModelState.IsValid)
            {
                try
                {
                    var results = await _apiHelper.PostAsync<AspNetUserRole, AspNetUserRole>("UserRoles",aspNetUserRole);

                    var userid = await userManager.Users.Where(u => u.Id == aspNetUserRole.UserId).FirstOrDefaultAsync();
                    //_context.Add(aspNetUserRole);
                    //await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index), new { id = userid.KofCMemberID });
                }
                catch (Exception ex)
                {
                    var errorModel = new ErrorViewModel
                    {
                        RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier,
                        Message = "You tried to add a duplicate role to this member"
                    };

                    return View("Error", errorModel);
                }
            }
            return View();
        }

        //// GET: AspNetUserRoles/Edit/5
        //public async Task<IActionResult> Edit(string id)
        //{
        //    if (id == null)
        //    {
        //        return NotFound();
        //    }

        //    var aspNetUserRole = await _context.AspNetUserRoles.FindAsync(id);
        //    if (aspNetUserRole == null)
        //    {
        //        return NotFound();
        //    }
        //    return View(aspNetUserRole);
        //}

        // POST: AspNetUserRoles/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        ////////[HttpPost]
        ////////[ValidateAntiForgeryToken]
        ////////public async Task<IActionResult> Edit(string id, [Bind("UserId,RoleId")] AspNetUserRole aspNetUserRole)
        ////////{
        ////////    if (id != aspNetUserRole.UserId)
        ////////    {
        ////////        return NotFound();
        ////////    }

        ////////    if (ModelState.IsValid)
        ////////    {
        ////////        try
        ////////        {
        ////////            _context.Update(aspNetUserRole);
        ////////            await _context.SaveChangesAsync();
        ////////        }
        ////////        catch (DbUpdateConcurrencyException)
        ////////        {
        ////////            if (!AspNetUserRoleExists(aspNetUserRole.UserId))
        ////////            {
        ////////                return NotFound();
        ////////            }
        ////////            else
        ////////            {
        ////////                throw;
        ////////            }
        ////////        }
        ////////        return RedirectToAction(nameof(Index));
        ////////    }
        ////////    return View(aspNetUserRole);
        ////////}

        // GET: AspNetUserRoles/Delete/5
        public async Task<IActionResult> Delete(string id, string role)
        {
            var userid = await userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            var userrole = await roleManager.FindByNameAsync(role);
            //if (userid == null)
            //{
            //    return View();
            //}
            //var myroles = await roleManager.GetRoleIdAsync(role);
            //ViewBag.UserID = userid;
            //ViewBag.USERGUID = userid.Id;
            //ViewBag.UserName = string.Concat(userid.FirstName, " ", userid.LastName);
            //var myrole = myroles.Contains(role);
            //if (myrole)
            //{
            var newRoleModel = new IdentityUserRole<string>
            {
                UserId = userid.Id,
                RoleId = userrole.Id
            };

            //}
            ViewBag.RoleToDelete = role;
            ViewBag.KofCMemberID = userid.KofCMemberID;
            return View(newRoleModel);
        }

        // POST: AspNetUserRoles/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(string RoleId, string UserId)
        {
            //var userid = await userManager.Users.Where(u => u.Id == id).FirstOrDefaultAsync();
            var userid = await userManager.Users.Where(u => u.Id == UserId).FirstOrDefaultAsync();
            await _apiHelper.DeleteAsync($"UserRole/{RoleId}/{UserId}");

            //var aspNetUserRole = await _context.AspNetUserRoles.Where(r => r.RoleId == RoleId && r.UserId == UserId).FirstOrDefaultAsync();
            //if (aspNetUserRole != null)
            //{
            //    _context.AspNetUserRoles.Remove(aspNetUserRole);
            //}

            //await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index), new { id = userid.KofCMemberID });

        }

        //private bool AspNetUserRoleExists(string id)
        //{
        //    return _context.AspNetUserRoles.Any(e => e.UserId == id);
        //}
    }
}
