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


var builder = WebApplication.CreateBuilder(args);

//******************************************************************************************************************************
// 6/6/2024 Tim Philomneo
//  Getting the connect string.  We have switched from the local appsettings.json to using KeyVault.
//  we have 2 vaults, PROD and DEV (see the KV section in appsettings.json  Let's keep the local connection
//  but to use it you will need to comment out the next 4 lines and uncoment the 5th one.
//  Securtiy to KeyVault is handled by the DefaultAzureCredential.  It will use your VS login if you are running
//  in Visual Studio or the Azure Application Identity when published.
//******************************************************************************************************************************
var kvURL = builder.Configuration.GetSection("KV").GetValue(typeof(string),"KVDev");
var client = new SecretClient(new Uri((string)kvURL), new DefaultAzureCredential());
var cnString = client.GetSecret("DASPDEV").Value;
var connectionString = cnString.Value;
//var connectionString = builder.Configuration.GetConnectionString("DevConnection") ?? throw new InvalidOperationException("Connection string 'DefaultConnection' not found.");
//******************************************************************************************************************************

builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlServer(connectionString));

builder.Services.AddDbContext<IdentityDBContext>(options =>
    options.UseSqlServer(connectionString));

// changed this to add IdentiyRole too
builder.Services.AddDefaultIdentity<KofCUser>(options => options.SignIn.RequireConfirmedAccount = true)
    .AddRoles<IdentityRole>().AddEntityFrameworkStores<IdentityDBContext>();

builder.Services.AddDatabaseDeveloperPageExceptionFilter();

//builder.Services.AddApplicationInsightsTelemetry();
//builder.Host.AddSerilogLogging(builder.Configuration);

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
