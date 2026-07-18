using HR.Organisation.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Infrastructure.Data;

public class OrganisationContext : BaseDbContext
{
    public OrganisationContext()
    {

    }
    public OrganisationContext(DbContextOptions<OrganisationContext> options, UserResolverService userService) : base(options, userService)
    {

    }
    public DbSet<OrganisationChart> OrganisationCharts { get; set; }
    public DbSet<OrganisationJobSkillYearSetting> OrganisationJobSkillYearSettings { get; set; }
    public DbSet<OrganisationChartImage> OrganisationChartImages { get; set; }
    public DbSet<OrganisationJobCategory> OrganisationJobCategories { get; set; }
    public DbSet<OrganisationJobGroup> OrganisationJobGroups { get; set; }
    public DbSet<OrganisationJobSeries> OrganisationJobSeries { get; set; }
    public DbSet<OrganizationJob> OrganizationJobs { get; set; }
    public DbSet<OrganisationPosition> OrganisationPositions { get; set; }
    public DbSet<OrganisationPositionJob> OrganisationPositionJobs { get; set; }
    public DbSet<OrganisationPositionOccuptionMoreThanOneCach> OrganisationPositionOccuptionMoreThanOneCachs { get; set; }
    public DbSet<OrganisationPositionSuggested> OrganisationPositionSuggesteds { get; set; }
    public DbSet<OrganisationPositionSupervisor> OrganisationPositionSupervisors { get; set; }
    public DbSet<OrganisationProject> OrganisationProjects { get; set; }
    public DbSet<TashkilatTempFile> TashkilatTempFiles { get; set; }
    public DbSet<JobScoringFactor> JobScoringFactors { get; set; }
    public DbSet<JobScoreAbundanceComplexity> JobScoreAbundanceComplexities { get; set; }
    public DbSet<Abundance> Abundances { get; set; }
    public DbSet<Complexity> Complexities { get; set; }
    public DbSet<G20ScoreDomainJobDegree> G20ScoreDomainJobDegries { get; set; }
    public DbSet<JobComplexityJobScoringFactorQuestion> JobComplexityJobScoringFactorQuestions { get; set; }
    public DbSet<JobAbundanceJobScoringFactorQuestion> JobAbundanceJobScoringFactorQuestions { get; set; }
    public DbSet<OrganizationJobEducationGradeQualification> OrganizationJobEducationGradeDescriptions { get; set; }
    public DbSet<OrganizationJobEducationFieldQualification> OrganizationJobEducationFieldDescriptions { get; set; }
    public DbSet<RelatedOrganizationJobDescription> RelatedOrganizationJobDescriptions { get; set; }
    public DbSet<OrganizationJobTaskDescription> OrganizationJobTaskDescriptions { get; set; }
    public DbSet<OrganizationJobPeriodicTaskDescription> OrganizationJobPeriodicTaskDescriptions { get; set; }
    public DbSet<OrganizationJobPerformanceEvaluationCriteriaDescription> OrganizationJobPerformanceEvaluationCriteriaDescriptions { get; set; }
    public DbSet<OrganizationJobRiskAndFaultDescription> OrganizationJobRiskAndFaultDescriptions { get; set; }
    public DbSet<OrganizationJobAbilityQualification> OrganizationJobAbilityQualifications { get; set; }
    public DbSet<OrganizationJobInitialCourseQualification> OrganizationJobInitialCourseQualifications { get; set; }
    public DbSet<OrganizationJobRequiredSoftwaresQualification> OrganizationJobRequiredSoftwaresQualifications { get; set; }
    public DbSet<OrganizationJobForeignLanguageQualification> OrganizationJobForeignLanguageQualifications { get; set; }
    public DbSet<OrganizationJobCompetencyQualification> OrganizationJobCompetencyQualifications { get; set; }
    public DbSet<OrganizationJobRequiredCharacterQualification> OrganizationJobRequiredCharacterQualifications { get; set; }
    public DbSet<OrganizationGoal> OrganizationGoals { get; set; }


    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        ConfigureOrganisationChartDefaults(modelBuilder);

        ConfigureOrganisationPositionRelationships(modelBuilder);

        ConfigureBaseTableValueRelationships(modelBuilder);
        modelBuilder.Entity<TashkilatTempFile>()
 .Property(u => u.Extension)
 .HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");


