using FastReport;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Infrastructure;
using Microsoft.Extensions.Configuration;
using System.Configuration;
using System.Data;
using System.Diagnostics;
using Serilog;
using com.sun.xml.@internal.rngom.dt;
using Microsoft.IdentityModel.Tokens;
using sun.misc;

namespace KofCWSCWebsite.Data
{
    public class DataSetService
    {
        private IConfiguration _configuration;
        private IWebHostEnvironment? _hostingEnvironment;
        private IHttpContextAccessor _httpContextAccessor;
        public string ReportsPath { get; private set; }
        //public DataSet DataSet { get; private set; } = new DataSet();

        public DataSetService(IWebHostEnvironment hostingEnvironment, IConfiguration configuration, IHttpContextAccessor httpContextAccessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _configuration = configuration;
            _httpContextAccessor = httpContextAccessor;
            SetReportsFolder();
        }

        private void SetReportsFolder() => ReportsPath = FindReportsFolder(_hostingEnvironment.WebRootPath);
        //private void SetDataSet() => _context.TblMasMembers.ToListAsync().Wait();

        public string GetMyHost()
        {
            var myhost = _configuration["Host:HostName"];
            if (myhost == null)
            {
                return "https://localhost:7213";
            }
            else
            {
                return myhost;
            }


        }
        private string FindReportsFolder(string startDir)
        {
            string directory = Path.Combine(startDir, "Reports");
            if (Directory.Exists(directory))
                return directory;

            for (int i = 0; i < 6; i++)
            {
                startDir = Path.Combine(startDir, "..");
                directory = Path.Combine(startDir, "Reports");
                if (Directory.Exists(directory))
                    return directory;
            }

            throw new Exception(directory);
        }
        // this will cycle through all connection strings and set them to the current production string
        //public void SetConnectStrings(ApplicationDbContext context,FastReport.Report myReport)
        //{
        //    var conn = context.Database.GetDbConnection();
        //    var myConn = conn?.ConnectionString;
        //    Debug.Write(myConn);
        //    for (int i = 0; i < myReport.Dictionary.Connections.Count; i++)
        //    {
        //        myReport.Dictionary.Connections[i].ConnectionString = myConn;
        //    }
        //}

        public Report PrepareReport(Report report, IConfiguration _conf, int param1, int param2 = 9, int param3 = 9)
        {
            //***************************************************************************************************************
            // 10/05/2024 Tim Philomeno
            // modified to use an optional parameter for the 2nd parm to support the directory
            // parm1 is the ShortForm and parm2 is the NextYear
            //-----------------------------------------------------------------------------------------------------------------
            try
            {
                //***************************************************************************************************************
                // 6/6/2024 Tim Philomneo
                //  so trying to encapsulate the nasty FastReports JSON stuff here so we can just call it
                //  from the controllers and it will just work
                // 10/02/2024 Tim Philomeno
                // so the report connection strings are already set for all reports.  All we have to do is modify
                // what is stored in the report itself.  i.e. change the HOST section and add the PARAM
                //***************************************************************************************************************
                for (int i = 0; i < report.Dictionary.Connections.Count; i++)
                {
                    string myHost = (string?)_conf.GetSection("APIURL").GetValue(typeof(string), "APIURL");
                    string currString = report.Dictionary.Connections[i].ConnectionString;
                    int currStringLen = currString.Length;
                    int currStringSemi = currString.IndexOf(';');

                    string myPre = currString.Split('/')[0];
                    //string myHost = currString.Split('/')[2];
                    string myMethod = currString.Split('/')[3];
                    string schema = currString.Substring(currStringSemi, currStringLen - currStringSemi);
                    if (param2 == 9 && param3 == 9)
                    {
                        report.Dictionary.Connections[i].ConnectionString = myPre + "//" + myHost + "/" + myMethod + "/" + param1 + schema;
                    }
                    else if (param3 == 9)
                    {
                        report.Dictionary.Connections[i].ConnectionString = myPre + "//" + myHost + "/" + myMethod + "/" + param1 + "/" + param2 + schema;
                    }
                    else
                    {
                        report.Dictionary.Connections[i].ConnectionString = myPre + "//" + myHost + "/" + myMethod + "/" + param1 + "/" + param2 + "/" + param3 + schema;
                    }
                }
                return report;
            }
            catch (Exception ex)
            {

                Log.Fatal(ex.Message + " " + ex.InnerException);
                return report;
            }

        }

