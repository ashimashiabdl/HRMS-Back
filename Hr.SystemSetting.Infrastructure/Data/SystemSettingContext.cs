using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Core.Entities.OrganisationEmployeeTypes;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Configuration;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Infrastructure.Data;

public class SystemSettingContext : BaseDbContext
{
    public SystemSettingContext()
    {

    }
    public SystemSettingContext(DbContextOptions<SystemSettingContext> options , UserResolverService userService) : base(options, userService)
    {

    }
    
    public DbSet<OrganisationCoefficient> OrganisationCoefficients { get; set; }
    public DbSet<OrganisationSettlementCause> OrganisationSettlementCauses { get; set; }
    public DbSet<OrganisationSettlementItem> OrganisationSettlementItems { get; set; }
    public DbSet<OrganisationEmployeeStatus> OrganisationEmployeeStatuses { get; set; }
    public DbSet<OrganisationEmployeeType> OrganisationEmployeeTypes { get; set; }
    public DbSet<OrganisationOrderType> OrganisationOrderTypes { get; set; }
    public DbSet<OrganisationWageItem> OrganisationWageItems { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderType> OrganisationEmployeeTypeOrderTypes { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChanges { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeDescription> OrganisationEmployeeTypeOrderTypeDescriptions { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; }
    public DbSet<OrganisationEmployeeTypeWageItem> OrganisationEmployeeTypeWageItems { get; set; }
    public DbSet<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; }
    public DbSet<OrganisationCostCenter> OrganisationCostCenters { get; set; }
    public DbSet<OrganisationFormula> OrganisationFormulas { get; set; }
    public DbSet<OrganisationSetting> OrganisationSettings { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeCheck { get; set; }
    public DbSet<OrganisationEmployeeTypeOrderTypeSummaryCalc> OrganisationEmployeeTypeOrderTypeSummaryCalcs { get; set; }
    public DbSet<OrganisationEmployeeTypeCoefficient> OrganisationEmployeeTypeCoefficients { get; set; }
    public DbSet<OrganisationOrderTypeHistoryExclusion> OrganisationOrderTypeHistoryExclusions { get; set; }
    public DbSet<OrganisationEmployeeTypeMRT> OrganisationEmployeeTypeOrderTypeMRTs { get; set; }
    public DbSet<OrganisationMRT> OrganisationMRTs { get; set; }
    public DbSet<OrganisationWorkPlace> OrganisationWorkPlaces { get; set; }
    public DbSet<OrganisationInsJobList> OrganisationInsJobLists { get; set; }
    public DbSet<OrganisationTempFile> OrganisationTempFiles { get; set; }
    public DbSet<OrganisationPeymanRow> PeymanRows { get; set; }
    public DbSet<OrganisationAgentOfPunishmentEncourage> OrganisationAgentOfPunishmentEncourages { get; set; }
    public DbSet<OrganisationAgentOfPunishmentEncourageScoreInterval> OrganisationAgentOfPunishmentEncourageScoreIntervals { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;


        modelBuilder.Entity<OrganisationTempFile>()
       .Property(u => u.Extension)
       .HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");


        modelBuilder.Entity<OrganisationMRT>()
      .Property(u => u.Extension)
      .HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))");

        // Configure BaseTableValue relationships without FK constraints
        ConfigureBaseTableValueRelationships(modelBuilder);

    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // OrganisationEmployeeTypeOrderType entity relationships
        modelBuilder.Entity<OrganisationEmployeeTypeOrderType>()
            .Property<long>("OrderLevelTypeId");
        modelBuilder.Entity<OrganisationEmployeeTypeOrderType>()
            .HasOne(o => o.OrderLevelType)
            .WithMany()
            .HasForeignKey("OrderLevelTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganisationEmployeeType entity relationships


        // OrganisationEmployeeTypeMRT entity relationships
        modelBuilder.Entity<OrganisationEmployeeTypeMRT>()
            .Property<long?>("SettingTypeId");
        modelBuilder.Entity<OrganisationEmployeeTypeMRT>()
            .HasOne(m => m.SettingType)
            .WithMany()
            .HasForeignKey("SettingTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganisationEmployeeTypeOrderTypeCheck entity relationships
   

        // OrganisationEmployeeTypeOrderTypeCoefficient entity relationships
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCoefficient>()
            .Property<long?>("CheckingTimeId");
        //modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCoefficient>()
        //    .HasOne(c => c.CheckingTime)
        //    .WithMany()
        //    .HasForeignKey("CheckingTimeId")
        //    .OnDelete(DeleteBehavior.NoAction);

        //modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCoefficient>()
        //    .Property<long?>("EnterTypeId");
        //modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCoefficient>()
        //    .HasOne(c => c.EnterType)
        //    .WithMany()
        //    .HasForeignKey("EnterTypeId")
        //    .OnDelete(DeleteBehavior.NoAction);

        // OrganisationEmployeeTypeOrderTypeSummaryCalc entity relationships
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .Property<long?>("CalExperienceRecordedEntertypeId");
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .HasOne(s => s.CalExperienceRecordedEntertype)
            .WithMany()
            .HasForeignKey("CalExperienceRecordedEntertypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .Property<long?>("CalRetiredRecordedEntertypeId");
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .HasOne(s => s.CalRetiredRecordedEntertype)
            .WithMany()
            .HasForeignKey("CalRetiredRecordedEntertypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .Property<long?>("CalYearRecordedEntertypeId");
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>()
            .HasOne(s => s.CalYearRecordedEntertype)
            .WithMany()
            .HasForeignKey("CalYearRecordedEntertypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganisationEmployeeTypeOrderTypeWageItem entity relationships
        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeWageItem>()
            .Property<long?>("CheckingTimeId");




        // OrganisationOrderType entity relationships
        modelBuilder.Entity<OrganisationOrderType>()
            .Property<long?>("OrderDirectionTypeId");
        modelBuilder.Entity<OrganisationOrderType>()
            .HasOne(o => o.OrderDirectionType)
            .WithMany()
            .HasForeignKey("OrderDirectionTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // OrganisationEmployeeTypeSettlementItem entity relationships (PaymentType / EnterType -> BaseTableValue)
        modelBuilder.Entity<OrganisationEmployeeTypeSettlementItem>()
            .HasOne(s => s.PaymentType)
            .WithMany()
            .HasForeignKey(s => s.PaymentTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationEmployeeTypeSettlementItem>()
            .HasOne(s => s.EnterType)
            .WithMany()
            .HasForeignKey(s => s.EnterTypeId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationEmployeeTypeSettlementItem>()
            .HasOne(s => s.MeasurementUnit)
            .WithMany()
            .HasForeignKey(s => s.MeasurementUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationFormula>()
            .HasOne(f => f.FormulaUsageLocation)
            .WithMany()
            .HasForeignKey(f => f.FormulaUsageLocationId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<OrganisationWageItem>()
            .Property<long?>("MappedExcelColumnId");
        modelBuilder.Entity<OrganisationWageItem>()
            .HasOne(o => o.MappedExcelColumn)
            .WithMany()
            .HasForeignKey("MappedExcelColumnId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationCoefficient>()
            .Property<long?>("MappedExcelColumnId");
        modelBuilder.Entity<OrganisationCoefficient>()
            .HasOne(o => o.MappedExcelColumn)
            .WithMany()
            .HasForeignKey("MappedExcelColumnId")
            .OnDelete(DeleteBehavior.NoAction);

        // Add more BaseTableValue relationships for SystemSetting entities...
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        
        base.OnConfiguring(optionsBuilder);
    }
}
