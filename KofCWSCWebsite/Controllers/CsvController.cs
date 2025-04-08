using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Globalization;
using System.IO;
using System.Net.Http;
using System.Text;
using System.Collections.Generic;
using CsvHelper; // You can install CsvHelper using NuGet
using CsvHelper.Configuration;
using KofCWSCWebsite.Models;
using KofCWSCWebsite.Data;
using KofCWSCWebsite.Services;
using Serilog;
using Microsoft.AspNetCore.Mvc.Razor.Compilation;
using NuGet.Protocol;
using Newtonsoft.Json;
using Microsoft.CodeAnalysis.VisualBasic.Syntax;
using Microsoft.AspNetCore.Mvc.ModelBinding;
using Microsoft.AspNetCore.Http.Extensions;
using System;
using Microsoft.AspNetCore.Authorization;
using Microsoft.IdentityModel.Tokens;
using System.Reflection;
using Microsoft.CodeAnalysis.CSharp.Syntax;



namespace KofCWSCWebsite.Controllers
{

    public class CsvController : Controller
    {
        public enum DelegateOffices
        {
            D1 = 115,
            D2 = 118,
            A1 = 119,
            A2 = 116
        }
        private DataSetService _dataSetService;
        private ApiHelper _apiHelper;
        private static bool _postFlag = false;
        private static int _fratyear;
        private static Guid _guid;
        public CsvController(DataSetService dataSetService, ApiHelper apiHelper)
        {
            _dataSetService = dataSetService;
            _apiHelper = apiHelper;
        }

        // GET: Display the form to upload CSV
        [Route("UploadDelegates/{id}")]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public IActionResult UploadDelegatesS(int id)
        {
            //************************************************************************************************
            // 12/10/2024 Tim Philomeno
            // I am using _PostFlag to allow my menu to call this process and NOT acutall do anything, just log
            // very strange behavior. This method get called twice to be able to get favico to the browser
            // so i create the static global var _postFlag and set it on the first call.  Subsequent calls
            // do not change it because once it is set to true it will not change
            //------------------------------------------------------------------------------------------------
            var myQ = Request.Path.Value.Split("/")[2];

            if (!_postFlag)
            {
                _postFlag = myQ == "1" ? true : false; ;
            }
            //------------------------------------------------------------------------------------------------
            return View("/Views/Convention/UploadDelegates.cshtml");
        }
        [Route("UploadNecrology/{id}")]
        //[HttpGet]
        [Authorize(Roles = "Admin,Necrology")]
        public IActionResult UploadNecrologyS(int id)
        {
            //************************************************************************************************
            // 12/10/2024 Tim Philomeno
            // I am using _PostFlag to allow my menu to call this process and NOT acutall do anything, just log
            // very strange behavior. This method get called twice to be able to get favico to the browser
            // so i create the static global var _postFlag and set it on the first call.  Subsequent calls
            // do not change it because once it is set to true it will not change
            //------------------------------------------------------------------------------------------------
            var myQ = Request.Path.Value.Split("/")[2];

            if (!_postFlag)
            {
                _postFlag = myQ == "1" ? true : false; ;
            }
            //------------------------------------------------------------------------------------------------
            return View("/Views/NecImpNecrologies/UploadNecrology.cshtml");
        }
        static bool ConvertToBool(string input)
        {
            // Treat "0" as false, anything else as true
            return input == "0" ? false : true;
        }

