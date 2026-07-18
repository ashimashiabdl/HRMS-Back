using HR.Order.Core.Data;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.Data.EmployeeRelated;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Diagnostics;
using Microsoft.EntityFrameworkCore.Metadata.Builders;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Payroll.Infrastructure.Data;

public class PayrollContext : BaseDbContext
{
    public PayrollContext()
    {

    }
    public PayrollContext(DbContextOptions<PayrollContext> options, UserResolverService userService) : base(options, userService)
    {

    }

    #region تسویه حساب 
    public DbSet<EmployeeSettlement> EmployeeSettlements { get; set; }
    public DbSet<EmployeeSettlementItem> EmployeeSettlementItems { get; set; }
    public DbSet<EmployeeSettlementAttachment> EmployeeSettlementAttachments { get; set; }
    public DbSet<EmployeeSettlementArchive> EmployeeSettlementArchives { get; set; }
    #endregion تسویه حساب

    public DbSet<PersonnelFunction> PersonnelFunctions { get; set; }

    public DbSet<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; }
    public DbSet<OrganisationFundType> OrganisationFundTypes { get; set; }
    public DbSet<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitions { get; set; }
    public DbSet<EmployeeFund> EmployeeFunds { get; set; }

    public DbSet<OrganisationLeave> OrganisationLeaves { get; set; }
    public DbSet<OrganisationEmployeeTypeLeave> OrganisationEmployeeTypeLeaves { get; set; }
    public DbSet<FicheLeaveItem> FicheLeaveItems { get; set; }
    public DbSet<PersonnelLeave> PersonnelLeaves { get; set; }
    public DbSet<PersonnelFunctionVisible> PersonnelFunctionVisibles { get; set; }
    public DbSet<Fiche> Fiches { get; set; }
    public DbSet<Tax> Taxes { get; set; }
    public DbSet<TaxTable> TaxTables { get; set; }
    public DbSet<PaymentPeriod> PaymentPeriods { get; set; }
    public DbSet<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItems { get; set; }
    public DbSet<PersonnelFicheItem> PersonnelFicheItems { get; set; }
    public DbSet<FicheItem> FicheItems { get; set; }
    public DbSet<CostCenterFicheItem> CostCenterFicheItems { get; set; }
    public DbSet<OrganisationFicheItem> OrganisationFicheItems { get; set; }
    public DbSet<TaxDiskette> TaxDiskettes { get; set; }
    public DbSet<InsuranceDiskette> InsuranceDiskettes { get; set; }
    public DbSet<BankDiskette> BankDiskettes { get; set; }
    public DbSet<TaxCoefficientItem> TaxCoefficientItems { get; set; }
    public DbSet<InsuranceType> InsuranceTypes { get; set; }
    public DbSet<InsuranceBranch> InsuranceBranchs { get; set; }
    public DbSet<InsuranceDisketteItem> InsuranceDisketteItems { get; set; }
    public DbSet<Bank> Banks { get; set; }
    public DbSet<BankBranch> BankBranchs { get; set; }
    public DbSet<BankDisketteItem> BankDisketteItems { get; set; }
    public DbSet<BlackList> BlackLists { get; set; }
    public DbSet<LoanType> LoanTypes { get; set; }
    public DbSet<PersonnelLoan> PersonnelLoans { get; set; }
    public DbSet<PersonnelLoanPayment> PersonnelLoanPayments { get; set; }
    public DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }
    public DbSet<PaymentType> PaymentTypes { get; set; }
    public DbSet<DeductionType> DeductionTypes { get; set; }
    public DbSet<PersonnelPayment> PersonnelPayments { get; set; }
    public DbSet<OrganProperty> OrganProperties { get; set; }

    public DbSet<TaxDisketteWH> TaxDisketteWHs { get; set; }
    public DbSet<TaxDisketteWP> TaxDisketteWPs { get; set; }
    public DbSet<TaxDisketteWK> TaxDisketteWKs { get; set; }
    public DbSet<BlockedAccount> BlockedAccounts { get; set; }

    public DbSet<PersonnelManagerList> PersonnelManagerLists { get; set; }
    public DbSet<Bill> Bills { get; set; }
    public DbSet<BillInstance> BillInstances { get; set; }
    public DbSet<BillDetail> BillDetails { get; set; }
    public DbSet<BillDetailException> BillDetailExceptions { get; set; }
    public DbSet<BillItem> BillItems { get; set; }
    public DbSet<FichePdfArchive> FichePdfArchives { get; set; }
    public DbSet<FicheReportArchive> FicheReportArchives { get; set; }
    public DbSet<TaxableIncome> TaxableIncomes { get; set; }
    public DbSet<StatusList> StatusLists { get; set; }
    public DbSet<StatusListItem> StatusListItems { get; set; }
    public DbSet<CalclulationSetting> CalclulationSettings { get; set; }
    public DbSet<MinimumMonthlyWage> MinimumMonthlyWages { get; set; }

    public DbSet<FicheStatus> FicheStatuses { get; set; }
    public DbSet<BatchPayRollRequest> BatchPayRollRequests { get; set; }
    public DbSet<BatchPayRollRequestDetail> BatchPayRollRequestDetails { get; set; }
    public DbSet<BatchSettlementRequest> BatchSettlementRequests { get; set; }
    public DbSet<BatchSettlementRequestDetail> BatchSettlementRequestDetails { get; set; }
    public DbSet<BankDisketteTemplate> BankDisketteTemplates { get; set; }
    public DbSet<BankDisketteTemplateRow> BankDisketteTemplateRows { get; set; }
    public DbSet<InsuranceDisketteFile> InsuranceDisketteFiles { get; set; }
    public DbSet<TaxNonCashPayment> TaxNonCashPayments { get; set; }
    public DbSet<TaxDisketteFile> TaxDisketteFiles { get; set; }
    public DbSet<InsuranceDisketteCostCenter> InsuranceDisketteCostCenters { get; set; }
    public DbSet<BankDisketteCostCenter> BankDisketteCostCenters { get; set; }
    public DbSet<BankDisketteGroupAndFile> BankDisketteGroupAndFiles { get; set; }
    public DbSet<TaxDisketteCostCenter> TaxDisketteCostCenters { get; set; }
    public DbSet<Arear> Arears { get; set; }
    public DbSet<ArearsChangedFicheItem> ArearsChangedFicheItems { get; set; }
    public DbSet<ArearFicheItem> ArearFicheItems { get; set; }
    public DbSet<ArearsStatus> ArearsStatuses { get; set; }
    public DbSet<DeductedArears> DeductedArears { get; set; }
    public DbSet<DeductedArearsDetail> DeductedArearsDetails { get; set; }
    public DbSet<InterdictOrder> InterdictOrders { get; set; }
    public DbSet<InterdictOrderCoefficientItem> InterdictOrderCoefficientItems { get; set; }
    public DbSet<RecruitOrder> RecruitOrders { get; set; }
    public DbSet<ArearFiche> ArearFiches { get; set; }
    public DbSet<BatchLog> BatchLogs { get; set; }
    public DbSet<FunctionExcelDefinition> FunctionExcelDefinitions { get; set; }
    public DbSet<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; }
    public DbSet<TempPersonnelFunction> TempPersonnelFunctions { get; set; }
    public DbSet<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; }
    public DbSet<PaymentPeriodEmployeeBonus> PaymentPeriodEmployeeBonus { get; set; }
    public DbSet<EmployeeDeductionPayment> EmployeeDeductionPayments { get; set; }
    public DbSet<TempPersonnelLeave> TempPersonnelLeaves { get; set; }
    public DbSet<EmployeeDeductionUploadBatch> EmployeeDeductionUploadBatches { get; set; }
    public DbSet<TempEmployeeDeduction> TempEmployeeDeductions { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {

        var cascadeFKs = modelBuilder.Model.GetEntityTypes()
              .SelectMany(t => t.GetForeignKeys())
              .Where(fk => !fk.IsOwnership && fk.DeleteBehavior == DeleteBehavior.Cascade);

        foreach (var fk in cascadeFKs)
            fk.DeleteBehavior = DeleteBehavior.Restrict;

        ConfigureEmployeeSettlementArchive(modelBuilder);

        modelBuilder
       .Entity<CalclulationSetting>()
       .HasIndex(u => u.OrganisationChartId)
       .IsUnique();

        modelBuilder.Entity<Fiche>()
       .HasIndex(p => new { p.EmployeeId, p.PaymentPeriodId, p.IsDeleted })
       .IsUnique();

        // ArearFiche / ArearFicheItem از Fiche / FicheItem ارث می‌برند ولی جداول جدا دارند (نه TPH/TPT)
        modelBuilder.Entity<ArearFiche>().HasBaseType((Type?)null);
        modelBuilder.Entity<ArearFicheItem>().HasBaseType((Type?)null);
        modelBuilder.Entity<ArearFicheItem>().Ignore(e => e.FicheId);
        modelBuilder.Entity<ArearFicheItem>().Ignore(e => e.Fiche);
        modelBuilder.Entity<ArearFiche>()
            .HasMany(a => a.ArearFicheItems)
            .WithOne(i => i.ArearFiche)
            .HasForeignKey(i => i.ArearFicheId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<ArearFiche>()
            .HasMany(a => a.ArearsChangedFicheItems)
            .WithOne(i => i.ArearFiche)
            .HasForeignKey(i => i.ArearFicheId)
            .OnDelete(DeleteBehavior.Restrict);
        modelBuilder.Entity<Arear>()
            .HasMany(a => a.ArearFiches)
            .WithOne(f => f.Arear)
            .HasForeignKey(f => f.ArearId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<TaxDiskette>()
       .HasIndex(p => new { p.PaymentPeriodId, p.BatchPayRollRequestId, p.IsDeleted })
       .IsUnique();

        modelBuilder.Entity<BankDiskette>()
       .HasIndex(p => new { p.PaymentPeriodId, p.BatchPayRollRequestId, p.IsDeleted })
       .IsUnique();

        modelBuilder.Entity<InsuranceDiskette>()
       .HasIndex(p => new { p.PaymentPeriodId, p.BatchPayRollRequestId, p.IsDeleted })
       .IsUnique();


        modelBuilder.Entity<OrganisationLeave>()
       .HasIndex(p => new { p.OrganisationChartId, p.LeaveTypeId, p.IsDeleted })
       .IsUnique();


        // Configure BaseTableValue relationships without FK constraints
        ConfigureBaseTableValueRelationships(modelBuilder);
    }

    private static void ConfigureEmployeeSettlementArchive(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<EmployeeSettlementArchive>()
            .HasIndex(a => a.EmployeeSettlementId)
            .IsUnique();

        modelBuilder.Entity<EmployeeSettlementAttachment>()
            .HasIndex(a => a.EmployeeSettlementId);
    }

    private void ConfigureBaseTableValueRelationships(ModelBuilder modelBuilder)
    {
        // TaxDiskette entity relationships
        modelBuilder.Entity<TaxDiskette>()
            .Property<long>("TaxDisketteStatusId");
        modelBuilder.Entity<TaxDiskette>()
            .HasOne(t => t.TaxDisketteStatus)
            .WithMany()
            .HasForeignKey("TaxDisketteStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        // TaxDisketteFile entity relationships
        modelBuilder.Entity<TaxDisketteFile>()
            .Property<long>("FileTypeId");
        modelBuilder.Entity<TaxDisketteFile>()
            .HasOne(t => t.FileType)
            .WithMany()
            .HasForeignKey("FileTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // InsuranceDiskette entity relationships
        modelBuilder.Entity<InsuranceDiskette>()
            .Property<long>("InsuranceDisketteStatusId");
        modelBuilder.Entity<InsuranceDiskette>()
            .HasOne(i => i.InsuranceDisketteStatus)
            .WithMany()
            .HasForeignKey("InsuranceDisketteStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<InsuranceDiskette>()
            .Property<long>("ReportTypeId");
        modelBuilder.Entity<InsuranceDiskette>()
            .HasOne(i => i.ReportType)
            .WithMany()
            .HasForeignKey("ReportTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // InsuranceDisketteFile entity relationships
        modelBuilder.Entity<InsuranceDisketteFile>()
            .Property<long>("FileTypeId");
        modelBuilder.Entity<InsuranceDisketteFile>()
            .HasOne(i => i.FileType)
            .WithMany()
            .HasForeignKey("FileTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // BankDiskette entity relationships
        modelBuilder.Entity<BankDiskette>()
            .Property<long>("BankDisketteStatusId");
        modelBuilder.Entity<BankDiskette>()
            .HasOne(b => b.BankDisketteStatus)
            .WithMany()
            .HasForeignKey("BankDisketteStatusId")
            .OnDelete(DeleteBehavior.NoAction);

        // BatchPayRollRequest entity relationships
        modelBuilder.Entity<BatchPayRollRequest>()
            .Property<long>("RequestStateId");


        modelBuilder.Entity<BatchPayRollRequest>()
            .Property<long>("RequestTypeId");

        modelBuilder.Entity<BatchSettlementRequest>()
            .Property<long>("RequestStateId");

        modelBuilder.Entity<BatchSettlementRequest>()
            .Property<long>("RequestTypeId");





        // OrganisationFicheItem entity relationships
        modelBuilder.Entity<OrganisationFicheItem>()
            .Property<long>("EnterTypeId");
        modelBuilder.Entity<OrganisationFicheItem>()
            .HasOne(o => o.EnterType)
            .WithMany()
            .HasForeignKey("EnterTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<OrganisationFicheItem>()
            .Property<long>("PaymentTypeId");
        modelBuilder.Entity<OrganisationFicheItem>()
            .HasOne(o => o.PaymentType)
            .WithMany()
            .HasForeignKey("PaymentTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // PersonnelFicheItem entity relationships
        modelBuilder.Entity<PersonnelFicheItem>()
            .Property<long>("EnterTypeId");
        modelBuilder.Entity<PersonnelFicheItem>()
            .HasOne(p => p.EnterType)
            .WithMany()
            .HasForeignKey("EnterTypeId")
            .OnDelete(DeleteBehavior.NoAction);

        // PersonnelFicheItem does not expose PaymentType navigation; omit nav binding
        // If needed in the future, use navless relationship:
        // modelBuilder.Entity<PersonnelFicheItem>()
        //     .Property<long>("PaymentTypeId");
        // modelBuilder.Entity<PersonnelFicheItem>()
        //     .HasOne<BaseTableValue>()
        //     .WithMany()
        //     .HasForeignKey("PaymentTypeId")
        //     .OnDelete(DeleteBehavior.NoAction);

        // CalclulationSetting entity relationships
        modelBuilder.Entity<CalclulationSetting>()
            .Property<long?>("RewardAndSanavatStoreTypeId");
        modelBuilder.Entity<CalclulationSetting>()
            .HasOne(c => c.RewardAndSanavatStoreType)
            .WithMany()
            .HasForeignKey("RewardAndSanavatStoreTypeId")
            .OnDelete(DeleteBehavior.NoAction);



        // FunctionExcelDefinition entity relationships
        modelBuilder.Entity<FunctionExcelDefinition>()
            .Property<long>("MappedExcelColumnId");
        modelBuilder.Entity<FunctionExcelDefinition>()
            .HasOne(f => f.MappedExcelColumn)
            .WithMany()
            .HasForeignKey("MappedExcelColumnId")
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<FunctionExcelDefinition>()
            .Property<long?>("PersonnelFunctionColumnId");
        modelBuilder.Entity<FunctionExcelDefinition>()
            .HasOne(f => f.PersonnelFunctionColumn)
            .WithMany()
            .HasForeignKey("PersonnelFunctionColumnId")
            .OnDelete(DeleteBehavior.NoAction);



        // BlackList entity relationships
        modelBuilder.Entity<BlackList>()
            .Property<long>("BlackListEnumerationId");


        // Explicit ArearsStatus relationships for concrete PersonnelFunction entities
        modelBuilder.Entity<PersonnelFunction>()
            .HasOne(p => p.ArearsStatus)
            .WithMany()
            .HasForeignKey(p => p.ArearsStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // PersonnelFunctionVisible has no relationships; all boolean columns

        modelBuilder.Entity<TempPersonnelFunction>()
            .HasOne(p => p.ArearsStatus)
            .WithMany()
            .HasForeignKey(p => p.ArearsStatusId)
            .OnDelete(DeleteBehavior.NoAction);

        // Explicit OrganisationChart relationships for concrete PersonnelFunction entities
        modelBuilder.Entity<PersonnelFunction>()
            .HasOne(p => p.CostCenter)
            .WithMany()
            .HasForeignKey(p => p.CostCenterId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PersonnelFunction>()
            .HasOne(p => p.OrganizationUnit)
            .WithMany()
            .HasForeignKey(p => p.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PersonnelFunction>()
            .HasOne(p => p.WorkPlace)
            .WithMany()
            .HasForeignKey(p => p.WorkPlaceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<PersonnelFunction>()
            .HasOne(p => p.OrganisationChart)
            .WithMany()
            .HasForeignKey(p => p.OrganisationChartId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TempPersonnelFunction>()
            .HasOne(p => p.CostCenter)
            .WithMany()
            .HasForeignKey(p => p.CostCenterId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TempPersonnelFunction>()
            .HasOne(p => p.OrganizationUnit)
            .WithMany()
            .HasForeignKey(p => p.OrganizationUnitId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TempPersonnelFunction>()
            .HasOne(p => p.WorkPlace)
            .WithMany()
            .HasForeignKey(p => p.WorkPlaceId)
            .OnDelete(DeleteBehavior.NoAction);

        modelBuilder.Entity<TempPersonnelFunction>()
            .HasOne(p => p.OrganisationChart)
            .WithMany()
            .HasForeignKey(p => p.OrganisationChartId)
            .OnDelete(DeleteBehavior.NoAction);

        // Add more relationships as needed...
    }
    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {
        // Suppress EF relational warning that is overly strict for our TPC + cross-schema FKs
        optionsBuilder.ConfigureWarnings(w => w.Ignore(RelationalEventId.ForeignKeyPropertiesMappedToUnrelatedTables));
        base.OnConfiguring(optionsBuilder);
    }
}
