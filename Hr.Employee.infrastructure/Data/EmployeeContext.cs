using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using HR.Employee.Core.Entities;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Service;

namespace Hr.Employee.infrastructure.Data;

public partial class EmployeeContext : BaseDbContext
{
    public EmployeeContext()
    {

    }
    public EmployeeContext(DbContextOptions<EmployeeContext> options, UserResolverService userService) : base(options, userService)
    {

    }
    public DbSet<HR.Employee.Core.Entities.Employee> Employees { get; set; }
    public DbSet<HR.Employee.Core.Entities.Appearance> Appearances { get; set; }
    public DbSet<HR.Employee.Core.Entities.Family> Families { get; set; }
    public DbSet<HR.Employee.Core.Entities.ContactInfo> ContactInfos { get; set; }
    public DbSet<HR.Employee.Core.Entities.BankAccount> BankAccounts { get; set; }
    public DbSet<HR.Employee.Core.Entities.Basij> Basijs { get; set; }
    public DbSet<HR.Employee.Core.Entities.BasijGrade> BasijGrades { get; set; }
    public DbSet<HR.Employee.Core.Entities.Captivity> Captivities { get; set; }
    public DbSet<HR.Employee.Core.Entities.Coefficient> Coefficients { get; set; }
    public DbSet<HR.Employee.Core.Entities.Competency> Competencies { get; set; }
    public DbSet<HR.Employee.Core.Entities.Course> Courses { get; set; }
    public DbSet<HR.Employee.Core.Entities.Disability> Disabilities { get; set; }
    public DbSet<HR.Employee.Core.Entities.DrivingLicense> DrivingLicenses { get; set; }
    public DbSet<HR.Employee.Core.Entities.Education> Educations { get; set; }
    public DbSet<HR.Employee.Core.Entities.EvaluationResult> EvaluationResults { get; set; }
    public DbSet<HR.Employee.Core.Entities.ForeignTravel> ForeignTravels { get; set; }
    public DbSet<HR.Employee.Core.Entities.ForeignLanguage> ForeignLanguages { get; set; }
    public DbSet<HR.Employee.Core.Entities.Image> Images { get; set; }
    public DbSet<HR.Employee.Core.Entities.Insurance> Insurances { get; set; }
    public DbSet<HR.Employee.Core.Entities.InsuranceDetail> InsuranceDetails { get; set; }
    public DbSet<HR.Employee.Core.Entities.Isar> Isars { get; set; }
    public DbSet<HR.Employee.Core.Entities.MilitaryService> MilitaryServices { get; set; }
    public DbSet<HR.Employee.Core.Entities.OtherVeteran> OtherVeterans { get; set; }
    public DbSet<HR.Employee.Core.Entities.War> Wars { get; set; }
    public DbSet<HR.Employee.Core.Entities.HistoryStop> HistoryStops { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeFile> EmployeeFiles { get; set; }
    public DbSet<HR.Employee.Core.Entities.File> Files { get; set; }
    public DbSet<HR.Employee.Core.Entities.Work> Works { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeSoftware> EmployeeSoftwares { get; set; }
    public DbSet<HR.Employee.Core.Entities.Ability> Abilities { get; set; }
    public DbSet<HR.Employee.Core.Entities.Character> Characters { get; set; }
    public DbSet<HR.Employee.Core.Entities.PunishmentEncourage> PunishmentEncourages { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeLoginHistory> EmployeeLoginHistories { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeOtp> EmployeeOtps { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeRefreshToken> EmployeeRefreshTokens { get; set; }
    public DbSet<HR.Employee.Core.Entities.GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; }
    public DbSet<HR.Employee.Core.Entities.GroupPunishmentEncourageFile> GroupPunishmentEncourageFiles { get; set; }
    public DbSet<HR.Employee.Core.Entities.TempPunishmentEncourage> TempPunishmentEncourages { get; set; }
    public DbSet<HR.Employee.Core.Entities.AbsenceRecord> AbsenceRecords { get; set; }
    public DbSet<HR.Employee.Core.Entities.Attendance> Attendances { get; set; }
    public DbSet<HR.Employee.Core.Entities.Experience> Experiences { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeRequest> EmployeeRequests { get; set; }
    public DbSet<HR.Employee.Core.Entities.EmployeeRequestDetail> EmployeeRequestDetails { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;


        modelBuilder.Entity<HR.Employee.Core.Entities.Family>()
     .HasIndex(p => new { p.EmployeeId, p.NationalNo, p.IsDeleted })
     .IsUnique();

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasIndex(e => new { e.PhoneNumber, e.IsDeleted })
            .HasFilter("[PhoneNumber] IS NOT NULL");

        ConfigureEmployeeDefaults(modelBuilder);
        ConfigureEmployeeEntityDefaults(modelBuilder);

        modelBuilder.Entity<EmployeeOtp>()
            .HasIndex(o => new { o.EmployeeId, o.Purpose, o.IsUsed, o.SentAt });

        modelBuilder.Entity<EmployeeRefreshToken>()
            .HasIndex(rt => new { rt.EmployeeId, rt.RevokedAt, rt.ExpiresAt });



        modelBuilder.Entity<HR.Employee.Core.Entities.File>()
.Property(u => u.Extension)
.HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");


        //  modelBuilder.Entity<EmployeeFile>()
        //.Property(u => u.Extension)
        //.HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");


        //         modelBuilder.Entity<HR.Employee.Core.Entities.Education>()
        //      .HasIndex(p => new { p.EmployeeId, p.IsDefaultEducation, p.IsDeleted  })
        //      .IsUnique();



        //         modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
        //     .HasIndex(p => new { p.EmployeeId, p.IsLast, p.IsDeleted })
        //     .IsUnique();



        //         modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
        // .HasIndex(p => new { p.EmployeeId, p.IsLast, p.IsDeleted })
        // .IsUnique();


        //         modelBuilder.Entity<HR.Employee.Core.Entities.BankAccount>()
        //.HasIndex(p => new { p.EmployeeId, p.Status, p.IsDeleted })
        //.IsUnique();

        // Configure BaseTableValue relationships without FK constraints
        ConfigureBaseTableValueRelationships(modelBuilder);

    }

    private static void ConfigureEmployeeDefaults(ModelBuilder modelBuilder)
    {
        const string bitFalse = "(CONVERT([bit],(0)))";
        const string intZero = "(0)";
        const string emptyString = "(N'')";

        var entity = modelBuilder.Entity<HR.Employee.Core.Entities.Employee>();

        entity.Property(e => e.FirstName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LastName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.EnglishFirstName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.EnglishLastName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.FatherName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PersonelCode).HasDefaultValueSql(emptyString);
        entity.Property(e => e.AccountingSystemEmployeeId).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IdentityNo).HasDefaultValueSql(emptyString);
        entity.Property(e => e.NationalNo).HasDefaultValueSql(emptyString);
        entity.Property(e => e.ActiveName).HasDefaultValueSql(emptyString);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IssueSerialString).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IssueSerialOrder).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PassportNo).HasDefaultValueSql(emptyString);
        entity.Property(e => e.InOutCard).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LostIssueSerialString).HasDefaultValueSql(emptyString);
        entity.Property(e => e.ReleaseReason).HasDefaultValueSql(emptyString);
        entity.Property(e => e.DeathCause).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PasswordHash).HasDefaultValueSql(emptyString);
        entity.Property(e => e.SecurityStamp).HasDefaultValueSql(emptyString);
        entity.Property(e => e.ConcurrencyStamp).HasDefaultValueSql(emptyString);
        entity.Property(e => e.PhoneNumber).HasDefaultValueSql(emptyString);
        entity.Property(e => e.MartyrChildTrackingCode).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IPAddress).HasDefaultValueSql(emptyString);
        entity.Property(e => e.CreatedBy).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LastModifiedBy).HasDefaultValueSql(emptyString);
        entity.Ignore(e => e.title);

        entity.Property(e => e.IsActive).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsVerify).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsRetired).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsWomenHead).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.EmailConfirmed).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.Disabled).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.TwoFactorEnabled).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsAdmin).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.PhoneNumberConfirmed).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsDeleted).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsHekmat).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsCashBenefits).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsWelfareBenefits).HasDefaultValueSql(bitFalse);

        entity.Property(e => e.SubsystemId).HasDefaultValueSql(intZero);
        entity.Property(e => e.AccessFailedCount).HasDefaultValueSql(intZero);
        entity.Property(e => e.PrivateJobStatus).HasDefaultValueSql(intZero);
        entity.Property(e => e.IssueSerialChar).HasDefaultValueSql(intZero);
        entity.Property(e => e.SectId).HasDefaultValueSql(intZero);
        entity.Property(e => e.Imperfective).HasDefaultValueSql(intZero);
    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // Employee entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.Gender)
            .WithMany()
            .HasForeignKey("GenderId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.Religeon)
            .WithMany()
            .HasForeignKey("ReligeonId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.Mazhab)
            .WithMany()
            .HasForeignKey("MazhabId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.Nationality)
            .WithMany()
            .HasForeignKey("NationalityId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.Citizenship)
            .WithMany()
            .HasForeignKey("CitizenshipId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.BloodGroup)
            .WithMany()
            .HasForeignKey("BloodGroupId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.MaritalStatus)
            .WithMany()
            .HasForeignKey("MaritalStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.HeadquartersOrRowType)
            .WithMany()
            .HasForeignKey("HeadquartersOrRowTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Employee>()
            .HasOne(e => e.MartyrRelation)
            .WithMany()
            .HasForeignKey("MartyrRelationId")
            .OnDelete(DeleteBehavior.NoAction);


      

  



        
        // BankAccount entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.BankAccount>()
            .Property<long?>("AccountTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.BankAccount>()
            .HasOne(b => b.AccountType)
            .WithMany()
            .HasForeignKey("AccountTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.BankAccount>()
            .Property<long?>("PayrollStatusId");
   

        // Image entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Image>()
            .Property<long?>("ImageTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Image>()
            .HasOne(i => i.ImageType)
            .WithMany()
            .HasForeignKey("ImageTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // ForignLanguage entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.ForeignLanguage>()
            .Property<long?>("LevelId");


        modelBuilder.Entity<HR.Employee.Core.Entities.ForeignLanguage>()
            .Property<long?>("LanguageskillId");


        modelBuilder.Entity<HR.Employee.Core.Entities.ForeignLanguage>()
            .Property<long?>("LanguageId");
 

        // Course entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .Property<long?>("CourseStatusId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .HasOne(c => c.CourseStatus)
            .WithMany()
            .HasForeignKey("CourseStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .Property<long?>("CourseRegTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .HasOne(c => c.CourseRegType)
            .WithMany()
            .HasForeignKey("CourseRegTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .Property<long?>("CourseLicenseId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .HasOne(c => c.CourseLicense)
            .WithMany()
            .HasForeignKey("CourseLicenseId")
            .OnDelete(DeleteBehavior.NoAction);

        // Competency entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Competency>()
            .Property<long?>("CompetencyTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Competency>()
            .HasOne(c => c.CompetencyType)
            .WithMany()
            .HasForeignKey("CompetencyTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Competency>()
            .Property<long?>("CompetencyLevelId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Competency>()
            .HasOne(c => c.CompetencyLevel)
            .WithMany()
            .HasForeignKey("CompetencyLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // Ability entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Ability>()
            .Property<long>("AbilityTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Ability>()
            .HasOne(a => a.AbilityType)
            .WithMany()
            .HasForeignKey("AbilityTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Ability>()
            .Property<long>("LevelTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Ability>()
            .HasOne(a => a.LevelType)
            .WithMany()
            .HasForeignKey("LevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // Appearance entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .Property<long?>("EyeColorId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .HasOne(a => a.EyeColor)
            .WithMany()
            .HasForeignKey("EyeColorId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .Property<long?>("SkinColorId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .HasOne(a => a.SkinColor)
            .WithMany()
            .HasForeignKey("SkinColorId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .Property<long?>("HairColorId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Appearance>()
            .HasOne(a => a.HairColor)
            .WithMany()
            .HasForeignKey("HairColorId")
            .OnDelete(DeleteBehavior.NoAction);

        // Basij entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .Property<long?>("BasijPlaceId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .HasOne(b => b.BasijPlace)
            .WithMany()
            .HasForeignKey("BasijPlaceId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .Property<long?>("BasijTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .HasOne(b => b.BasijType)
            .WithMany()
            .HasForeignKey("BasijTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .Property<long?>("ConfirmerOrganID");
        modelBuilder.Entity<HR.Employee.Core.Entities.Basij>()
            .HasOne(b => b.ConfirmerOrgan)
            .WithMany()
            .HasForeignKey("ConfirmerOrganID")
            .OnDelete(DeleteBehavior.NoAction);

        // Captivity entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Captivity>()
            .Property<long?>("CaptivityLocationId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Captivity>()
            .HasOne(c => c.CaptivityLocation)
            .WithMany()
            .HasForeignKey("CaptivityLocationId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Captivity>()
            .Property<long?>("ConfirmerOrganID");
        modelBuilder.Entity<HR.Employee.Core.Entities.Captivity>()
            .HasOne(c => c.ConfirmerOrgan)
            .WithMany()
            .HasForeignKey("ConfirmerOrganID")
            .OnDelete(DeleteBehavior.NoAction);

        // Character entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Character>()
            .Property<long>("CharacterTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Character>()
            .HasOne(c => c.CharacterType)
            .WithMany()
            .HasForeignKey("CharacterTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Character>()
            .Property<long>("RequiredLevelId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Character>()
            .HasOne(c => c.RequiredLevel)
            .WithMany()
            .HasForeignKey("RequiredLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // Coefficient entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Coefficient>()
            .Property<long?>("CoefficientTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Coefficient>()
            .HasOne(c => c.CoefficientType)
            .WithMany()
            .HasForeignKey("CoefficientTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // ContactInfo entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
            .Property<long?>("AddressTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
            .HasOne(c => c.AddressType)
            .WithMany()
            .HasForeignKey("AddressTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
            .Property<long?>("LocationTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.ContactInfo>()
            .HasOne(c => c.LocationType)
            .WithMany()
            .HasForeignKey("LocationTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // Course entity additional relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .Property<long?>("CourseTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .HasOne(c => c.CourseType)
            .WithMany()
            .HasForeignKey("CourseTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .Property<long?>("CourseId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Course>()
            .HasOne(c => c.CourseTitle)
            .WithMany()
            .HasForeignKey("CourseId")
            .OnDelete(DeleteBehavior.NoAction);

        // Education entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Education>()
            .Property<long?>("EducationStateID");
        modelBuilder.Entity<HR.Employee.Core.Entities.Education>()
            .HasOne(e => e.EducationState)
            .WithMany()
            .HasForeignKey("EducationStateID")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Education>()
            .Property<long?>("UniversityLevelId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Education>()
            .HasOne(e => e.UniversityLevel)
            .WithMany()
            .HasForeignKey("UniversityLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // EmployeeFile entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeFile>()
            .Property<long>("FileGroupId");
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeFile>()
            .HasOne(e => e.FileGroup)
            .WithMany()
            .HasForeignKey("FileGroupId")
            .OnDelete(DeleteBehavior.NoAction);

        // EmployeeSoftware entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .Property<long>("SoftwareId");
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .HasOne(e => e.Software)
            .WithMany()
            .HasForeignKey("SoftwareId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .Property<long>("SoftwareTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .HasOne(e => e.SoftwareType)
            .WithMany()
            .HasForeignKey("SoftwareTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .Property<long>("MasteryLevelTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeSoftware>()
            .HasOne(e => e.MasteryLevelType)
            .WithMany()
            .HasForeignKey("MasteryLevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // Insurance entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Insurance>()
            .Property<long?>("InsWorkShopTypeId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Insurance>()
            .HasOne(i => i.InsWorkShopType)
            .WithMany()
            .HasForeignKey("InsWorkShopTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.Insurance>()
            .Property<long?>("InsuranceSubmissionCityId");
        modelBuilder.Entity<HR.Employee.Core.Entities.Insurance>()
            .HasOne(i => i.InsuranceSubmissionCity)
            .WithMany()
            .HasForeignKey("InsuranceSubmissionCityId")
            .OnDelete(DeleteBehavior.NoAction);

        // Experience entity relationships
        modelBuilder.Entity<HR.Employee.Core.Entities.Experience>()
            .HasOne(e => e.HistoryType)
            .WithMany()
            .HasForeignKey(e => e.HistoryTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeRequest>()
            .HasOne(e => e.RequestDocumentRequirement)
            .WithMany()
            .HasForeignKey(e => e.RequestDocumentRequirementId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeRequest>()
            .HasOne(e => e.EmployeeRequestStatus)
            .WithMany()
            .HasForeignKey(e => e.EmployeeRequestStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeRequestDetail>()
            .HasOne(e => e.RequestDocumentRequirementDetail)
            .WithMany()
            .HasForeignKey(e => e.RequestDocumentRequirementDetailId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<HR.Employee.Core.Entities.EmployeeRequestDetail>()
            .HasOne(e => e.File)
            .WithMany()
            .HasForeignKey(e => e.FileId)
            .OnDelete(DeleteBehavior.NoAction);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

        
        base.OnConfiguring(optionsBuilder);
    }
}
