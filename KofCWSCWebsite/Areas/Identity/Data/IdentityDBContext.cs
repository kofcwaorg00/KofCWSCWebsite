using KofCWSCWebsite.Areas.Identity.Data;
using KofCWSCWebsite.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using Serilog;

namespace KofCWSCWebsite.Areas.Identity.Data;

public class IdentityDBContext : IdentityDbContext<KofCUser>
{
    public IdentityDBContext(DbContextOptions<IdentityDBContext> options)
        : base(options)
    {
        Log.Information("IdentityDBContext ctor ran.");
    }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);
        // Customize the ASP.NET Identity model and override the defaults if needed.
        // For example, you can rename the ASP.NET Identity table names and more.
        // Add your customizations after calling base.OnModelCreating(builder);
        builder.Entity<KofCUser>(entity =>
        {

            entity.ToTable("aspnetusers", tb =>
            {
                // this is because our table has triggers
                tb.UseSqlOutputClause(false);

            });
        });
    }
}