        // POST: Handle the CSV file upload and parse it
        [HttpPost]
        [Authorize(Roles = "Admin, Necrology")]
        public async Task<IActionResult> UploadNecrology(NecImpNecrologyViewModel model)
        {
            var records = new List<NecImpNecrology>();
            if (model.CsvFile != null && model.CsvFile.Length > 0)
            {
                using (var reader = new StreamReader(model.CsvFile.OpenReadStream(), Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    HeaderValidated = null,
                    MissingFieldFound = null,
                    Delimiter = ",",
                    HasHeaderRecord = true,
                    PrepareHeaderForMatch = args => args.Header.ToLowerInvariant() // Ignore case

            }))
                {
                    var cvnImport = csv.GetRecords<NecImpNecrology>(); // Parse CSV into CsvRecord objects
                    records.AddRange(cvnImport);
                }
                try
                {
                    var result = await _apiHelper.PostAsync<List<NecImpNecrology>, string>("/ImpNecrology", records);
                    ViewBag.ImpMessage = result;
                    TempData["ImpMessage"] = result;
                    return RedirectToAction("Index", "NecImpNecrologies");
                }
                catch (Exception ex)
                {
                    Log.Error(Utils.FormatLogEntry(this, ex));
                    ViewBag.ImpError = "Import of CSV File Failed";
                    return View("Views/Convention/ImpDelegatesFailed.cshtml", null);
                }
                //------------------------------------------------------------------------------------------------------
            }
            return RedirectToAction("NecImpNecrologies", "Index");
        }

        // POST: Handle the CSV file upload and parse it
        [HttpPost]
        [Authorize(Roles = "Admin, ConventionAdmin")]
        public async Task<IActionResult> UploadDelegates(CvnImpDelegateViewModel model)
        {
            var records = new List<CvnImpDelegateIMP>();
            //*************************************************************************
            // 12/3/2024 Tim Philomeno
            // pass this GUID to all logging processes and then get the full log
            // from the database and return it to the calling process
            _guid = Guid.NewGuid();
            _fratyear = await _apiHelper.GetAsync<int>("GetFratYear/0");
            WriteToDelegateImportLog(_guid, 0, "INFO", "BEGIN Import");
            //-------------------------------------------------------------------------
            if (model.CsvFile != null && model.CsvFile.Length > 0)
            {
                using (var reader = new StreamReader(model.CsvFile.OpenReadStream(), Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    //HeaderValidated = null,
                    Delimiter = ",",
                    HasHeaderRecord = true,

                }))
                {
                    try
                    {
                        var cvnImport = csv.GetRecords<CvnImpDelegateIMP>(); // Parse CSV into CsvRecord objects

                        records.AddRange(cvnImport);
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Utils.FormatLogEntry(this, ex));
                        ViewBag.ImpError = "The CSV File is not formatted properly.  Please run the macro to fix.";
                        return View("Views/Convention/ImpDelegatesFailed.cshtml", records);
                    }
                    //****************************************************************************************
                    // 12/1/2024 Tim Philomeno Save the data to the database
                    //*****************************************************************************************************
                    // Let's check to make sure that our incoming data does not have duplicate councils
                    if (HasDupCouncils(records))
                    {
                        ModelState.AddModelError(string.Empty, "Duplicate Councils found in CSV File. Please correct and try again");
                        ViewBag.ImpError = "Duplicate Councils found in CSV File";
                        return View("Views/Convention/ImpDelegatesFailed.cshtml", records);
                    }

                    ////////////PropertyInfo[] properties = typeof(CvnImpDelegate).GetProperties();
                    ////////////for (int i = 0; i < properties.Length; i++)
                    ////////////{
                    ////////////    PropertyInfo prop = properties[i];
                    ////////////    object value = prop.GetValue(model);
                    ////////////    if (value == "D1")
                    ////////////    {
                    ////////////        ViewBag.ImpError = "CSV File is not foratted properly.  Please run the macro.";
                    ////////////        return View("Views/Convention/ImpDelegatesFailed.cshtml", records);
                    ////////////    }
                    ////////////}
                    // Add records to list
                    //****************************************************************************************
                    // 12/1/2024 Tim Philomeno Save the data to the database
                    //*****************************************************************************************************
                    // 12/05/2024 Tim Philomeno
                    // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                    // call the API
                    try
                    {
                        var result = await _apiHelper.PostAsync<List<CvnImpDelegateIMP>, string>("/ImpDelegates", records);
                        ViewBag.ImpMessage = result;
                        TempData["ImpMessage"] = result;
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Utils.FormatLogEntry(this, ex));
                        ViewBag.ImpError = "Import of CSV File Failed";
                        return View("Views/Convention/ImpDelegatesFailed.cshtml", null);
                    }
                    //------------------------------------------------------------------------------------------------------
                    // first let's remove the currrent delegates in favor of the ones we are about to add
                    var apiHelperD = new ApiHelper(_dataSetService);
                    var apiHelperC = new ApiHelper(_dataSetService);
                    WriteToDelegateImportLog(_guid, 0, "INFO", $"Removing old Delegates in lieu of incoming ones.");
                    await apiHelperD.GetAsync<int>($"/ClearDelegates/{_fratyear}");