        public string GetAPIBaseAddress()
        {
            try
            {
                string _myBaseAddress;
                _myBaseAddress = (string)_configuration.GetSection("APIURL").GetValue(typeof(string), "APIURL");

                if (_myBaseAddress.IsNullOrEmpty())
                {
                    Log.Fatal("No API URI Initialized");
                    throw new Exception("API URI is not set");
                }
                return "https://" + _myBaseAddress;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }
        public Dictionary<string,string> GetStates()
        {
            var states = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    {
        { "Alabama", "AL" },
        { "Alaska", "AK" },
        { "Arizona", "AZ" },
        { "Arkansas", "AR" },
        { "California", "CA" },
        { "Colorado", "CO" },
        { "Connecticut", "CT" },
        { "Delaware", "DE" },
        { "Florida", "FL" },
        { "Georgia", "GA" },
        { "Hawaii", "HI" },
        { "Idaho", "ID" },
        { "Illinois", "IL" },
        { "Indiana", "IN" },
        { "Iowa", "IA" },
        { "Kansas", "KS" },
        { "Kentucky", "KY" },
        { "Louisiana", "LA" },
        { "Maine", "ME" },
        { "Maryland", "MD" },
        { "Massachusetts", "MA" },
        { "Michigan", "MI" },
        { "Minnesota", "MN" },
        { "Mississippi", "MS" },
        { "Missouri", "MO" },
        { "Montana", "MT" },
        { "Nebraska", "NE" },
        { "Nevada", "NV" },
        { "New Hampshire", "NH" },
        { "New Jersey", "NJ" },
        { "New Mexico", "NM" },
        { "New York", "NY" },
        { "North Carolina", "NC" },
        { "North Dakota", "ND" },
        { "Ohio", "OH" },
        { "Oklahoma", "OK" },
        { "Oregon", "OR" },
        { "Pennsylvania", "PA" },
        { "Rhode Island", "RI" },
        { "South Carolina", "SC" },
        { "South Dakota", "SD" },
        { "Tennessee", "TN" },
        { "Texas", "TX" },
        { "Utah", "UT" },
        { "Vermont", "VT" },
        { "Virginia", "VA" },
        { "Washington", "WA" },
        { "West Virginia", "WV" },
        { "Wisconsin", "WI" },
        { "Wyoming", "WY" }
    };
            return states;
        }
        public string GetStateAbbreviation(string stateName)
        {
            var mystates = GetStates();
    //        var states = new Dictionary<string, string>(StringComparer.OrdinalIgnoreCase)
    //{
    //    { "Alabama", "AL" },
    //    { "Alaska", "AK" },
    //    { "Arizona", "AZ" },
    //    { "Arkansas", "AR" },
    //    { "California", "CA" },
    //    { "Colorado", "CO" },
    //    { "Connecticut", "CT" },
    //    { "Delaware", "DE" },
    //    { "Florida", "FL" },
    //    { "Georgia", "GA" },
    //    { "Hawaii", "HI" },
    //    { "Idaho", "ID" },
    //    { "Illinois", "IL" },
    //    { "Indiana", "IN" },
    //    { "Iowa", "IA" },
    //    { "Kansas", "KS" },
    //    { "Kentucky", "KY" },
    //    { "Louisiana", "LA" },
    //    { "Maine", "ME" },
    //    { "Maryland", "MD" },
    //    { "Massachusetts", "MA" },
    //    { "Michigan", "MI" },
    //    { "Minnesota", "MN" },
    //    { "Mississippi", "MS" },
    //    { "Missouri", "MO" },
    //    { "Montana", "MT" },
    //    { "Nebraska", "NE" },
    //    { "Nevada", "NV" },
    //    { "New Hampshire", "NH" },
    //    { "New Jersey", "NJ" },
    //    { "New Mexico", "NM" },
    //    { "New York", "NY" },
    //    { "North Carolina", "NC" },
    //    { "North Dakota", "ND" },
    //    { "Ohio", "OH" },
    //    { "Oklahoma", "OK" },
    //    { "Oregon", "OR" },
    //    { "Pennsylvania", "PA" },
    //    { "Rhode Island", "RI" },
    //    { "South Carolina", "SC" },
    //    { "South Dakota", "SD" },
    //    { "Tennessee", "TN" },
    //    { "Texas", "TX" },
    //    { "Utah", "UT" },
    //    { "Vermont", "VT" },
    //    { "Virginia", "VA" },
    //    { "Washington", "WA" },
    //    { "West Virginia", "WV" },
    //    { "Wisconsin", "WI" },
    //    { "Wyoming", "WY" }
    //};

            if (mystates.TryGetValue(stateName, out string abbreviation))
            {
                return abbreviation;
            }
            else
            {
                throw new ArgumentException($"State name '{stateName}' is not recognized.");
            }
        }



    }

}
