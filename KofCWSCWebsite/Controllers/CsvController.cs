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
        private static bool _postFlag = false;
        public CsvController(DataSetService dataSetService)
        {
            _dataSetService = dataSetService;
        }

        // GET: Display the form to upload CSV
        [Route("UploadDelegates/{id}")]
        public IActionResult UploadDelegates(int id)
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
        static bool ConvertToBool(string input)
        {
            // Treat "0" as false, anything else as true
            return input == "0" ? false : true;
        }
        // POST: Handle the CSV file upload and parse it
        [HttpPost]
        public async Task<IActionResult> Upload(CvnImpDelegateViewModel model)
        {
            var records = new List<CvnImpDelegate>();
            //*************************************************************************
            // 12/3/2024 Tim Philomeno
            // pass this GUID to all logging processes and then get the full log
            // from the database and return it to the calling process
            Guid guid = Guid.NewGuid();
            //-------------------------------------------------------------------------
            if (model.CsvFile != null && model.CsvFile.Length > 0)
            {
                using (var reader = new StreamReader(model.CsvFile.OpenReadStream(), Encoding.UTF8))
                using (var csv = new CsvReader(reader, new CsvConfiguration(CultureInfo.InvariantCulture)
                {
                    Delimiter = ",",
                    HasHeaderRecord = true,

                }))
                {
                    var cvnImport = csv.GetRecords<CvnImpDelegate>(); // Parse CSV into CsvRecord objects
                    records.AddRange(cvnImport);
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
                    // Add records to list
                    //****************************************************************************************
                    // 12/1/2024 Tim Philomeno Save the data to the database
                    //*****************************************************************************************************
                    // 12/05/2024 Tim Philomeno
                    // Now that we have a generic ApiHelper class, these are the only 2 lines that we should need to
                    // call the API
                    try
                    {
                        var apiHelper = new ApiHelper(_dataSetService);
                        var result = await apiHelper.PostAsync<List<CvnImpDelegate>, CvnImpDelegate>("/ImpDelegates", records);
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
                    WriteToDelegateImportLog(guid, 0, "INFO", $"Removing old Delegates in lieu of incoming ones.");
                    await apiHelperD.GetAsync<int>($"/ClearDelegates/{await apiHelperC.GetAsync<int>($"/GetFratYear/{0}")}");

                    //------------------------------------------------------------------------------------------------------
                    ViewBag.ImportedDelegates = records.Count();
                    //****************************************************************************************
                    // 12/1/2024 Tim PHilomeno then add or update the members as needed
                    foreach (var myDel in records)
                    {
                        try
                        {
                            WriteToDelegateImportLog(guid, myDel.D1MemberID, "INFO", $"Processing of Council {myDel.CouncilNumber} Started");
                            //bool D1 = await ProcessCouncilD1(myDel);
                            if (!await ProcessCouncilD1(myDel, guid))
                            {
                                WriteToDelegateImportLog(guid, myDel.D1MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} D1, MemberID {myDel.D1MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID D1 {myDel.D1MemberID} Failed");
                            }
                            //bool D2 = await ProcessCouncilD2(myDel);
                            if (!await ProcessCouncilD2(myDel, guid))
                            {
                                WriteToDelegateImportLog(guid, myDel.D2MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} D2, MemberID {myDel.D2MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID D2 {myDel.D2MemberID} Failed");
                            }
                            //bool A1 = await ProcessCouncilA1(myDel);
                            if (!await ProcessCouncilA1(myDel, guid))
                            {
                                WriteToDelegateImportLog(guid, myDel.A1MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} A1, MemberID {myDel.A1MemberID} Failed");
                                throw new Exception($"Processing of {myDel.CouncilNumber} for MemberID A1 {myDel.A1MemberID} Failed");
                            }
                            //bool A2 = await ProcessCouncilA2(myDel);
                            if (!await ProcessCouncilA2(myDel, guid))
                            {
                                WriteToDelegateImportLog(guid, myDel.A2MemberID, "ERROR", $"Processing of Council {myDel.CouncilNumber} A2, MemberID {myDel.A2MemberID} Failed");
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
                    //await ProcessDelegateImport(guid);
                    var apiHelperLog = new ApiHelper(_dataSetService);
                    var myLog = await apiHelperLog.GetAsync<IEnumerable<CvnImpDelegatesLog>>($"/GetImpDelegatesLog/{guid}");
                    return View("Views/Convention/ImpDelegatesSuccess.cshtml", myLog);
                }
            }
            ModelState.AddModelError("", "Please upload a valid CSV file.");
            ViewBag.ImpError = "CSV File is invalid";
            return View("Views/Convention/ImpDelegatesFailed.cshtml", null);
        }
        private async Task<bool> ProcessCouncilD1(CvnImpDelegate cvnImpDelegate, Guid guid)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.D1MemberID is not null)
            {
                WriteToDelegateImportLog(guid, 0, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} D1 Started");
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
                            if (_postFlag) { await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsD1Member); }
                            WriteToDelegateImportLog(guid, cvnImpDelegate.D1MemberID, "INFO", "Adding New Member");
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
                            if (_postFlag) { await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsD1Member.MemberId}", myIsD1Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.D1MemberID, "INFO", "Updating Existing Member");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(Utils.FormatLogEntry(this, ex));
                        return false;
                    }
                }
                // add the office here
                AddDelegate((int)cvnImpDelegate.D1MemberID, (int)DelegateOffices.D1, guid);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilD2(CvnImpDelegate cvnImpDelegate, Guid guid)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.D2MemberID is not null)
            {
                WriteToDelegateImportLog(guid, cvnImpDelegate.D2MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} D2 Started");
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
                            if (_postFlag) { await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsD2Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.D2MemberID, "INFO", "Add a New Member");
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
                            if (_postFlag) { await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsD2Member.MemberId}", myIsD2Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.D2MemberID, "INFO", "Update an Existing Member");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegate((int)cvnImpDelegate.D2MemberID, (int)DelegateOffices.D2, guid);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilA1(CvnImpDelegate cvnImpDelegate, Guid guid)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.A1MemberID is not null)
            {
                WriteToDelegateImportLog(guid, cvnImpDelegate.A1MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} A1 Started");
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
                            if (_postFlag) { await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsA1Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.A1MemberID, "INFO", "Add a New Member");
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
                            if (_postFlag) { await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsA1Member.MemberId}", myIsA1Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.A1MemberID, "INFO", "Update an Existing Member");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegate((int)cvnImpDelegate.A1MemberID, (int)DelegateOffices.A1, guid);
            }
            return true;
        }
        private async Task<bool> ProcessCouncilA2(CvnImpDelegate cvnImpDelegate, Guid guid)
        {
            //**********************************************************************************************
            // 11/30/2024 Tim Philomeno
            // This will process each council delegate model individually adding and updating member data
            // as needed and will add the appropriate delegate office record.
            //----------------------------------------------------------------------------------------------
            if (cvnImpDelegate.A2MemberID is not null)
            {
                WriteToDelegateImportLog(guid, cvnImpDelegate.A2MemberID, "INFO", $"Processing of Council {cvnImpDelegate.CouncilNumber} A2 Started");
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
                            if (_postFlag) { await apiHelper.PostAsync<TblMasMember, TblMasMember>("/Member", myIsA2Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.A2MemberID, "INFO", "Add a New Member");

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
                            if (_postFlag) { await apiHelper.PutAsync<TblMasMember, TblMasMember>($"/member/{myIsA2Member.MemberId}", myIsA2Member); };
                            WriteToDelegateImportLog(guid, cvnImpDelegate.A2MemberID, "INFO", "Update an Existing Member");
                        }
                    }
                    catch (Exception ex)
                    {
                        Log.Error(ex.Message + " " + ex.InnerException);
                        return false;
                    }
                }
                AddDelegate((int)cvnImpDelegate.A2MemberID, (int)DelegateOffices.A2, guid);
            }
            return true;
        }

        private bool FillD1(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delgate Import API";
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
            if (myMember.FirstName != myDelegate.D1FirstName)
            {
                myMember.FirstName = myDelegate.D1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (myMember.LastName != myDelegate.D1LastName)
            {
                myMember.LastName = myDelegate.D1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (myMember.Mi != myDelegate.D1MiddleName)
            {
                myMember.Mi = myDelegate.D1MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (myMember.Suffix != myDelegate.D1Suffix)
            {
                myMember.Suffix = myDelegate.D1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (myMember.Address != myDelegate.D1Address1)
            {
                myMember.Address = myDelegate.D1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddInfo2UpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (myMember.City != myDelegate.D1City)
            {
                myMember.City = myDelegate.D1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (myMember.State != myDelegate.D1State)
            {
                myMember.State = myDelegate.D1State;
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (myMember.PostalCode != myDelegate.D1ZipCode)
            {
                myMember.PostalCode = myDelegate.D1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (myMember.Phone != myDelegate.D1Phone)
            {
                myMember.Phone = myDelegate.D1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (myMember.Email != myDelegate.D1Email)
            {
                myMember.Email = myDelegate.D1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillD2(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delgate Import API";
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
            if (myMember.FirstName != myDelegate.D2FirstName)
            {
                myMember.FirstName = myDelegate.D2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (myMember.LastName != myDelegate.D2LastName)
            {
                myMember.LastName = myDelegate.D2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (myMember.Mi != myDelegate.D2MiddleName)
            {
                myMember.Mi = myDelegate.D2MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (myMember.Suffix != myDelegate.D2Suffix)
            {
                myMember.Suffix = myDelegate.D2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (myMember.Address != myDelegate.D2Address1)
            {
                myMember.Address = myDelegate.D2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddInfo2UpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (myMember.City != myDelegate.D2City)
            {
                myMember.City = myDelegate.D2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (myMember.State != myDelegate.D2State)
            {
                myMember.State = myDelegate.D2State;
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (myMember.PostalCode != myDelegate.D2ZipCode)
            {
                myMember.PostalCode = myDelegate.D2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (myMember.Phone != myDelegate.D2Phone)
            {
                myMember.Phone = myDelegate.D2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (myMember.Email != myDelegate.D2Email)
            {
                myMember.Email = myDelegate.D2Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillA1(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delgate Import API";
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
            if (myMember.FirstName != myDelegate.A1FirstName)
            {
                myMember.FirstName = myDelegate.A1FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (myMember.LastName != myDelegate.A1LastName)
            {
                myMember.LastName = myDelegate.A1LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (myMember.Mi != myDelegate.A1MiddleName)
            {
                myMember.Mi = myDelegate.A1MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (myMember.Suffix != myDelegate.A1Suffix)
            {
                myMember.Suffix = myDelegate.A1Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (myMember.Address != myDelegate.A1Address1)
            {
                myMember.Address = myDelegate.A1Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddInfo2UpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (myMember.City != myDelegate.A1City)
            {
                myMember.City = myDelegate.A1City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (myMember.State != myDelegate.A1State)
            {
                myMember.State = myDelegate.A1State;
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (myMember.PostalCode != myDelegate.A1ZipCode)
            {
                myMember.PostalCode = myDelegate.A1ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (myMember.Phone != myDelegate.A1Phone)
            {
                myMember.Phone = myDelegate.A1Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (myMember.Email != myDelegate.A1Email)
            {
                myMember.Email = myDelegate.A1Email;
                myMember.EmailUpdated = DateTime.Now;
                myMember.EmailUpdatedBy = UpdatedBy;
                isUpdated = true;
            }

            return isUpdated;
        }
        private bool FillA2(ref TblMasMember myMember, CvnImpDelegate myDelegate)
        {
            string UpdatedBy = "Delgate Import API";
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
            if (myMember.FirstName != myDelegate.A2FirstName)
            {
                myMember.FirstName = myDelegate.A2FirstName;
                myMember.FirstNameUpdated = DateTime.Now;
                myMember.FirstNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // LASTNAME
            if (myMember.LastName != myDelegate.A2LastName)
            {
                myMember.LastName = myDelegate.A2LastName;
                myMember.LastNameUpdated = DateTime.Now;
                myMember.LastNameUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // MI
            if (myMember.Mi != myDelegate.A2MiddleName)
            {
                myMember.Mi = myDelegate.A2MiddleName;
                myMember.Miupdated = DateTime.Now;
                myMember.MiupdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // SUFFIX
            if (myMember.Suffix != myDelegate.A2Suffix)
            {
                myMember.Suffix = myDelegate.A2Suffix;
                myMember.SuffixUpdated = DateTime.Now;
                myMember.SuffixUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // ADDRESS
            if (myMember.Address != myDelegate.A2Address1)
            {
                myMember.Address = myDelegate.A2Address1;
                myMember.AddressUpdated = DateTime.Now;
                myMember.AddInfo2UpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // CITY
            if (myMember.City != myDelegate.A2City)
            {
                myMember.City = myDelegate.A2City;
                myMember.CityUpdated = DateTime.Now;
                myMember.CityUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // STATE
            if (myMember.State != myDelegate.A2State)
            {
                myMember.State = myDelegate.A2State;
                myMember.StateUpdated = DateTime.Now;
                myMember.StateUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // POSTALCODE
            if (myMember.PostalCode != myDelegate.A2ZipCode)
            {
                myMember.PostalCode = myDelegate.A2ZipCode;
                myMember.PostalCodeUpdated = DateTime.Now;
                myMember.PostalCodeUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // PHONE
            if (myMember.Phone != myDelegate.A2Phone)
            {
                myMember.Phone = myDelegate.A2Phone;
                myMember.PhoneUpdated = DateTime.Now;
                myMember.PhoneUpdatedBy = UpdatedBy;
                isUpdated = true;
            }
            // EMAIL
            if (myMember.Email != myDelegate.A2Email)
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
        private async void AddDelegate(int KofCID, int OfficeId, Guid guid)
        {
            try
            {
                var apiHelper = new ApiHelper(_dataSetService);

                TblCorrMemberOffice tblCorrMemberOffice = new TblCorrMemberOffice();
                TblMasMember myID = await apiHelper.GetAsync<TblMasMember>($"/IsKofCMember/{KofCID}");
                if (myID == null)
                {
                    WriteToDelegateImportLog(guid, 0, "INFO", $"Unable to add Office {OfficeId} for missing Member {KofCID}");
                    return;
                }
                tblCorrMemberOffice.OfficeId = OfficeId;
                tblCorrMemberOffice.MemberId = myID.MemberId;
                tblCorrMemberOffice.PrimaryOffice = false;
                tblCorrMemberOffice.Year = await apiHelper.GetAsync<int>($"/GetFratYear/{0}");
                tblCorrMemberOffice.District = null;
                tblCorrMemberOffice.Council = null;
                tblCorrMemberOffice.Assembly = null;

                if (_postFlag) { await apiHelper.PostAsync<TblCorrMemberOffice, TblCorrMemberOffice>("/MemberOffice", tblCorrMemberOffice); };
                WriteToDelegateImportLog(guid, myID.MemberId, "INFO", $"Adding Office {OfficeId} to Member {myID.MemberId}");
            }
            catch (Exception ex)
            {
                Log.Error(Utils.FormatLogEntry(this, ex));
                WriteToDelegateImportLog(guid, KofCID, "ERROR", $"Adding Office {OfficeId} to Member {KofCID}");
            }

        }
        private bool HasDupCouncils(List<CvnImpDelegate> model)
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
    }

}

