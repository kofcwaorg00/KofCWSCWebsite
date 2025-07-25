﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Services;
using Newtonsoft.Json;
using Serilog;
using Microsoft.AspNetCore.Authorization;
using com.sun.xml.@internal.bind.v2.model.core;
using com.sun.tools.corba.se.logutil;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNet.Identity;

namespace KofCWSCWebsite.Controllers
{
    public class TblValCouncilsController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;
        private readonly Microsoft.AspNetCore.Identity.UserManager<KofCUser> _userManager;
        
        public TblValCouncilsController(DataSetService dataSetService, ApiHelper apiHelper, Microsoft.AspNetCore.Identity.UserManager<KofCUser> userManager)
        {
            _dataSetService = dataSetService;
            _apiHelper = new ApiHelper(_dataSetService);
            _userManager = userManager;
        }

        // GET: TblValCouncils
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Index()
        {
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // I guess the programmers that created the controller code template didn't think that a GET or INDEX
            // would return any errors.  It either gets some or not so no try/catch here
            var result = await _apiHelper.GetAsync<IEnumerable<TblValCouncil>>("/Councils");
            //------------------------------------------------------------------------------------------------------
            return View(result);
        }
        public async Task<IActionResult> MPDEdit()
        {
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // I guess the programmers that created the controller code template didn't think that a GET or INDEX
            // would return any errors.  It either gets some or not so no try/catch here
            var result = await _apiHelper.GetAsync<List<TblValCouncilMPD>>("/Councils");

            //var model = new TblValCouncilMPD
            //{
            //    Councils = result
            //};
            return View(result.OrderBy(e => e.District).ThenBy(e => e.CNumber).ToList());
        }


