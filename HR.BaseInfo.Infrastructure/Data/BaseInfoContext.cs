
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Service;

namespace HR.BaseInfo.infrastructure.Data;

public class BaseInfoContext : BaseDbContext
{
    public BaseInfoContext()
    {

    }
    public BaseInfoContext(DbContextOptions<BaseInfoContext> options, UserResolverService userService) : base(options, userService)
    {

    }

    public DbSet<BaseTable> BaseTables { get; set; }
    public DbSet<ConfidentialityLevel> ConfidentialityLevels { get; set; }
    public DbSet<AttendanceHoliday> AttendanceHolidays { get; set; }
    public DbSet<LeaveType> LeaveTypes { get; set; }
    public DbSet<TaxExemptionType> TaxExemptionTypes { get; set; }
    public DbSet<SettlementItem> SettlementItems { get; set; }
    public DbSet<SettlementCause> SettlementCauses { get; set; }
    public DbSet<Rank> Ranks { get; set; }
    public DbSet<PositionManagementLevel> PositionManagementLevels { get; set; }
    public DbSet<PositionState> PositionStates { get; set; }
    public DbSet<SettlementStatus> SettlementStatuses { get; set; }
    public DbSet<SettlementDocumentAttachmentType> SettlementDocumentAttachmentTypes { get; set; }
    public DbSet<ReportMapColumn> ReportMapColumns { get; set; }
    public DbSet<Carousel> Carousels { get; set; }
    public DbSet<RequestDocumentRequirement> RequestDocumentRequirements { get; set; }
    public DbSet<RequestDocumentRequirementDetail> RequestDocumentRequirementDetails { get; set; }
    public DbSet<EmployeeRequestStatus> EmployeeRequestStatuses { get; set; }
    public DbSet<ManagementAndStewardshipJob> ManagementAndStewardshipJobs { get; set; }
    public DbSet<AbsenceType> AbsenceTypes { get; set; }
    public DbSet<AbsenceTypeValue> AbsenceTypeValues { get; set; }
    public DbSet<SendedSMS> SendedSMSs { get; set; }
    public DbSet<BaseTableValue> BaseTableValues { get; set; }
    public DbSet<OrderStatus> OrderStatuses { get; set; }
    public DbSet<AgentOfPunishmentEncourage> AgentOfPunishmentEncourages { get; set; }
    public DbSet<OrderType> OrderTypes { get; set; }
    public DbSet<WageItem> WageItems { get; set; }
    public DbSet<EmployeeType> EmployeeTypes { get; set; }
    public DbSet<EducationGrade> EducationGrades { get; set; }
    public DbSet<EmployeeStatus> EmployeeStatuses { get; set; }
    public DbSet<HistoryType> HistoryTypes { get; set; }
    public DbSet<MeasurementUnit> MeasurementUnits { get; set; }
    public DbSet<University> Universities { get; set; }
    public DbSet<Coefficient> Coefficients { get; set; }
    public DbSet<Places> Places { get; set; }
    public DbSet<Core.Entities.File> Files { get; set; }
    public DbSet<EducationField> EducationFields { get; set; }
    public DbSet<EducationOrientation> EducationOrientations { get; set; }
    public DbSet<EducationGroup> EducationGroups { get; set; }
    public DbSet<Job> Jobs { get; set; }
    public DbSet<Position> Positions { get; set; }
    public DbSet<Formula> Formulas { get; set; }
    public DbSet<FormulaUsageLocation> FormulaUsageLocations { get; set; }
    public DbSet<EmployeeTypeGroup> EmployeeTypeGroups { get; set; }
    public DbSet<EmployeeStatusGroup> EmployeeStatusGroups { get; set; }
    public DbSet<OrderTypeGroup> OrderTypeGroups { get; set; }
    public DbSet<Setting> Settings { get; set; }
    public DbSet<JobGroup> JobGroups { get; set; }
    public DbSet<JobCategory> JobCategories { get; set; }
    public DbSet<JobSeries> JobSeries { get; set; }
    public DbSet<PositionType> PositionTypes { get; set; }
    public DbSet<StaffingRule> StaffingRules { get; set; }
    public DbSet<OrganizationType> OrganizationTypes { get; set; }
    public DbSet<Project> Projects { get; set; }
    public DbSet<InsurancePosition> InsurancePositions { get; set; }
    public DbSet<BaseWorkPlace> BaseWorkPlaces { get; set; }
    public DbSet<TaminInsuranceJobList> TaminInsuranceJobLists { get; set; }
    public DbSet<Log> Logs { get; set; }
    public DbSet<TempGlobalFile> TempGlobalFiles { get; set; }
    public DbSet<AgentOfPunishmentEncourageGroup> AgentOfPunishmentEncourageGroups { get; set; }
    public DbSet<ExcelDefinitionType> ExcelDefinitionTypes { get; set; }
    public DbSet<UserIssueReport> UserIssueReports { get; set; }
    public DbSet<UserFileUpload> UserFileUploads { get; set; }
    public DbSet<Feedback> Feedbacks { get; set; }
    public DbSet<FAQ> FAQs { get; set; }
    public DbSet<SystemGuide> SystemGuides { get; set; }
    public DbSet<TaxOccupation> TaxOccupations { get; set; }
    public DbSet<ImportProfile> ImportProfiles { get; set; }
    public DbSet<ImportProfileField> ImportProfileFields { get; set; }
    public DbSet<ImportProfileContextField> ImportProfileContextFields { get; set; }
    public DbSet<ImportBatch> ImportBatches { get; set; }
    public DbSet<ImportTempRow> ImportTempRows { get; set; }
    public DbSet<SkillLevel> SkillLevels { get; set; }
    public DbSet<FundType> FundTypes { get; set; }
    public DbSet<JobActivityType> JobActivityTypes { get; set; }
    public DbSet<JobLevel> JobLevels { get; set; }
    public DbSet<Core.Entities.Version> Versions { get; set; }
    public DbSet<VersionChangeLog> VersionChangeLogs { get; set; }
    public DbSet<ImageAttachment> ImageAttachments { get; set; }
    protected override void ConfigureConventions(ModelConfigurationBuilder configurationBuilder)
    {
        base.ConfigureConventions(configurationBuilder);
        configurationBuilder.DefaultTypeMapping<SharedKernel.Data.KeyValuePair>();
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        modelBuilder.Entity<BaseTable>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<StaffingRule>()
  .HasIndex(u => u.title)
  .IsUnique();



        modelBuilder.Entity<JobSeries>()
   .HasIndex(u => u.title)
   .IsUnique();


        modelBuilder.Entity<PositionType>()
.HasIndex(u => u.title)
.IsUnique();


        modelBuilder.Entity<JobGroup>()
   .HasIndex(u => u.title)
   .IsUnique();

        modelBuilder.Entity<JobCategory>()
                 .HasIndex(u => u.title)
                 .IsUnique();



        modelBuilder.Entity<EducationGroup>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<OrderType>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<WageItem>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<EmployeeType>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<BaseTableValue>()
          .HasIndex(p => new { p.BaseTableId, p.title })
          .IsUnique();


        modelBuilder.Entity<EducationGrade>()
    .HasIndex(u => u.title)
    .IsUnique();

        modelBuilder.Entity<University>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<Coefficient>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<Job>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<Position>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<Formula>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<FormulaUsageLocation>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<EmployeeTypeGroup>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<EmployeeStatusGroup>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<OrderTypeGroup>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<Setting>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<EmployeeStatus>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<HistoryType>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<MeasurementUnit>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<SettlementItem>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<SettlementCause>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<Rank>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<PositionManagementLevel>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<PositionState>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<SettlementStatus>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<SettlementDocumentAttachmentType>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<ReportMapColumn>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<Carousel>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<RequestDocumentRequirement>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<RequestDocumentRequirementDetail>()
          .HasIndex(p => new { p.RequestDocumentRequirementId, p.title })
          .IsUnique();
        modelBuilder.Entity<RequestDocumentRequirementDetail>()
          .HasOne(d => d.RequestDocumentRequirement)
          .WithMany(r => r.Details)
          .HasForeignKey(d => d.RequestDocumentRequirementId)
          .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<EmployeeRequestStatus>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<SkillLevel>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<FundType>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<JobActivityType>()
          .HasIndex(u => u.title)
          .IsUnique();
        modelBuilder.Entity<JobLevel>()
          .HasIndex(u => u.title)
          .IsUnique();

        modelBuilder.Entity<SendedSMS>()
            .HasIndex(s => new { s.MobileNumber, s.CreateDate })
            .HasFilter("[MobileNumber] IS NOT NULL");

        modelBuilder.Entity<SendedSMS>()
            .HasIndex(s => new { s.IPAddress, s.CreateDate })
            .HasFilter("[IPAddress] IS NOT NULL AND [IPAddress] <> ''");

        // Indexes for Logs
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.CreatedOn);
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.Level);
        modelBuilder.Entity<Log>()
            .HasIndex(l => l.Logger);

        // Configure BaseTableValue relationships without FK constraints
        modelBuilder.Entity<Places>()
            .Property<long?>("PlaceTypeId");

        modelBuilder.Entity<Places>()
            .HasOne(p => p.PlaceType)
            .WithMany()
            .HasForeignKey("PlaceTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<AttendanceHoliday>()
            .HasOne(h => h.Place)
            .WithMany()
            .HasForeignKey(h => h.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);

    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        base.OnConfiguring(optionsBuilder);
    }
}
