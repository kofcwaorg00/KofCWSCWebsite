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

namespace KofCWSCWebsite.Data
{
    public class DataSetService
    {
        private IConfiguration _configuration;
        private readonly ApplicationDbContext _context;
        private IWebHostEnvironment? _hostingEnvironment;
        public string ReportsPath { get; private set; }
        public DataSet DataSet { get; private set; } = new DataSet();

        public DataSetService(ApplicationDbContext context, IWebHostEnvironment hostingEnvironment,IConfiguration configuration)
        {
            _hostingEnvironment = hostingEnvironment;
            _context = context;
            _configuration = configuration;
            SetReportsFolder();
            //SetDataSet();
        }

        private void SetReportsFolder() => ReportsPath = FindReportsFolder(_hostingEnvironment.WebRootPath);
        //private void SetDataSet() => _context.TblMasMembers.ToListAsync().Wait();

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
        public void SetConnectStrings(ApplicationDbContext context,FastReport.Report myReport)
        {
            var conn = context.Database.GetDbConnection();
            var myConn = conn?.ConnectionString;
            Debug.Write(myConn);
            for (int i = 0; i < myReport.Dictionary.Connections.Count; i++)
            {
                myReport.Dictionary.Connections[i].ConnectionString = myConn;
            }
        }
        
        public Report PrepareReport(Report report,string APIMethod, IConfiguration _conf,string param)
        {
            try
            {
                //***************************************************************************************************************
                // 6/6/2024 Tim Philomneo
                //  so trying to encapsulate the nasty FastReports JSON stuff here so we can just call it
                //  from the controllers and it will just work
                //***************************************************************************************************************
                var myAPIURL = (string?)_conf.GetSection("FRConnectStrings").GetValue(typeof(string), "AZDEVAPIURL");
                var mySchema = (string?)_conf.GetSection("FRConnectStrings").GetValue(typeof(string), "GetLabelByOffice");
                var myPrefix = "Json=";
                ///--------------------------------------------------------------------------------------------------------------
                // So I can't find a way to use a parameter to pass to the API URL in the FastReports API.  So, the only choice I
                // have is to build the myJSONCON as a string and substituitng the paramerter then  setting it
                //var myJSONCON = "Json=https://dev.kofc-wa.org/API/GetLabelByOffice/17;JsonSchema='{\"type\":\"array\",\"items\":{\"type\":\"object\",\"properties\":{\"district\":{\"type\":\"number\"},\"altOfficeDescription\":{\"type\":\"string\"},\"firstName\":{\"type\":\"string\"},\"lastName\":{\"type\":\"string\"},\"address\":{\"type\":\"string\"},\"council\":{\"type\":\"number\"},\"assembly\":{\"type\":\"number\"},\"city\":{\"type\":\"string\"},\"state\":{\"type\":\"string\"},\"postalCode\":{\"type\":\"string\"},\"officeDescription\":{\"type\":\"string\"},\"officeID\":{\"type\":\"number\"},\"councilName\":{\"type\":\"string\"},\"fullName\":{\"type\":\"string\"},\"csz\":{\"type\":\"string\"}}}}';Encoding=utf-8;SimpleStructure=false";
                var myJSONCON = myPrefix + myAPIURL + APIMethod + param + mySchema;
                ///--------------------------------------------------------------------------------------------------------------
                report.Dictionary.Connections[0].ConnectionString = myJSONCON;

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
                _myBaseAddress = (string)_configuration.GetSection("APIURL").GetValue(typeof(string), "AZDEV");

                if (_myBaseAddress.IsNullOrEmpty())
                {
                    Log.Fatal("No API URI Initialized");
                    throw new Exception("API URI is not set");
                }
                return _myBaseAddress;
            }
            catch (Exception)
            {
                return string.Empty;
            }

        }

        
    }
    
}