        modelBuilder.Entity<OrganisationChartImage>()
      .Property(u => u.Extension)
      .HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");




    }

    private static void ConfigureOrganisationChartDefaults(ModelBuilder modelBuilder)
    {
        const string bitFalse = "(CONVERT([bit],(0)))";
        const string intZero = "(0)";
        const string emptyString = "(N'')";

        var entity = modelBuilder.Entity<OrganisationChart>();

        entity.Property(e => e.title).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LetterCode).HasDefaultValueSql(emptyString);
        entity.Property(e => e.Code).HasDefaultValueSql(emptyString);
        entity.Property(e => e.SystemCode).HasDefaultValueSql(emptyString);
        entity.Property(e => e.Rank).HasDefaultValueSql(emptyString);
        entity.Property(e => e.IPAddress).IsRequired().HasDefaultValueSql(emptyString);
        entity.Property(e => e.CreatedBy).HasDefaultValueSql(emptyString);
        entity.Property(e => e.LastModifiedBy).HasDefaultValueSql(emptyString);

        entity.Property(e => e.IsPayLocation).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsRoot).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsApproved).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsDeleted).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsCostCenter).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsRegister).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsIndependentOrg).HasDefaultValueSql(bitFalse);
        entity.Property(e => e.IsOrg).HasDefaultValueSql(bitFalse);

        entity.Property(e => e.Order).HasDefaultValueSql(intZero);

        entity.HasOne(e => e.Place)
            .WithMany()
            .HasForeignKey(e => e.PlaceId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private static void ConfigureOrganisationPositionRelationships(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<OrganisationPosition>();

        // عنوان محترمانه can repeat across positions; do not inherit BaseEntity unique title index.
        var titleIndexes = entity.Metadata.GetIndexes()
            .Where(i => i.Properties.Count == 1 && i.Properties[0].Name == nameof(OrganisationPosition.title))
            .ToList();
        foreach (var index in titleIndexes)
        {
            entity.Metadata.RemoveIndex(index);
        }

        entity.HasOne(e => e.OrganisationChart)
            .WithMany()
            .HasForeignKey(e => e.OrganisationChartId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.RelatedNode)
            .WithMany()
            .HasForeignKey(e => e.RelatedNodeId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.Rank)
            .WithMany()
            .HasForeignKey(e => e.RankId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.PositionManagementLevel)
            .WithMany()
            .HasForeignKey(e => e.PositionManagementLevelId)
            .OnDelete(DeleteBehavior.Restrict);

        entity.HasOne(e => e.PositionState)
            .WithMany()
            .HasForeignKey(e => e.PositionStateId)
            .OnDelete(DeleteBehavior.Restrict);
    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // OrganisationChart
        modelBuilder.Entity<OrganisationChart>()
            .Property<long?>("OrgTypeId");
  

        // OrganizationJob
        modelBuilder.Entity<OrganizationJob>()
            .Property<long?>("StateId");
        modelBuilder.Entity<OrganizationJob>()
            .HasOne(o => o.State)
            .WithMany()
            .HasForeignKey("StateId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJob>()
            .Property<long?>("JobNatureId");
        modelBuilder.Entity<OrganizationJob>()
            .HasOne(o => o.JobNature)
            .WithMany()
            .HasForeignKey("JobNatureId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJob>()
            .Property<long?>("BasQualificationGenderId");
        modelBuilder.Entity<OrganizationJob>()
            .HasOne(o => o.BasQualificationGender)
            .WithMany()
            .HasForeignKey("BasQualificationGenderId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJob>()
            .Property<long?>("CoefficientOfJobTypeId");
        modelBuilder.Entity<OrganizationJob>()
            .HasOne(o => o.CoefficientOfJobType)
            .WithMany()
            .HasForeignKey("CoefficientOfJobTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJob>()
            .Property<long?>("ProcessAreaId");
        modelBuilder.Entity<OrganizationJob>()
            .HasOne(o => o.ProcessArea)
            .WithMany()
            .HasForeignKey("ProcessAreaId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganisationJobGroup
        modelBuilder.Entity<OrganisationJobGroup>()
            .Property<long?>("StateId");
        modelBuilder.Entity<OrganisationJobGroup>()
            .HasOne(g => g.State)
            .WithMany()
            .HasForeignKey("StateId")
            .OnDelete(DeleteBehavior.NoAction);

        // JobScoringFactor
        modelBuilder.Entity<JobScoringFactor>()
            .Property<long>("GroupId");
        modelBuilder.Entity<JobScoringFactor>()
            .HasOne(j => j.Group)
            .WithMany()
            .HasForeignKey("GroupId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobAbilityQualification
        modelBuilder.Entity<OrganizationJobAbilityQualification>()
            .Property<long>("AbilityTypeId");
        modelBuilder.Entity<OrganizationJobAbilityQualification>()
            .HasOne(a => a.AbilityType)
            .WithMany()
            .HasForeignKey("AbilityTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobAbilityQualification>()
            .Property<long>("LevelTypeId");
        modelBuilder.Entity<OrganizationJobAbilityQualification>()
            .HasOne(a => a.LevelType)
            .WithMany()
            .HasForeignKey("LevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobCompetencyQualification
        modelBuilder.Entity<OrganizationJobCompetencyQualification>()
            .Property<long>("CompetencyTypeId");
        modelBuilder.Entity<OrganizationJobCompetencyQualification>()
            .HasOne(c => c.CompetencyType)
            .WithMany()
            .HasForeignKey("CompetencyTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobCompetencyQualification>()
            .Property<long?>("CompetencyLevelId");
        modelBuilder.Entity<OrganizationJobCompetencyQualification>()
            .HasOne(c => c.CompetencyLevel)
            .WithMany()
            .HasForeignKey("CompetencyLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobForeignLanguageQualification
        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .Property<long>("LanguageTypeId");
        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .HasOne(l => l.LanguageType)
            .WithMany()
            .HasForeignKey("LanguageTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .Property<long>("LanguageLevelTypeId");
        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .HasOne(l => l.LanguageLevelType)
            .WithMany()
            .HasForeignKey("LanguageLevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .Property<long>("LanguageSkillTypeId");
        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>()
            .HasOne(l => l.LanguageSkillType)
            .WithMany()
            .HasForeignKey("LanguageSkillTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobInitialCourseQualification
        modelBuilder.Entity<OrganizationJobInitialCourseQualification>()
            .Property<long>("CourseTypeId");
        modelBuilder.Entity<OrganizationJobInitialCourseQualification>()
            .HasOne(i => i.CourseType)
            .WithMany()
            .HasForeignKey("CourseTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobInitialCourseQualification>()
            .Property<long>("CourseLevelId");
        modelBuilder.Entity<OrganizationJobInitialCourseQualification>()
            .HasOne(i => i.CourseLevel)
            .WithMany()
            .HasForeignKey("CourseLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobPerformanceEvaluationCriteriaDescription
        modelBuilder.Entity<OrganizationJobPerformanceEvaluationCriteriaDescription>()
            .Property<long>("CriteriaTypeId");
        modelBuilder.Entity<OrganizationJobPerformanceEvaluationCriteriaDescription>()
            .HasOne(p => p.CriteriaType)
            .WithMany()
            .HasForeignKey("CriteriaTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobPeriodicTaskDescription
        modelBuilder.Entity<OrganizationJobPeriodicTaskDescription>()
            .Property<long?>("TaskPeriodId");
        modelBuilder.Entity<OrganizationJobPeriodicTaskDescription>()
            .HasOne(p => p.TaskPeriod)
            .WithMany()
            .HasForeignKey("TaskPeriodId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobRequiredCharacterQualification
        modelBuilder.Entity<OrganizationJobRequiredCharacterQualification>()
            .Property<long>("CharacterTypeId");
        modelBuilder.Entity<OrganizationJobRequiredCharacterQualification>()
            .HasOne(r => r.CharacterType)
            .WithMany()
            .HasForeignKey("CharacterTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobRequiredCharacterQualification>()
            .Property<long>("RequiredLevelId");
        modelBuilder.Entity<OrganizationJobRequiredCharacterQualification>()
            .HasOne(r => r.RequiredLevel)
            .WithMany()
            .HasForeignKey("RequiredLevelId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobRequiredSoftwaresQualification
        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .Property<long>("SoftwareId");
        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .HasOne(s => s.Software)
            .WithMany()
            .HasForeignKey("SoftwareId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .Property<long>("SoftwareTypeId");
        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .HasOne(s => s.SoftwareType)
            .WithMany()
            .HasForeignKey("SoftwareTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .Property<long>("MasteryLevelTypeId");
        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>()
            .HasOne(s => s.MasteryLevelType)
            .WithMany()
            .HasForeignKey("MasteryLevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobRiskAndFaultDescription
        modelBuilder.Entity<OrganizationJobRiskAndFaultDescription>()
            .Property<long?>("RiskOrFaultTypeId");
        modelBuilder.Entity<OrganizationJobRiskAndFaultDescription>()
            .HasOne(r => r.RiskOrFaultType)
            .WithMany()
            .HasForeignKey("RiskOrFaultTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganizationJobTaskDescription
        modelBuilder.Entity<OrganizationJobTaskDescription>()
            .Property<long?>("TaskTypeId");
        modelBuilder.Entity<OrganizationJobTaskDescription>()
            .HasOne(t => t.TaskType)
            .WithMany()
            .HasForeignKey("TaskTypeId")
            .OnDelete(DeleteBehavior.NoAction);
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
    }
}
