using System;
using System.Collections.Generic;
using Azure.Identity;
using KofCWSCWebsite.Models;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.Data.SqlClient.AlwaysEncrypted.AzureKeyVaultProvider;
using Serilog;
using KofCWSCWebsite.Services;


namespace KofCWSCWebsite.Data;

public partial class ApplicationDbContext : DbContext
{
    private static bool isKVInit = false;
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
                    { 
                        SqlColumnEncryptionAzureKeyVaultProvider.ProviderName, akvProvider
                    }
                });
                isKVInit = true;
            }
            
        }
        catch (Exception ex)
        {
            Log.Error(Utils.FormatLogEntry(this, ex, "in ApplicationDbContext ctor"));
            throw new Exception("SQL Azure Key Vault Initialization Failed");
        }
    }

    public virtual DbSet<TblValCouncil> TblValCouncils { get; set; }
    public virtual DbSet<TblValCouncilMPD> TblValCouncilsMPD { get; set; }
    public virtual DbSet<TblValCouncilFSEdit> TblValCouncilsFSEdit { get; set; }
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
    public virtual DbSet<NextID> NextIDs { get; set; }
    public DbSet<KofCWSCWebsite.Models.TblValAssy> TblValAssys { get; set; } = default!;
    //public DbSet<KofCWSCWebsite.Models.TblValOffice> TblValOffice { get; set; } = default!;
    public DbSet<KofCWSCWebsite.Models.SPGetSOSView> SPGetSOSViews { get; set; } = default!;
    public DbSet<KofCWSCWebsite.Models.SPGetCouncilsView> SPGetCouncilsView { get; set; } = default!;
    public DbSet<KofCWSCWebsite.Models.SPGetAssysView> SPGetAssysView { get; set; } = default!;
    public virtual DbSet<EmailOffice> TblWebTrxEmailOffices { get; set; }

    public virtual DbSet<FileStorage> FileStorages { get; set; }
    public virtual DbSet<FileStorageVM> FileStoragesVM { get; set; }
    public virtual DbSet<CvnControl> TblCvnControls { get; set; }
    public virtual DbSet<CvnImpDelegate> CvnImpDelegates { get; set; }
    public virtual DbSet<CvnImpDelegatesLog> TblCvnImpDelegatesLogs { get; set; }
    public virtual DbSet<CvnDelegateDays> CvnDelegateDays { get; set; }
    public virtual DbSet<CvnMileage> TblCvnMasMileages { get; set; }
    public virtual DbSet<CvnLocation> TblCvnMasLocations { get; set; }
    public virtual DbSet<CvnMpd> TblCvnTrxMpds { get; set; }
    public virtual DbSet<MemberSuspension> TblSysMasMemberSuspensions { get; set; }
    public virtual DbSet<LogCorrMemberOffice> TblLogCorrMemberOffices { get; set; }
    public virtual DbSet<AspNetUserRole> AspNetUserRoles { get; set; }
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
        modelBuilder.Entity<TblValCouncilMPD>(entity =>
        {
            entity.HasKey(e => e.CNumber);
        });
        modelBuilder.Entity<TblValCouncilFSEdit>(entity =>
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
            entity.Property(e => e.KofCid).HasColumnName("KofCID");
            entity.Property(e => e.LastName).HasMaxLength(50);
            entity.Property(e => e.MI)
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

        modelBuilder.Entity<SPGetCouncilsView>(entity =>
        {
            entity.HasKey(e => e.CouncilNo);
        });

        modelBuilder.Entity<SPGetSOSView>(entity =>
        {
            entity.HasKey(e => e.SortBy);
        });

        modelBuilder.Entity<SPGetAssysView>(entity =>
        {
            entity.HasKey(e => e.AssyNo);
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

        modelBuilder.Entity<EmailOffice>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblWEB_EmailOffice");

            entity.ToTable("tblWEB_trxEmailOffice");

            entity.Property(e => e.Dd).HasColumnName("DD");
            entity.Property(e => e.Fc).HasColumnName("FC");
            entity.Property(e => e.Fn).HasColumnName("FN");
            entity.Property(e => e.From).HasMaxLength(50);
            entity.Property(e => e.Fs).HasColumnName("FS");
            entity.Property(e => e.Gk).HasColumnName("GK");
            entity.Property(e => e.DateSent).HasColumnName("DateSent");
            entity.Property(e => e.Subject).HasMaxLength(50);
            entity.Property(e => e.Attachment).HasColumnName("Attachment");
            entity.Ignore("Attachment");
        });

        modelBuilder.Entity<FileStorage>(
            dob =>
            {
                dob.ToTable("tblWEB_FileStorage");
            });
        modelBuilder.Entity<NextID>(entity =>
        {
            entity.HasKey(e => e.NextTempID);
        });

        modelBuilder.Entity<CvnControl>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK_tblCVN_Control1");

            entity.ToTable("tblCVN_Control");

            entity.Property(e => e.Id).HasColumnName("id");
            entity.Property(e => e.LocationString).HasMaxLength(1000);
            entity.Property(e => e.MPDDay).HasColumnName("MPDDay");
            entity.Property(e => e.MPDMile).HasColumnName("MPDMile");
        });
        //modelBuilder.Entity<FileStorageVM>(
        //    ob =>
        //    {
        //        ob.ToTable("tblWEB_FileStorage");
        //        ob.HasOne(o => o.FileStorage).WithOne()
        //            .HasForeignKey<FileStorage>(o => o.Id);
        //        ob.Navigation(o => o.FileStorage).IsRequired();
        //    });
        modelBuilder.Entity<CvnImpDelegate>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.ToTable("tblCVN_ImpDelegates");

            entity.Property(e => e.A1Address1)
                .HasMaxLength(255)
                .HasColumnName("A1Address1");
            entity.Property(e => e.A1Address2)
                .HasMaxLength(255)
                .HasColumnName("A1Address2");
            entity.Property(e => e.A1City)
                .HasMaxLength(255)
                .HasColumnName("A1City");
            entity.Property(e => e.A1Email)
                .HasMaxLength(255)
                .HasColumnName("A1Email");
            entity.Property(e => e.A1FirstName)
                .HasMaxLength(255)
                .HasColumnName("A1FirstName");
            entity.Property(e => e.A1LastName)
                .HasMaxLength(255)
                .HasColumnName("A1LastName");
            entity.Property(e => e.A1MemberID).HasColumnName("A1MemberID");
            entity.Property(e => e.A1MiddleName)
                .HasMaxLength(255)
                .HasColumnName("A1MiddleName");
            entity.Property(e => e.A1Phone)
                .HasMaxLength(255)
                .HasColumnName("A1Phone");
            entity.Property(e => e.A1State)
                .HasMaxLength(255)
                .HasColumnName("A1State");
            entity.Property(e => e.A1Suffix)
                .HasMaxLength(255)
                .HasColumnName("A1Suffix");
            entity.Property(e => e.A1ZipCode)
                .HasMaxLength(255)
                .HasColumnName("A1ZipCode");
            entity.Property(e => e.A2Address1)
                .HasMaxLength(255)
                .HasColumnName("A2Address1");
            entity.Property(e => e.A2Address2)
                .HasMaxLength(255)
                .HasColumnName("A2Address2");
            entity.Property(e => e.A2City)
                .HasMaxLength(255)
                .HasColumnName("A2City");
            entity.Property(e => e.A2Email)
                .HasMaxLength(255)
                .HasColumnName("A2Email");
            entity.Property(e => e.A2FirstName)
                .HasMaxLength(255)
                .HasColumnName("A2FirstName");
            entity.Property(e => e.A2LastName)
                .HasMaxLength(255)
                .HasColumnName("A2LastName");
            entity.Property(e => e.A2MemberID).HasColumnName("A2MemberID");
            entity.Property(e => e.A2MiddleName)
                .HasMaxLength(255)
                .HasColumnName("A2MiddleName");
            entity.Property(e => e.A2Phone)
                .HasMaxLength(255)
                .HasColumnName("A2Phone");
            entity.Property(e => e.A2State)
                .HasMaxLength(255)
                .HasColumnName("A2State");
            entity.Property(e => e.A2Suffix)
                .HasMaxLength(255)
                .HasColumnName("A2Suffix");
            entity.Property(e => e.A2ZipCode)
                .HasMaxLength(255)
                .HasColumnName("A2ZipCode");
            entity.Property(e => e.CouncilName)
                .HasMaxLength(255)
                .HasColumnName("CouncilName");
            entity.Property(e => e.CouncilNumber).HasColumnName("CouncilNumber");
            entity.Property(e => e.D1Address1)
                .HasMaxLength(255)
                .HasColumnName("D1Address1");
            entity.Property(e => e.D1Address2)
                .HasMaxLength(255)
                .HasColumnName("D1Address2");
            entity.Property(e => e.D1City)
                .HasMaxLength(255)
                .HasColumnName("D1City");
            entity.Property(e => e.D1Email)
                .HasMaxLength(255)
                .HasColumnName("D1Email");
            entity.Property(e => e.D1FirstName)
                .HasMaxLength(255)
                .HasColumnName("D1FirstName");
            entity.Property(e => e.D1LastName)
                .HasMaxLength(255)
                .HasColumnName("D1LastName");
            entity.Property(e => e.D1MemberID).HasColumnName("D1MemberID");
            entity.Property(e => e.D1MiddleName)
                .HasMaxLength(255)
                .HasColumnName("D1MiddleName");
            entity.Property(e => e.D1Phone)
                .HasMaxLength(255)
                .HasColumnName("D1Phone");
            entity.Property(e => e.D1State)
                .HasMaxLength(255)
                .HasColumnName("D1State");
            entity.Property(e => e.D1Suffix)
                .HasMaxLength(255)
                .HasColumnName("D1Suffix");
            entity.Property(e => e.D1ZipCode)
                .HasMaxLength(255)
                .HasColumnName("D1ZipCode");
            entity.Property(e => e.D2Address1)
                .HasMaxLength(255)
                .HasColumnName("D2Address1");
            entity.Property(e => e.D2Address2)
                .HasMaxLength(255)
                .HasColumnName("D2Address2");
            entity.Property(e => e.D2City)
                .HasMaxLength(255)
                .HasColumnName("D2City");
            entity.Property(e => e.D2Email)
                .HasMaxLength(255)
                .HasColumnName("D2Email");
            entity.Property(e => e.D2FirstName)
                .HasMaxLength(255)
                .HasColumnName("D2FirstName");
            entity.Property(e => e.D2LastName)
                .HasMaxLength(255)
                .HasColumnName("D2LastName");
            entity.Property(e => e.D2MemberID).HasColumnName("D2MemberID");
            entity.Property(e => e.D2MiddleName)
                .HasMaxLength(255)
                .HasColumnName("D2MiddleName");
            entity.Property(e => e.D2Phone)
                .HasMaxLength(255)
                .HasColumnName("D2Phone");
            entity.Property(e => e.D2State)
                .HasMaxLength(255)
                .HasColumnName("D2State");
            entity.Property(e => e.D2Suffix)
                .HasMaxLength(255)
                .HasColumnName("D2Suffix");
            entity.Property(e => e.D2ZipCode)
                .HasMaxLength(255)
                .HasColumnName("D2ZipCode");
            entity.Property(e => e.FormSubmitterSEmail)
                .HasMaxLength(255)
                .HasColumnName("FormSubmitterSEmail");
            //entity.Property(e => e.ID)
            //    .ValueGeneratedOnAdd()
            //    .HasColumnName("ID");
            entity.Property(e => e.SubmissionDate)
                .HasColumnType("datetime")
                .HasColumnName("SubmissionDate");
        });
        modelBuilder.Entity<CvnImpDelegatesLog>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblCVN_I__3214EC071756F502");

            entity.ToTable("tblCVN_ImpDelegatesLog");

            entity.Property(e => e.Data).IsUnicode(false);
            entity.Property(e => e.Guid).HasColumnName("GUID");
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Rundate).HasColumnType("datetime");
            entity.Property(e => e.Type)
                .HasMaxLength(10)
                .IsUnicode(false);
        });
        modelBuilder.Entity<CvnDelegateDays>(entity =>
        {
            entity.HasNoKey();
        });
        modelBuilder.Entity<USPSAddress>(entity =>
        {
            entity.HasNoKey();
            entity.Ignore(p => p.AdditionalInfo);
            entity.Ignore(p => p.Address);
            
        });
        modelBuilder.Entity<CvnMileage>(entity =>
        {
            entity.ToTable("tblCVN_MasMileage");

            entity.Property(e => e.Location).HasMaxLength(50);
        });
        modelBuilder.Entity<CvnLocation>(entity =>
        {
            entity.ToTable("tblCVN_MasLocations");

            entity.HasIndex(e => e.Location, "IX_tblCVN_MasLocations").IsUnique();

            entity.Property(e => e.Location).HasMaxLength(50);
        });
        modelBuilder.Entity<CvnMpd>(entity =>
        {
            entity.ToTable("tblCVN_TrxMPD");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.CheckTotal).HasColumnType("numeric(10, 2)");
            entity.Property(e => e.Day1GD1).HasMaxLength(50);
            entity.Property(e => e.Day2GD1).HasMaxLength(50);
            entity.Property(e => e.Day3GD1).HasMaxLength(50);
            entity.Property(e => e.Group).HasMaxLength(50);
            entity.Property(e => e.Location).HasMaxLength(50);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.Office).HasMaxLength(50);
            entity.Property(e => e.Payee).HasMaxLength(50);
        });

        modelBuilder.Entity<MemberSuspension>(entity =>
        {
            entity.HasKey(e => e.Id).HasName("PK__tblSYS_M__3214EC07A5FFC31C");

            entity.ToTable("tblSYS_MasMemberSuspension");

            entity.Property(e => e.KofCid).HasColumnName("KofCId");
        });
        modelBuilder.Entity<LogCorrMemberOffice>(entity =>
        {
            entity.ToTable("tblLOG_CorrMemberOffice");

            entity.Property(e => e.Id).HasColumnName("ID");
            entity.Property(e => e.ChangeDate).HasColumnType("datetime");
            entity.Property(e => e.ChangeType).HasMaxLength(10);
            entity.Property(e => e.MemberId).HasColumnName("MemberID");
            entity.Property(e => e.OfficeId).HasColumnName("OfficeID");
        });

        modelBuilder.Entity<AspNetUserRole>(entity =>
        {
            entity.HasKey(e => new { e.UserId, e.RoleId });

            entity.HasIndex(e => e.RoleId, "IX_AspNetUserRoles_RoleId");
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);

public DbSet<USPSAddress> Uspsaddress { get; set; } = default!;

public DbSet<KofCWSCWebsite.Models.CvnImpDelegateIMP> CvnImpDelegateIMP { get; set; } = default!;

public DbSet<KofCWSCWebsite.Models.NecImpNecrology> NecImpNecrology { get; set; } = default!;

public DbSet<KofCWSCWebsite.Models.AspNetUser> AspNetUsers { get; set; } = default!;

}

