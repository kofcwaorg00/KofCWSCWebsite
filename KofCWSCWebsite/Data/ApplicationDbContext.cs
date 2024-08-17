using System;
using System.Collections.Generic;
using Azure.Identity;
using KofCWSCWebsite.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Serilog;

namespace KofCWSCWebsite.Data;

public partial class ApplicationDbContext : DbContext
{
    private static bool isKVInit;
    public ApplicationDbContext()
    {
    }

    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
        //*****************************************************************************************************************
        // 6/14/2024 Tim Philomeno
        // this is the magic code that allows client permissions for SQL Server to get its master encryption key from KeyVault
        // the authentication is done using DefaultAzureCredential.  That cycles through multple types
        // of authentication.  For the develoment environment, it uses the credentials that the developer has used to
        // login to Visual Studio.  For publised environments, you need to setup Azure Identity to allow
        // the applicaiton to authenticate and get access to KeyVault
        // NOTE: THE AZURE DEPENDENCY WILL BE REMOVED WHEN THIS SITE IS FULLY USING THE API
        //*****************************************************************************************************************
        try
        {
            if (!isKVInit)
            {
                SqlColumnEncryptionAzureKeyVaultProvider akvProvider = new SqlColumnEncryptionAzureKeyVaultProvider(new DefaultAzureCredential());
                SqlConnection.RegisterColumnEncryptionKeyStoreProviders(customProviders: new Dictionary<string, SqlColumnEncryptionKeyStoreProvider>(capacity: 1, comparer: StringComparer.OrdinalIgnoreCase)
            {
                    { SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider}
            });
            }
            isKVInit = true;
        }
        catch (Exception ex)
        {
            Log.Error(GetType() + " " + ex.Message + " " + ex.InnerException);
            throw new Exception("SQL Azure Key Vault Initialization Failed");
        }
    }

    public virtual DbSet<TblValCouncil> TblValCouncils { get; set; }

    public virtual DbSet<TblWebSelfPublish> TblWebSelfPublishes { get; set; }

    public virtual DbSet<TblMasPso> TblMasPsos { get; set; }

    public virtual DbSet<TblMasAward> TblMasAwards { get; set; }

    public virtual DbSet<TblMasMember> TblMasMembers { get; set; }
    public virtual DbSet<KofCMemberIDUsers> KofCMemberIDUsers { get; set; }
    public virtual DbSet<TblCorrMemberOffice> TblCorrMemberOffices { get; set; }

    public virtual DbSet<TblValOffice> TblValOffices { get; set; }

    public virtual DbSet<MemberVM> funSYS_BuildName { get; set; }

    public virtual DbSet<TblWebTrxAoi> TblWebTrxAois { get; set; }

    public virtual DbSet<TblSysTrxEvent> TblSysTrxEvents { get; set; }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        if (!optionsBuilder.IsConfigured)
        {
#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see http://go.microsoft.com/fwlink/?LinkId=723263.
            //optionsBuilder.UseSqlServer("Data Source=tcp:sql2k805.discountasp.net;Initial Catalog=SQL2008R2_137411_kofcwsc;User ID=SQL2008R2_137411_kofcwsc_user;Password=S1995KC;");
        }
    }


    //    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //#warning To protect potentially sensitive information in your connection string, you should move it out of source code. You can avoid scaffolding the connection string by using the Name= syntax to read it from configuration - see https://go.microsoft.com/fwlink/?linkid=2131148. For more guidance on storing connection strings, see https://go.microsoft.com/fwlink/?LinkId=723263.
    //        => optionsBuilder.UseSqlServer("Data Source=sql2k805.discountasp.net;AttachDbFilename=;Initial Catalog=SQL2008R2_137411_kofcwsc;Integrated Security=False;Persist Security Info=True;User ID=SQL2008R2_137411_kofcwsc_user;Password=S1995KC;TrustServerCertificate=True");

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<MemberVM>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<TblValCouncil>(entity =>
        {
            entity.HasKey(e => e.CNumber)
                .HasName("aaaaatbl_ValCouncils_PK")
                .IsClustered(false);

            entity.ToTable("tbl_ValCouncils");

            entity.Property(e => e.CNumber)
                .ValueGeneratedNever()
                .HasColumnName("C_NUMBER");
            entity.Property(e => e.AddInfo1)
                .HasMaxLength(60)
                .HasColumnName("ADD INFO 1");
            entity.Property(e => e.AddInfo2)
                .HasMaxLength(60)
                .HasColumnName("ADD INFO 2");
            entity.Property(e => e.AddInfo3)
                .HasMaxLength(50)
                .HasColumnName("ADD INFO 3");
            entity.Property(e => e.Arbalance)
                .HasColumnType("numeric(14, 2)")
                .HasColumnName("ARBalance");
            entity.Property(e => e.BulletinUrl).HasColumnName("BulletinURL");
            entity.Property(e => e.CLocation)
                .HasMaxLength(50)
                .HasColumnName("C_LOCATION");
            entity.Property(e => e.CName)
                .HasMaxLength(32)
                .HasColumnName("C_NAME");
            entity.Property(e => e.Chartered).HasColumnType("datetime");
            entity.Property(e => e.DioceseId)
                .HasMaxLength(3)
                .HasColumnName("DioceseID");
            entity.Property(e => e.District).HasColumnName("DISTRICT");
            entity.Property(e => e.LiabIns).HasDefaultValue(false);
            entity.Property(e => e.Status)
                .HasMaxLength(1)
                .HasDefaultValue("A")
                .IsFixedLength();
            entity.Property(e => e.WebSiteUrl).HasColumnName("WebSiteURL");
        });

        modelBuilder.Entity<TblWebSelfPublish>(entity =>
        {
            entity.HasKey(e => e.Url).HasName("PK_tblWEB_SelfPublish_1");

            entity.ToTable("tblWEB_SelfPublish");

            entity.Property(e => e.Url)
                .HasMaxLength(400)
                .HasColumnName("URL");
            entity.Property(e => e.Data)
                .HasColumnType("text");
            entity.Property(e => e.OID)
                .HasColumnType("int")
                .HasColumnName("OID");
        });

        modelBuilder.Entity<TblMasPso>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_MasP__3214EC27821953F8");

            entity.ToTable("tbl_MasPSOs");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.StateAdvocate)
                .HasMaxLength(255)
                .HasColumnName("State Advocate");
            entity.Property(e => e.StateDeputy)
                .HasMaxLength(255)
                .HasColumnName("State Deputy");
            entity.Property(e => e.StateSecretary)
                .HasMaxLength(255)
                .HasColumnName("State Secretary");
            entity.Property(e => e.StateTreasurer)
                .HasMaxLength(255)
                .HasColumnName("State Treasurer");
            entity.Property(e => e.StateWarden)
                .HasMaxLength(255)
                .HasColumnName("State Warden");
        });

        modelBuilder.Entity<TblMasAward>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tbl_MasA__3214EC078DBBE52E");

            entity.ToTable("tbl_MasAwards");

            entity.Property(e => e.AwardDescription)
                .HasMaxLength(255)
                .HasColumnName("Award Description");
            entity.Property(e => e.AwardDueDate)
                .HasColumnType("datetime")
                .HasColumnName("Award Due Date");
            entity.Property(e => e.AwardName)
                .HasMaxLength(255)
                .HasColumnName("Award Name");
            entity.Property(e => e.AwardSubmissionEmailAddress)
                .HasMaxLength(255)
                .HasColumnName("Award Submission Email Address");
            entity.Property(e => e.LinkToTheAwardForm)
                .HasMaxLength(255)
                .HasColumnName("Link to the Award Form");
        });

        modelBuilder.Entity<TblValAssy>(entity =>
        {
            entity.HasKey(e => e.ANumber)
                .HasName("aaaaatbl_ValAssys_PK")
                .IsClustered(false);

            entity.ToTable("tbl_ValAssys");

            entity.Property(e => e.ANumber)
                .ValueGeneratedNever()
                .HasColumnName("A_NUMBER");
            entity.Property(e => e.ALocation)
                .HasMaxLength(50)
                .HasColumnName("A_LOCATION");
            entity.Property(e => e.AName)
                .HasMaxLength(50)
                .HasColumnName("A_NAME");
            entity.Property(e => e.AddInfo1)
                .HasMaxLength(60)
                .HasColumnName("ADD INFO 1");
            entity.Property(e => e.AddInfo2)
                .HasMaxLength(60)
                .HasColumnName("ADD INFO 2");
            entity.Property(e => e.AddInfo3)
                .HasMaxLength(60)
                .HasColumnName("ADD INFO 3");
            entity.Property(e => e.MasterLoc)
                .HasMaxLength(1)
                .IsFixedLength();
            entity.Property(e => e.WebSiteUrl).HasColumnName("WebSiteURL");
        });

        modelBuilder.Entity<TblValOffice>(entity =>
        {
            entity.HasKey(e => e.OfficeId)
                .HasName("aaaaatbl_ValOffices_PK")
                .IsClustered(false);

            entity.ToTable("tbl_ValOffices", tb =>
            {
                tb.HasTrigger("T_tbl_ValOffices_DTrig");
                tb.HasTrigger("T_tbl_ValOffices_UTrig");
            });

            entity.Property(e => e.OfficeId).HasColumnName("OfficeID");
            entity.Property(e => e.AltDescription).HasMaxLength(75);
            entity.Property(e => e.EmailAlias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.OfficeDescription).HasMaxLength(75);
            entity.Property(e => e.SupremeUrl).HasColumnName("SupremeURL");
            entity.Property(e => e.UseAsFormalTitle).HasDefaultValue(false);
        });

        modelBuilder.Entity<TblMasMember>(entity =>
        {
            entity.HasKey(e => e.MemberId);

            entity.ToTable("tbl_MasMembers", tb =>
            {
                // this is because our table has triggers
                tb.UseSqlOutputClause(false);

                tb.HasTrigger("T_tbl_MasMembers_DTrig");
                tb.HasTrigger("T_tbl_MasMembers_U1Trig");
                tb.HasTrigger("T_tbl_MasMembers_UTrig");
            });

            entity.HasIndex(e => e.KofCid, "NonClusteredIndex-20130527-110545").IsUnique();

            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Address).HasMaxLength(255);
            entity.Property(e => e.City).HasMaxLength(50);
            entity.Property(e => e.Email).HasMaxLength(50);
            entity.Property(e => e.KofCid)
                .HasMaxLength(7)
                .HasColumnName("KofCID");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.Mi)
                .HasMaxLength(50)
                .HasColumnName("MI");
            entity.Property(e => e.Phone).HasMaxLength(30);
            entity.Property(e => e.PostalCode).HasMaxLength(20);
            entity.Property(e => e.State).HasMaxLength(20);
        });

        modelBuilder.Entity<TblCorrMemberOffice>(entity =>
        {
            entity.ToTable("tbl_CorrMemberOffice");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.OfficeId).HasColumnName("OfficeID");
            entity.Property(e => e.Year).HasDefaultValue(2024);

        });

        modelBuilder.Entity<TblValOffice>(entity =>
        {
            entity.HasKey(e => e.OfficeId)
                .HasName("aaaaatbl_ValOffices_PK")
                .IsClustered(false);

            entity.ToTable("tbl_ValOffices", tb =>
            {
                tb.HasTrigger("T_tbl_ValOffices_DTrig");
                tb.HasTrigger("T_tbl_ValOffices_UTrig");
            });

            entity.Property(e => e.OfficeId).HasColumnName("OfficeID");
            entity.Property(e => e.AltDescription).HasMaxLength(75);
            entity.Property(e => e.ContactUsstring)
                .HasMaxLength(200)
                .HasColumnName("ContactUSString");
            entity.Property(e => e.EmailAlias)
                .HasMaxLength(50)
                .IsUnicode(false);
            entity.Property(e => e.GroupId).HasColumnName("GroupID");
            entity.Property(e => e.OfficeDescription).HasMaxLength(75);
            entity.Property(e => e.SupremeUrl).HasColumnName("SupremeURL");
            entity.Property(e => e.UseAsFormalTitle).HasDefaultValue(false);
        });

        modelBuilder.Entity<TblWebTrxAoi>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("tblWEB_TrxAOI");

            entity.Property(e => e.GraphicUrl)
                .HasMaxLength(250)
                .HasColumnName("GraphicURL");
            entity.Property(e => e.LinkUrl)
                .HasMaxLength(250)
                .HasColumnName("LinkURL");
            entity.Property(e => e.PostedDate).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(250);
            entity.Property(e => e.Type)
                .HasMaxLength(2)
                .IsFixedLength();
        });
        modelBuilder.Entity<SPGetSOS>(entity =>
        {
            entity.HasNoKey();
        });

        modelBuilder.Entity<SPGetSOSView>(entity =>
        {
            entity.HasKey(e => e.SortBy);
        });

        modelBuilder.Entity<TblSysTrxEvent>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSYS_t__3214EC07638F8109");

            entity.ToTable("tblSYS_trxEvents");

            entity.Property(e => e.AddedBy).HasMaxLength(50);
            entity.Property(e => e.AttachUrl)
                .HasMaxLength(250)
                .HasColumnName("AttachURL");
            entity.Property(e => e.Begin).HasColumnType("datetime");
            entity.Property(e => e.DateAdded).HasColumnType("datetime");
            entity.Property(e => e.End).HasColumnType("datetime");
            entity.Property(e => e.Title).HasMaxLength(50);
            entity.Property(e => e.isAllDay).HasColumnType("boolean");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<KofCWSCWebsite.Models.TblValAssy> TblValAssy { get; set; } = default!;

public DbSet<KofCWSCWebsite.Models.TblValOffice> TblValOffice { get; set; } = default!;

public DbSet<KofCWSCWebsite.Models.SPGetSOSView> SPGetSOSView { get; set; } = default!;
}

