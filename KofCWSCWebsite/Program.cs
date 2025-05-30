using KofCWSCWebsite.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Areas.Identity.Data;
using KofCWSCWebsite.Services;
using Azure.Identity;
using Azure.Security.KeyVault.Secrets;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Serilog.AspNetCore;
using Serilog.Sinks.Email;
using Serilog.Extensions.Hosting;
using Serilog.Enrichers;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http.Features;
using System.Net;

var builder = WebApplication.CreateBuilder(args);

KeyVaultHelper kvh = new KeyVaultHelper(builder.Configuration);
string azureConnectionString = kvh.GetSecret("AZEmailConnString");

// Setup to use KeyVault
// KeyVaultHelper is now handleing this
//var kvURLAZ = builder.Configuration.GetSection("KV").GetValue(typeof(string), "VAULTURL");
//var kvclient = new SecretClient(new Uri((string)kvURLAZ), new DefaultAzureCredential());
//////////// Get the AZEmailString
//////////var vConnString = kvclient.GetSecret("AZEmailConnString").Value;
//////////string azureConnectionString = vConnString.Value;

// Read the Azure connection string from configuration
//string azureConnectionString = builder.Configuration["Azure:CommunicationService:ConnectionString"];
string fromEmail = builder.Configuration["Azure:CommunicationService:FromEmail"];
string toEmail = builder.Configuration["Azure:CommunicationService:ToEmail"];

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/MyAppLog.txt", retainedFileCountLimit: 21, rollingInterval: RollingInterval.Day, shared: true)
    .WriteTo.Sink(new AzureCommunicationEmailSink(azureConnectionString, fromEmail, toEmail))
    .CreateLogger();

Log.Information("Initialized Serilog and Starting Application");
Log.Information("ENV = " + Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT"));
//******************************************************************************************************************************
// 6/6/2024 Tim Philomneo
//  Getting the connect string.  We have switched from the local appsettings.json to using KeyVault.
//  we have 2 vaults, PROD and DEV (see the KV section in appsettings.json  Let's keep the local connection
//  but to use it you will need to comment out the next 4 lines and uncoment the 5th one.
//  Securtiy to KeyVault is handled by the DefaultAzureCredential.  It will use your VS login if you are running
//  in Visual Studio or the Azure Application Identity when published.
//******************************************************************************************************************************
try
{
    string connectionString = string.Empty;
    //**************************************************************************************************
    // Secrets for sql server db connect strings
    // DBCONN = KofCWSC sql server KofCWSCWeb
    // AZPROD = KofCWSC sql server KofCWSCWeb
    // AZDEV = KofCWSC sql server KofCWSCWebDev
    // DBCONNLOC = Tim's local sql server KofCWSCWebSite
    // DBCONNLOCMARC = Marcus' local sql server KofCWeb
    //---------------------------------------------------------------------------------------------------
    string myEnv = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT").ToLower();
    switch (myEnv)
    {
        case "production":
            connectionString = kvh.GetSecret("AZPROD");
            break;
        case "development":
            connectionString = kvh.GetSecret("DBCONNLOC");
            break;
        case "test":
            connectionString = kvh.GetSecret("AZDEV");
            break;
        default:
            connectionString = kvh.GetSecret("AZPROD");
            break;
    }
    //string connectionString = cnString.Value;

    //------------------------------------------------------------------------------------------------------------------------------
    // make sure we have a value from KeyVault. if not throw an exception
    if (connectionString.IsNullOrEmpty()) throw new Exception("APIURL is not defined");
    //------------------------------------------------------------------------------------------------------------------------------
    builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(
        connectionString,
        sqlServerOptions => sqlServerOptions.EnableRetryOnFailure(
            maxRetryCount: 5,
            maxRetryDelay: TimeSpan.FromSeconds(30),
            errorNumbersToAdd: null
        )
    )
);


    builder.Services.AddDbContext<IdentityDBContext>(options =>
        options.UseSqlServer(connectionString));

}
catch (Exception ex)
{
    Log.Error(ex.Message + " - " + ex.InnerException);
    throw;
}
//*******************************************************************************************
// 9/13/2024 Tim Philomeno
// added this based on Dangs running of a security tool on our site.
//-------------------------------------------------------------------------------------------
//builder.Services.AddResponseCompression(options =>
//{
//    var gzip = options.Providers.OfType<GzipCompressionProvider>().FirstOrDefault();
//    if (gzip != null)
//    {
//        options.Providers.Remove(gzip);
//    }
//    //options.EnableForHttps = false;
//});
//*******************************************************************************************
// changed this to add IdentiyRole too
try
{
    builder.Services.AddDefaultIdentity<KofCUser>
        (options =>
        {
            options.SignIn.RequireConfirmedAccount = true;
            options.Password.RequiredLength = 8;

        })
        .AddRoles<IdentityRole>()
        .AddEntityFrameworkStores<IdentityDBContext>();

    // Force Identity's security stamp to be validated every minute.
    builder.Services.Configure<SecurityStampValidatorOptions>(o =>
                       o.ValidationInterval = TimeSpan.FromMinutes(1));
}
catch (Exception ex)
{
    Log.Error(ex.Message + " - " + ex.InnerException);
    throw;
}

// implement a session timeout
try
{
    builder.Services.ConfigureApplicationCookie(options =>
    {
        options.ExpireTimeSpan = TimeSpan.FromMinutes(60);
        options.LoginPath = "/Identity/Account/Login";
        // ReturnUrlParameter requires 
        //using Microsoft.AspNetCore.Authentication.Cookies;
        options.ReturnUrlParameter = CookieAuthenticationDefaults.ReturnUrlParameter;
        options.SlidingExpiration = true;
    });
}
catch (Exception ex)
{
    Log.Error(ex.Message + " - " + ex.InnerException);
    throw;
}
builder.Services.Configure<FormOptions>(options => { options.ValueCountLimit = builder.Configuration.GetValue<int>("AspNetCore:FormOptions:ValueCountLimit"); });
builder.Services.AddScoped<DataSetService, DataSetService>();
builder.Services.AddScoped<ApiHelper, ApiHelper>();
builder.Services.AddScoped<HttpClient, HttpClient>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

// this was added to properly handle binding to date controls on forms
//builder.Services.AddControllersWithViews()
builder.Services.AddControllersWithViews()
        .AddMvcOptions(options =>
        {
            var supportedCultures = new[] { "en-US" };
            var localizationOptions = new RequestLocalizationOptions()
                .SetDefaultCulture(supportedCultures[0])
                .AddSupportedCultures(supportedCultures)
                .AddSupportedUICultures(supportedCultures);

            options.ModelBinderProviders.Insert(0, new Microsoft.AspNetCore.Mvc.ModelBinding.Binders.DateTimeModelBinderProvider());
        });

builder.Services.AddTransient<ISenderEmail, EmailSender>();

builder.Services.AddFastReport();


var app = builder.Build();

// documentation says to call this before UseMvc or UseEndpoints
app.UseFastReport();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    //////////////app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();
//*******************************************************************
// added this to prevent extra requests to get favicon
app.Use(async (context, next) =>
{
    if (context.Request.Path.Value.Contains("favicon.ico"))
    {
        context.Response.StatusCode = 204; // No Content
        return;
    }
    await next();
});
//*******************************************************************
app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