        // GET: TblValCouncils/Details/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
            //------------------------------------------------------------------------------------------------------

        }
        [Authorize(Roles = "Admin,CouncilOfficer")]
        public async Task<IActionResult> FSDetails(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");
                return View("FSDetails", result);
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return NotFound();
            }
            //------------------------------------------------------------------------------------------------------

        }

        // GET: TblValCouncils/Create
        [Authorize(Roles = "Admin,DataAdmin")]
        public IActionResult Create()
        {
            return View();
        }

        // POST: TblValCouncils/Create
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime")] TblValCouncil tblValCouncil)
        {
            if (ModelState.IsValid)
            {
                //*****************************************************************************************************
                // 12/05/2024 Tim Philomeno
                // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                // call the API
                // this API will returns the created model with the id.  It seems that the developers who created the
                // controller code templates didn't see any issue with not returing any http response messages.  So
                // should the return from here be to INDEX or the newly created council????
                try
                {
                    var myuser = await _userManager.GetUserAsync(User);
                    tblValCouncil.Updated = DateTime.Now;
                    tblValCouncil.UpdatedBy = myuser.KofCMemberID;

                    var apiHelper = new ApiHelper(_dataSetService);
                    var result = await apiHelper.PostAsync<TblValCouncil, TblValCouncil>("/Council/", tblValCouncil);
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    ModelState.AddModelError(string.Empty, ex.Message);
                    return RedirectToAction(nameof(Index));
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblValCouncils/Edit/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
            //------------------------------------------------------------------------------------------------------
        }
        [Authorize(Roles = "Admin,CouncilOfficer")]
        public async Task<IActionResult> FSEdit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<TblValCouncilFSEdit>($"/FSEditCouncil/{id}");
                if (result == null)
                {
                    // if there is no FS assigned, just send back the council record so we can still edit
                    var noFSresult = await _apiHelper.GetAsync<TblValCouncilFSEdit>($"/Council/{id}");
                    return View("FSEdit",noFSresult);
                    //return Ok($"Can't Retrieve FS Address, FS missing for council {id}");
                }
                return View("FSEdit", result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
            //------------------------------------------------------------------------------------------------------
        }

        [Authorize(Roles = "Admin,CouncilOfficer")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> FSEdit(int id, [Bind("CNumber,CLocation,CName,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime")] TblValCouncil tblValCouncil)
        public async Task<IActionResult> FSEdit(int id,TblValCouncilFSEdit tblValCouncilFSEdit)
        {
            if (id != tblValCouncilFSEdit.CNumber)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //*****************************************************************************************************
                // 1/29/2024 Tim Philomeno
                // we are only dealing with addresses and meeting data so take what we get and copy to what we have
                try
                {
                    //var existCouncil = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");

                    //existCouncil.PhyAddress = tblValCouncilFSEdit.PhyAddress;
                    //existCouncil.PhyCity = tblValCouncilFSEdit.PhyCity;
                    //existCouncil.PhyState = tblValCouncilFSEdit.PhyState;
                    //existCouncil.PhyPostalCode = tblValCouncilFSEdit.PhyPostalCode;
                    //existCouncil.MailAddress = tblValCouncilFSEdit.MailAddress;
                    //existCouncil.MailCity = tblValCouncilFSEdit.MailCity;
                    //existCouncil.MailState = tblValCouncilFSEdit.MailState;
                    //existCouncil.MailPostalCode = tblValCouncilFSEdit.MailPostalCode;
                    //existCouncil.MeetAddress = tblValCouncilFSEdit.MeetAddress;
                    //existCouncil.MeetCity = tblValCouncilFSEdit.MeetCity;
                    //existCouncil.MeetState = tblValCouncilFSEdit.MeetState;
                    //existCouncil.MeetPostalCode = tblValCouncilFSEdit.MeetPostalCode;
                    //existCouncil.BMeetDOW = tblValCouncilFSEdit.BMeetDOW;
                    //existCouncil.BMeetTime = tblValCouncilFSEdit.BMeetTime;
                    //existCouncil.OMeetDOW = tblValCouncilFSEdit.OMeetDOW;
                    //existCouncil.OMeetTime = tblValCouncilFSEdit.OMeetTime;
                    //existCouncil.SMeetDOW = tblValCouncilFSEdit.SMeetDOW;
                    //existCouncil.SMeetTime = tblValCouncilFSEdit.SMeetTime;
                    var myuser = await _userManager.GetUserAsync(User);
                    tblValCouncilFSEdit.Updated = DateTime.Now;
                    tblValCouncilFSEdit.UpdatedBy = myuser.KofCMemberID;
                    var results = await _apiHelper.PutAsync<TblValCouncilFSEdit, TblValCouncilFSEdit>($"/Council/FSEdit/{id}", tblValCouncilFSEdit);
                }
                catch (Exception ex)
                {
                    //***************************************************************************************************
                    // 12/05/2024 Tim Philomeno
                    // I want handle these consistantly so we are returning an http response that can be caught here
                    // the response message should appear in the ex.Message, then Log it and allow the method to
                    // finish refreshing the index page3
                    ModelState.AddModelError(string.Empty, ex.Message);
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    //------------------------------------------------------------------------------------------------------
                }
            }
            else
            {

                return View(tblValCouncilFSEdit);
            }
            if (User.IsInRole("Admin"))
            {
                return RedirectToAction("Index", "TblValCouncils");
            }
            else
            {
                return RedirectToAction("Index", "Home");
            }
        }



        //[Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        //[ValidateAntiForgeryToken]
        //public IActionResult MPDEdit([Bind(include: "CNumber,CName,District,SeatedDelegateDay1D1,SeatedDelegateDay1D2,SeatedDelegateDay2D1,SeatedDelegateDay2D2,SeatedDelegateDay3D1,SeatedDelegateDay3D2")] List<TblValCouncil> tblValCouncils)
        //public IActionResult MPDEdit([Bind(include: "CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime,SeatedDelegateDay1D1,SeatedDelegateDay1D2,SeatedDelegateDay2D1,SeatedDelegateDay2D2,SeatedDelegateDay3D1,SeatedDelegateDay3D2")] List<TblValCouncil> tblValCouncils)
        public async Task<IActionResult> MPDEdit(List<TblValCouncilMPD> tblValCouncils)
        {
            if (tblValCouncils == null || tblValCouncils.Count == 0)
            {
                return NotFound("Model from View is null");
            }

            if (ModelState.IsValid)
            {
                var myuser = await _userManager.GetUserAsync(User);
                
                foreach (var updatedCouncil in tblValCouncils)
                {
                    if (updatedCouncil != null)
                    {
                        if (IsMPDModifed(updatedCouncil))
                        {
                            // dont want to update these for MPD Edit
                            //updatedCouncil.Updated = DateTime.Now;
                            //updatedCouncil.UpdatedBy = myuser.KofCMemberID;
                            var result = await _apiHelper.PutAsync<TblValCouncilMPD, TblValCouncil>($"/Council/MPD/{updatedCouncil.CNumber}", updatedCouncil);
                        }
                        ////var council = new TblValCouncil { CNumber = updatedCouncil.CNumber };
                        ////_context.Attach(council);
                        ////council.SeatedDelegateDay1D1 = updatedCouncil.SeatedDelegateDay1D1;
                        ////_context.Entry(council).Property(e => e.SeatedDelegateDay1D1).IsModified = true;
                        ////council.Status = "A";
                        ////_context.Entry(council).Property(e => e.Status).IsModified = true;
                        // continue with others...
                        //var result = await _apiHelper.PutAsync<TblValCouncilMPD, TblValCouncil>($"/Council/MPD/{updatedCouncil.CNumber}", updatedCouncil);
                    }
                } // Save changes to the database in a real application }
            }
            return RedirectToAction("MPDEdit");
        }
        // POST: TblValCouncils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        //public async Task<IActionResult> Edit(int id, [Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime,SeatedDelegateDay1D1,SeatedDelegateDay1D2,SeatedDelegateDay2D1,SeatedDelegateDay2D2,SeatedDelegateDay3D1,SeatedDelegateDay3D2")] TblValCouncil tblValCouncil)
        //public async Task<IActionResult> Edit(int id, [Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status")] TblValCouncil tblValCouncil)
        public async Task<IActionResult> Edit(int id,TblValCouncil tblValCouncil)
        {
            if (id != tblValCouncil.CNumber)
            {
                return NotFound();
            }
            if (ModelState.IsValid)
            {
                //*****************************************************************************************************
                // 12/05/2024 Tim Philomeno
                // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                // call the API
                try
                {
                    //var myuser = await _userManager.GetUserAsync(User);
                    //tblValCouncil.Updated = DateTime.Now;
                    //tblValCouncil.UpdatedBy = myuser.KofCMemberID;
                    var result = await _apiHelper.PutAsync<TblValCouncil, TblValCouncil>($"/Council/{id}", tblValCouncil);
                }
                catch (Exception ex)
                {
                    //***************************************************************************************************
                    // 12/05/2024 Tim Philomeno
                    // I want handle these consistantly so we are returning an http response that can be caught here
                    // the response message should appear in the ex.Message, then Log it and allow the method to
                    // finish refreshing the index page3
                    ModelState.AddModelError(string.Empty, ex.Message);
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    //------------------------------------------------------------------------------------------------------
                }
            }
            return RedirectToAction(nameof(Index));
        }

        // GET: TblValCouncils/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }
            //*****************************************************************************************************
            // 12/05/2024 Tim Philomeno
            // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
            // call the API
            // this API will return NotFound if the item is not found so the try/catch block will catch it
            // and return the same
            try
            {
                var result = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");
                return View(result);
            }
            catch (Exception ex)
            {
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
        }

        // POST: TblValCouncils/Delete/5
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _apiHelper.DeleteAsync($"/Council/{id}");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                var myError = ex.Message.Contains("FK")? "Cannot Delete a Council that has Members referenced":"Unknown Error";
                ModelState.AddModelError(string.Empty, myError);
                Log.Error(Utils.FormatLogEntry(this, ex));
                //return RedirectToAction(nameof(Index));
                return View();
                //var ev = new ErrorViewModel
                //{
                //    Message = ex.Message
                //};
                //TempData["errormessage"] = ex.Message;
                //return View("Error",ev);
            }
        }
        private bool IsMPDModifed(TblValCouncilMPD mpd)
        {
            var result = _apiHelper.GetAsync<TblValCouncilMPD>($"/Council/{mpd.CNumber}");
            if (mpd.SeatedDelegateDay1D1 != result.Result.SeatedDelegateDay1D1) { return true; }
            else if (mpd.SeatedDelegateDay1D2 != result.Result.SeatedDelegateDay1D2) { return true; }
            else if (mpd.SeatedDelegateDay2D1 != result.Result.SeatedDelegateDay2D1) { return true; }
            else if (mpd.SeatedDelegateDay2D2 != result.Result.SeatedDelegateDay2D2) { return true; }
            else if (mpd.SeatedDelegateDay3D1 != result.Result.SeatedDelegateDay3D1) { return true; }
            else if (mpd.SeatedDelegateDay3D2 != result.Result.SeatedDelegateDay3D2) { return true; }
            else { return false; }
        }
    }
}
