using KofCWSCWebsite.Data;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using KofCWSCWebsite.Areas.Identity.Data;
using Microsoft.AspNetCore.Identity.UI.Services;
using KofCWSCWebsite.Services;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore;
using Azure.Identity;

using Microsoft.Azure.KeyVault;
using Azure.Security.KeyVault.Secrets;
using Microsoft.AspNetCore.DataProtection;
using Microsoft.IdentityModel.Tokens;
using Serilog;
using Microsoft.AspNet.Identity;

var builder = WebApplication.CreateBuilder(args);

Log.Logger = new LoggerConfiguration()
    .WriteTo.Console()
    .Enrich.FromLogContext()
    .WriteTo.File("logs/MyAppLog.txt", retainedFileCountLimit: 21, rollingInterval: RollingInterval.Day,shared: true)
    .CreateLogger();

Log.Information("Initialized Serilog and Starting Application");
//******************************************************************************************************************************
// 6/6/2024 Tim Philomneo
//  Getting the connect string.  We have switched from the local appsettings.json to using KeyVault.
//  we have 2 vaults, PROD and DEV (see the KV section in appsettings.json  Let's keep the local connection
//  but to use it you will need to comment out the next 4 lines and uncoment the 5th one.
//  Securtiy to KeyVault is handled by the DefaultAzureCredential.  It will use your VS login if you are running
//  in Visual Studio or the Azure Application Identity when published.
//******************************************************************************************************************************
var kvURL = builder.Configuration.GetSection("KV").GetValue(typeof(string), "KVDev");
var client = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
var cnString = client.GetSecret("AZDEV").Value;
string connectionString = cnString.Value;

Log.Information("Found CS " + connectionString);
//------------------------------------------------------------------------------------------------------------------------------
//////////////var connectionString = builder.Configuration.GetConnectionString("DASPDEVConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//------------------------------------------------------------------------------------------------------------------------------
// make sure we have a value from KeyVault. if not throw an exception
if (connectionString.IsNullOrEmpty()) throw new Exception("APIURL is not defined");
//------------------------------------------------------------------------------------------------------------------------------
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IdentityDBContext>(options =>
    options.UseSqlServer(connectionString));

// changed this to add IdentiyRole too
builder.Services.AddDefaultIdentity<KofCUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityDBContext>();

// my attempt at useing userid/email/kofcid...still needed to deal with email and custom validator NOT
//builder.Services.AddDefaultIdentity<KofCUser>(options =>
//{
//    // Specify the allowed characters for usernames (including spaces and special characters)
//    options.User.AllowedUserNameCharacters = "123456789";
//    // Set other identity options as needed
//    options.User.RequireUniqueEmail = true; // Optional: Require unique email addresses
//    options.SignIn.RequireConfirmedAccount = true;
//}).AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityDBContext>(); 




builder.Services.AddDatabaseDeveloperPageExceptionFilter();

builder.Services.AddControllersWithViews();

builder.Services.AddTransient<IEmailSender, EmailSender>();

var app = builder.Build();

// Configure the HTTP request pipeline.
if (app.Environment.IsDevelopment())
{
    app.UseMigrationsEndPoint();
}
else
{
    app.UseExceptionHandler("/Home/Error");
    // The default HSTS value is 30 days. You may want to change this for production scenarios, see https://aka.ms/aspnetcore-hsts.
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();
app.UseAuthentication();
app.UseAuthorization();


app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Home}/{action=Index}/{id?}");
app.MapRazorPages();

app.Run();
