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

namespace KofCWSCWebsite.Controllers
{
    public class TblValCouncilsController : Controller
    {
        private DataSetService _dataSetService;
        private readonly ApiHelper _apiHelper;

        public TblValCouncilsController(DataSetService dataSetService, ApiHelper apiHelper)
        {
            _dataSetService = dataSetService;
            _apiHelper = new ApiHelper(_dataSetService);
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
                return View("FSDetails",result);
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
                    var apiHelper = new ApiHelper(_dataSetService);
                    var result = await apiHelper.PostAsync<TblValCouncil,TblValCouncil>("/Council/",tblValCouncil);
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
                var result = await _apiHelper.GetAsync<TblValCouncil>($"/Council/{id}");
                return View("FSEdit",result);
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
            public async Task<IActionResult> FSEdit(int id, [Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime")] TblValCouncil tblValCouncil)
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
                    var result = await _apiHelper.PutAsync<TblValCouncil, TblMasMember>($"/Council/{id}", tblValCouncil);
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
            return RedirectToAction("Index","Home");
        }


        // POST: TblValCouncils/Edit/5
        // To protect from overposting attacks, enable the specific properties you want to bind to.
        // For more details, see http://go.microsoft.com/fwlink/?LinkId=317598.
        [Authorize(Roles = "Admin,DataAdmin")]
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("CNumber,CLocation,CName,District,AddInfo1,AddInfo2,AddInfo3,LiabIns,DioceseId,Chartered,WebSiteUrl,BulletinUrl,Arbalance,Status,PhyAddress,PhyCity,PhyState,PhyPostalCode,MailAddress,MailCity,MailState,MailPostalCode,MeetAddress,MeetCity,MeetState,MeetPostalCode,BMeetDOW,BMeetTime,OMeetDOW,OMeetTime,SMeetDOW,SMeetTime")] TblValCouncil tblValCouncil)
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
                ModelState.AddModelError(string.Empty, ex.Message);
                Log.Error(Utils.FormatLogEntry(this, ex));
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