                    //------------------------------------------------------------------------------------------------------
                    ViewBag.ImportedDelegates = records.Count();
                    //****************************************************************************************
                    // now reset and prime council and dd seated days
                    var affected = await _apiHelper.GetAsync<int>("PrimeCouncilDelegatesAndDDDays");
                    WriteToDelegateImportLog(_guid, 0, "INFO", $"Resetting and priming council and dd seated days. {affected} records affected.");
                    //------------------------------------------------------------------------------------------------------
                    // 12/1/2024 Tim PHilomeno then add or update the members as needed
                    foreach (var myDel in records)
                    {
                        try
                        {
                            WriteToDelegateImportLog(_guid, myDel.D1MemberID, "INFO", $"Processing of Council {myDel.CouncilNumber} Started");
                            //bool D1 = await ProcessCouncilD1(myDel);
                            if (!await ProcessCouncilD1(myDel))
                            {
                                WriteToDelegateImportLog(_guid, myDel.D1MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} D1, MemberID {myDel.D1MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID D1 {myDel.D1MemberID} Failed");
                            }
                            //bool D2 = await ProcessCouncilD2(myDel);
                            if (!await ProcessCouncilD2(myDel))
                            {
                                WriteToDelegateImportLog(_guid, myDel.D2MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} D2, MemberID {myDel.D2MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID D2 {myDel.D2MemberID} Failed");
                            }
                            //bool A1 = await ProcessCouncilA1(myDel);
                            if (!await ProcessCouncilA1(myDel))
                            {
                                WriteToDelegateImportLog(_guid, myDel.A1MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} A1, MemberID {myDel.A1MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID A1 {myDel.A1MemberID} Failed");
                            }
                            //bool A2 = await ProcessCouncilA2(myDel);
                            if (!await ProcessCouncilA2(myDel))
                            {
                                WriteToDelegateImportLog(_guid, myDel.A2MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} A2, MemberID {myDel.A2MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID A2 {myDel.A2MemberID} Failed");
                            }

                        }
                        catch (Exception ex)
                        {
                            Log.Error(Utils.FormatLogEntry(this, ex));
                            ViewBag.ImpError = "Processing of Delegate Records Failed";
                            return View("Views/Convention/ImpDelegatesFailed.cshtml", records);
                        }

                    }
                    //----------------------------------------------------------------------------------------
                    // when we are all done then process the records on the server to add corrmemberoffices
                    // only allow the corrmemberoffices to be created if all KofCIDs are accounted for
                    var myDelegates = await _apiHelper.GetAsync<IEnumerable<TblCorrMemberOfficeVM>>("CheckForMissingDelegateMembersAndCreateDelegates");
                    if (myDelegates.Count() > 0)
                    {
                        ViewBag.Message = $@"We found {myDelegates.Count().ToString()} member(s) in the import file in that are not in our database.
                            Please make sure this/these members are added and rerun this process.
                            NOTE: the MemberID Listed is the KofC MemberID";
                        // then we need to present the issues to the user and stop here
                        return View("Views/TblCorrMemberOffices/MissingDelegates.cshtml", myDelegates);
                    }
                    //----------------------------------------------------------------------------------------
                    WriteToDelegateImportLog(_guid, 0, "INFO", "END Import");
                    var apiHelperLog = new ApiHelper(_dataSetService);
                    var myLog = await apiHelperLog.GetAsync<IEnumerable<CvnImpDelegatesLog>>($"/GetImpDelegatesLog/{_guid}");
                    //return View("Views/Convention/ImpDelegatesSuccess.cshtml", myLog);
                    TempData["Message"] = $"Import of {records.Count()} Councils was Successful ";
                    return RedirectToAction("Index", "CvnImpDelegates");
                }
            }
            ModelState.AddModelError("", "Please upload a valid CSV file.");
            ViewBag.ImpError = "CSV File is invalid";
            return View("Views/Convention/ImpDelegatesFailed.cshtml", null);
        }
        private async Task<bool> ProcessCouncilD1(CvnImpDelegateIMP cvnImpDelegate)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.D1MemberID is not null)
            {
                WriteToDelegateImportLog(_guid, cvnImpDelegate.D1MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} D1 Started");
                // first see if the D1 member exists
                TblMasMember? myIsD1Member = null;

                var apiHelper = new ApiHelper(_dataSetService);
                myIsD1Member = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{cvnImpDelegate.D1MemberID}");

                // if D1 exists myIsD1Member will not be null
                if (myIsD1Member == null) // Add a Member
                {
                    try
                    {
                        if (FillD1(ref myIsD1Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsD1Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D1MemberID, "ADD", "Adding New Member");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D1MemberID, "INFO", "LOG ONLY Adding New Member");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }

                }
                else // Update an existing Member
                {
                    try
                    {
                        if (FillD1(ref myIsD1Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                //await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsD1Member.MemberId}", myIsD1Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D1MemberID, "INFO", "Updating Existing Member - NOTE: Member Update is Disabled");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D1MemberID, "INFO", "LOG ONLY Updating Existing Member - NOTE: Member Update is Disabled");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Utils.FormatLogEntry(this, ex));
                        return false;
                    }
                }
                // add the office here
                AddDelegateOffice((int)cvnImpDelegate.D1MemberID, (int)DelegateOffices.D1);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilD2(CvnImpDelegateIMP cvnImpDelegate)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.D2MemberID is not null)
            {
                WriteToDelegateImportLog(_guid, cvnImpDelegate.D2MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} D2 Started");
                // first see if the D2 member exists
                TblMasMember? myIsD2Member = null;

                var apiHelper = new ApiHelper(_dataSetService);
                myIsD2Member = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{cvnImpDelegate.D2MemberID}");

                // if D2 exists myIsD2Member will not be null
                if (myIsD2Member == null) // Add a Member
                {
                    try
                    {
                        if (FillD2(ref myIsD2Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsD2Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D2MemberID, "ADD", "Add a New Member");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D2MemberID, "INFO", "LOG ONLY Add a New Member");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                else // Update an existing Member
                {
                    try
                    {
                        if (FillD2(ref myIsD2Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                //await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsD2Member.MemberId}", myIsD2Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D2MemberID, "INFO", "Update an Existing Member - NOTE: Member Update is Disabled");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.D2MemberID, "INFO", "LOG ONLY Update an Existing Member - NOTE: Member Update is Disabled");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegateOffice((int)cvnImpDelegate.D2MemberID, (int)DelegateOffices.D2);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilA1(CvnImpDelegateIMP cvnImpDelegate)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.A1MemberID is not null)
            {
                WriteToDelegateImportLog(_guid, cvnImpDelegate.A1MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} A1 Started");
                // first see if the A1 member exists
                TblMasMember? myIsA1Member = null;

                var apiHelper = new ApiHelper(_dataSetService);
                myIsA1Member = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{cvnImpDelegate.A1MemberID}");

                // if A1 exists myIsA1Member will not be null
                if (myIsA1Member == null) // Add a Member
                {
                    try
                    {
                        if (FillA1(ref myIsA1Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsA1Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A1MemberID, "ADD", "Add a New Member");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A1MemberID, "INFO", "LOG ONLY Add a New Member");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                else // Update an existing Member
                {
                    try
                    {
                        if (FillA1(ref myIsA1Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                //await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsA1Member.MemberId}", myIsA1Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A1MemberID, "INFO", "Update an Existing Member - NOTE: Member Update is Disabled");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A1MemberID, "INFO", "LOG ONLY Update an Existing Member - NOTE: Member Update is Disabled");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegateOffice((int)cvnImpDelegate.A1MemberID, (int)DelegateOffices.A1);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilA2(CvnImpDelegateIMP cvnImpDelegate)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.A2MemberID is not null)
            {
                WriteToDelegateImportLog(_guid, cvnImpDelegate.A2MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} A2 Started");
                // first see if the A2 member exists
                TblMasMember? myIsA2Member = null;

                var apiHelper = new ApiHelper(_dataSetService);
                myIsA2Member = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{cvnImpDelegate.A2MemberID}");

                // if A2 exists myIsA2Member will not be null
                if (myIsA2Member == null) // Add a Member
                {
                    try
                    {
                        if (FillA2(ref myIsA2Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsA2Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A2MemberID, "ADD", "Add a New Member");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A2MemberID, "INFO", "LOG ONLY Add a New Member");
                            }

                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                else // Update an existing Member
                {
                    try
                    {
                        if (FillA2(ref myIsA2Member, cvnImpDelegate))
                        {
                            if (_postFlag)
                            {
                                //await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsA2Member.MemberId}", myIsA2Member);
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A2MemberID, "INFO", "Update an Existing Member - NOTE: Member Update is Disabled");
                            }
                            else
                            {
                                WriteToDelegateImportLog(_guid, cvnImpDelegate.A2MemberID, "INFO", "LOG ONLY Update an Existing Member - NOTE: Member Update is Disabled");
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegateOffice((int)cvnImpDelegate.A2MemberID, (int)DelegateOffices.A2);
            }
            return true;
        }

        private bool FillD1(ref TblMasMember myMember, CvnImpDelegateIMP myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;

            if (myMember == null)
            {
                myMember = new TblMasMember();
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.D1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.D1FirstName, myMember.FirstName, myMember, "FirstName"))
            {
                myMember.FirstName = myDelegate.D1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.D1LastName, myMember.LastName, myMember, "LastName"))
            {
                myMember.LastName = myDelegate.D1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.D1MiddleName, myMember.Mi, myMember, "MiddleName"))
            {
                myMember.Mi = myDelegate.D1MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.D1Suffix, myMember.Suffix, myMember, "Suffix"))
            {
                myMember.Suffix = myDelegate.D1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.D1Address1, myMember.Address, myMember, "Address"))
            {
                myMember.Address = myDelegate.D1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.D1City, myMember.City, myMember, "City"))
            {
                myMember.City = myDelegate.D1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.D1State), myMember.State, myMember, "State"))
            {
                myMember.State = GetStateAbbr(myDelegate.D1State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.D1ZipCode, myMember.PostalCode, myMember, "PostalCode"))
            {
                myMember.PostalCode = myDelegate.D1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.D1Phone, myMember.Phone, myMember, "Phone"))
            {
                myMember.Phone = myDelegate.D1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.D1Email, myMember.Email, myMember, "Email"))
            {
                myMember.Email = myDelegate.D1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillD2(ref TblMasMember myMember, CvnImpDelegateIMP myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;

            if (myMember == null)
            {
                myMember = new TblMasMember();
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.D2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.D2FirstName, myMember.FirstName, myMember, "FirstName"))
            {
                myMember.FirstName = myDelegate.D2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.D2LastName, myMember.LastName, myMember, "LastName"))
            {
                myMember.LastName = myDelegate.D2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.D2MiddleName, myMember.Mi, myMember, "MiddleName"))
            {
                myMember.Mi = myDelegate.D2MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.D2Suffix, myMember.Suffix, myMember, "Suffix"))
            {
                myMember.Suffix = myDelegate.D2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.D2Address1, myMember.Address, myMember, "Address"))
            {
                myMember.Address = myDelegate.D2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.D2City, myMember.City, myMember, "City"))
            {
                myMember.City = myDelegate.D2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.D2State), myMember.State, myMember, "State"))
            {
                myMember.State = GetStateAbbr(myDelegate.D2State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.D2ZipCode, myMember.PostalCode, myMember, "PostalCode"))
            {
                myMember.PostalCode = myDelegate.D2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.D2Phone, myMember.Phone, myMember, "Phone"))
            {
                myMember.Phone = myDelegate.D2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.D2Email, myMember.Email, myMember, "Email"))
            {
                myMember.Email = myDelegate.D2Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillA1(ref TblMasMember myMember, CvnImpDelegateIMP myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;

            if (myMember == null)
            {
                myMember = new TblMasMember();
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.A1MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.A1FirstName, myMember.FirstName, myMember, "FirstName"))
            {
                myMember.FirstName = myDelegate.A1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.A1LastName, myMember.LastName, myMember, "LastName"))
            {
                myMember.LastName = myDelegate.A1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.A1MiddleName, myMember.Mi, myMember, "MiddleName"))
            {
                myMember.Mi = myDelegate.A1MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.A1Suffix, myMember.Suffix, myMember, "Suffix"))
            {
                myMember.Suffix = myDelegate.A1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.A1Address1, myMember.Address, myMember, "Address"))
            {
                myMember.Address = myDelegate.A1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.A1City, myMember.City, myMember, "City"))
            {
                myMember.City = myDelegate.A1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.A1State), myMember.State, myMember, "State"))
            {
                myMember.State = GetStateAbbr(myDelegate.A1State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.A1ZipCode, myMember.PostalCode, myMember, "PostalCode"))
            {
                myMember.PostalCode = myDelegate.A1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.A1Phone, myMember.Phone, myMember, "Phone"))
            {
                myMember.Phone = myDelegate.A1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.A1Email, myMember.Email, myMember, "Email"))
            {
                myMember.Email = myDelegate.A1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillA2(ref TblMasMember myMember, CvnImpDelegateIMP myDelegate)
        {
            string UpdatedBy = "Delegate Import API";
            bool isUpdated = false;

            if (myMember == null)
            {
                myMember = new TblMasMember();
            }

            // if myMember is null that means that we are adding a new one so we need to
            // spin up a new model object and skip memberid assignmane
            // if we have an exitsting incomning member recored it's MemberID will already
            // be set so don't mess with it
            // myMember.MemberId = ??

            myMember.KofCid = (int)myDelegate.A2MemberID;
            // ok for each member property that we can update, let's figure out if the data
            // has changed.  If so, then update it else leave it alone
            //-------------------------------------------------------------------------------------
            // the only time we update the property is if it changed or is null
            // changed allows us to only update property data that has actually changes
            // based on the existing value.  If the property is null then we are adding and
            // want the assignment too  != Property should work in both cases, NULL if 
            // adding and not equal if we are updating
            //-------------------------------------------------------------------------------------
            // COUNCIL
            if (myMember.Council != (int)myDelegate.CouncilNumber)
            {
                myMember.Council = (int)myDelegate.CouncilNumber;
                myMember.CouncilUpdated = DateTime.Now;
                myMember.CouncilUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // FIRSTNAME
            if (ShouldUpdate(myDelegate.A2FirstName, myMember.FirstName, myMember, "FirstName"))
            {
                myMember.FirstName = myDelegate.A2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (ShouldUpdate(myDelegate.A2LastName, myMember.LastName, myMember, "LastName"))
            {
                myMember.LastName = myDelegate.A2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (ShouldUpdate(myDelegate.A2MiddleName, myMember.Mi, myMember, "MiddleName"))
            {
                myMember.Mi = myDelegate.A2MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (ShouldUpdate(myDelegate.A2Suffix, myMember.Suffix, myMember, "Suffix"))
            {
                myMember.Suffix = myDelegate.A2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (ShouldUpdate(myDelegate.A2Address1, myMember.Address, myMember, "Address"))
            {
                myMember.Address = myDelegate.A2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddressUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (ShouldUpdate(myDelegate.A2City, myMember.City, myMember, "City"))
            {
                myMember.City = myDelegate.A2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (ShouldUpdate(GetStateAbbr(myDelegate.A2State), myMember.State, myMember, "State"))
            {
                myMember.State = GetStateAbbr(myDelegate.A2State);
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (ShouldUpdate(myDelegate.A2ZipCode, myMember.PostalCode, myMember, "PostalCode"))
            {
                myMember.PostalCode = myDelegate.A2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (ShouldUpdate(myDelegate.A2Phone, myMember.Phone, myMember, "Phone"))
            {
                myMember.Phone = myDelegate.A2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (ShouldUpdate(myDelegate.A2Email, myMember.Email, myMember, "Email"))
            {
                myMember.Email = myDelegate.A2Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private async Task<bool> ProcessDelegateImport(Guid GUID)
        {
            try
            {
                var apiHelper = new ApiHelper(_dataSetService);
                await apiHelper.GetAsync<TblMasMember>($"/ProcessDelegateImport/{GUID}");
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                return false;
            }
            return true;
        }
        private async void WriteToDelegateImportLog(Guid GUID, int? MemberID, string Type, string Data)
        {
            CvnImpDelegatesLog cvnImpDelegatesLog = new CvnImpDelegatesLog();
            cvnImpDelegatesLog.Rundate = DateTime.Now;
            cvnImpDelegatesLog.Guid = GUID;
            cvnImpDelegatesLog.Type = Type;
            cvnImpDelegatesLog.MemberId = MemberID;
            cvnImpDelegatesLog.Data = Data;
            var apiHelper = new ApiHelper(_dataSetService);
            await apiHelper.PostAsync<CvnImpDelegatesLog, CvnImpDelegatesLog>("/CreateImpDelegatesLog", cvnImpDelegatesLog);
        }
        private async void AddDelegateOffice(int KofCID, int OfficeId)
        {
            try
            {
                return;
                var apiHelper = new ApiHelper(_dataSetService);

                TblCorrMemberOffice tblCorrMemberOffice = new TblCorrMemberOffice();
                TblMasMember myID = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{KofCID}");
                if (myID == null)
                {
                    WriteToDelegateImportLog(_guid, 0, "INFO", $"Unable to add Office {OfficeId} for missing Member {KofCID}");
                    return;
                }
                tblCorrMemberOffice.OfficeId = OfficeId;
                tblCorrMemberOffice.MemberId = myID.MemberId;
                tblCorrMemberOffice.PrimaryOffice = false;
                tblCorrMemberOffice.Year = _fratyear;
                tblCorrMemberOffice.District = null;
                tblCorrMemberOffice.Council = null;
                tblCorrMemberOffice.Assembly = null;

                await apiHelper.PostAsync<TblCorrMemberOffice, TblCorrMemberOffice>("/MemberOffice", tblCorrMemberOffice);
                WriteToDelegateImportLog(_guid, myID.MemberId, "INFO", $"Adding Office {OfficeId} to Member {myID.MemberId}");
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex, "***" + $"Adding Office {OfficeId} to Member {KofCID}"));
                WriteToDelegateImportLog(_guid, KofCID, "ERROR", $"Adding Office {OfficeId} to Member {KofCID}");
            }

        }
        private bool HasDupCouncils(List<CvnImpDelegateIMP> model)
        {
            if (model == null) { return true; }
            foreach (var myDel in model)
            {
                if (model.Where(u => u.CouncilNumber == myDel.CouncilNumber).Count() > 1)
                {
                    return true;
                }
            }
            return false;
        }
        private bool ShouldUpdate(string? inItem, string? exItem, TblMasMember member, string what)
        {
            // if we are adding a new member then the exItem will be null or blank
            if (!exItem.IsNullOrEmpty())
            {
                // if we are updating an existing item but the incoming value is null or blank don't update
                if (!inItem.IsNullOrEmpty())
                {
                    if (exItem.ToUpper() != inItem.ToUpper())
                    {
                        WriteToDelegateImportLog(_guid, member.KofCid, "UPD", $"Update an Existing Member - {what} - our data={exItem} - delegate data={inItem}");
                        return true;
                    }
                    else
                    {
                        return false;
                    }
                }
                return false;
            }
            return true;
        }
        private string GetStateAbbr(string state)
        {
            if (state.Length > 2)
            {
                return _dataSetService.GetStateAbbreviation(state);
            }
            else
            {
                return state;
            }
        }

    }
}

