using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Chart", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganizationTypeId", Name = "IX_Organisation_Chart_OrganizationTypeId")]
[Microsoft.EntityFrameworkCore.Index("ParentOrganisationChartId", Name = "IX_Organisation_Chart_ParentOrganisationChartId")]
public partial class OrganisationChart
{
    [Key]
    public long Id { get; set; }

    public long? ParentOrganisationChartId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsPayLocation { get; set; }

    public bool IsApproved { get; set; }

    public bool? IsCostCenter { get; set; }

    public bool? IsIndependentOrg { get; set; }

    public bool? IsOrg { get; set; }

    public bool? IsRegister { get; set; }

    [StringLength(128)]
    public string? LetterCode { get; set; }

    public int? Order { get; set; }

    [StringLength(128)]
    public string? Rank { get; set; }

    [StringLength(128)]
    public string? Code { get; set; }

    public long? OrganizationTypeId { get; set; }

    [StringLength(128)]
    public string? SystemCode { get; set; }

    [StringLength(128)]
    public string? UniqueIdentifier { get; set; }

    [StringLength(128)]
    public string? AbbreviationMark { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(128)]
    public string? ShortName { get; set; }

    public bool IsRoot { get; set; }

    public long? OrgTypeId { get; set; }

    public long? TaxNodeStatusId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Ability> Abilities { get; set; } = new List<Ability>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Appearance> Appearances { get; set; } = new List<Appearance>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Arear> Arears { get; set; } = new List<Arear>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<AttendanceDevice> AttendanceDevices { get; set; } = new List<AttendanceDevice>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<AttendanceEmployeeShiftAssignment> AttendanceEmployeeShiftAssignments { get; set; } = new List<AttendanceEmployeeShiftAssignment>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<AttendanceLocation> AttendanceLocations { get; set; } = new List<AttendanceLocation>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<AttendanceShift> AttendanceShifts { get; set; } = new List<AttendanceShift>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BankAccount> BankAccounts { get; set; } = new List<BankAccount>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<BankDisketteCostCenter> BankDisketteCostCenters { get; set; } = new List<BankDisketteCostCenter>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<BankDisketteItem> BankDisketteItems { get; set; } = new List<BankDisketteItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BankDisketteTemplate> BankDisketteTemplates { get; set; } = new List<BankDisketteTemplate>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BankDiskette> BankDiskettes { get; set; } = new List<BankDiskette>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BasijGrade> BasijGrades { get; set; } = new List<BasijGrade>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Basij> Basijs { get; set; } = new List<Basij>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BatchRequest> BatchRequests { get; set; } = new List<BatchRequest>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<BillDetailException> BillDetailExceptions { get; set; } = new List<BillDetailException>();

    [InverseProperty("BuyerCostCenter")]
    public virtual ICollection<BillInstance> BillInstanceBuyerCostCenters { get; set; } = new List<BillInstance>();

    [InverseProperty("SellerCostCenter")]
    public virtual ICollection<BillInstance> BillInstanceSellerCostCenters { get; set; } = new List<BillInstance>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Bill> Bills { get; set; } = new List<Bill>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BlackList> BlackLists { get; set; } = new List<BlackList>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<BlockedAccount> BlockedAccounts { get; set; } = new List<BlockedAccount>();

    [InverseProperty("OrganisationChart")]
    public virtual CalclulationSetting? CalclulationSetting { get; set; }

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Captivity> Captivities { get; set; } = new List<Captivity>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Character> Characters { get; set; } = new List<Character>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Coefficient1> Coefficient1s { get; set; } = new List<Coefficient1>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Competency> Competencies { get; set; } = new List<Competency>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<ContactInfo> ContactInfos { get; set; } = new List<ContactInfo>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<CostCenterFicheItem> CostCenterFicheItemCostCenters { get; set; } = new List<CostCenterFicheItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<CostCenterFicheItem> CostCenterFicheItemOrganisationCharts { get; set; } = new List<CostCenterFicheItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Course> Courses { get; set; } = new List<Course>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<DeductionType> DeductionTypes { get; set; } = new List<DeductionType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Disability> Disabilities { get; set; } = new List<Disability>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<DrivingLicense> DrivingLicenses { get; set; } = new List<DrivingLicense>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<DynamicReport> DynamicReports { get; set; } = new List<DynamicReport>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Education> Educations { get; set; } = new List<Education>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeDeductionUploadBatch> EmployeeDeductionUploadBatches { get; set; } = new List<EmployeeDeductionUploadBatch>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeFile> EmployeeFiles { get; set; } = new List<EmployeeFile>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeFund> EmployeeFunds { get; set; } = new List<EmployeeFund>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; } = new List<EmployeeLeaveEntitlement>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeSettlement> EmployeeSettlements { get; set; } = new List<EmployeeSettlement>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EmployeeSoftware> EmployeeSoftwares { get; set; } = new List<EmployeeSoftware>();

    [InverseProperty("BaseOrganisation")]
    public virtual ICollection<Employee> Employees { get; set; } = new List<Employee>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<EvaluationResult> EvaluationResults { get; set; } = new List<EvaluationResult>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Experience> Experiences { get; set; } = new List<Experience>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Family> Families { get; set; } = new List<Family>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<Fiche> FicheCostCenters { get; set; } = new List<Fiche>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Fiche> FicheOrganisationCharts { get; set; } = new List<Fiche>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<File1> File1s { get; set; } = new List<File1>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<ForeignLanguage> ForeignLanguages { get; set; } = new List<ForeignLanguage>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<ForeignTravel> ForeignTravels { get; set; } = new List<ForeignTravel>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<FormulaDatabaseFunctionDefinition> FormulaDatabaseFunctionDefinitions { get; set; } = new List<FormulaDatabaseFunctionDefinition>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<FormulaTable> FormulaTables { get; set; } = new List<FormulaTable>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<FunctionExcelDefinition> FunctionExcelDefinitions { get; set; } = new List<FunctionExcelDefinition>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<G20ScoreDomainJobDegree> G20ScoreDomainJobDegrees { get; set; } = new List<G20ScoreDomainJobDegree>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; } = new List<GroupPunishmentEncourage>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<HistoryStop> HistoryStops { get; set; } = new List<HistoryStop>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Image> Images { get; set; } = new List<Image>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<InsuranceBranch> InsuranceBranches { get; set; } = new List<InsuranceBranch>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<InsuranceDisketteCostCenter> InsuranceDisketteCostCenters { get; set; } = new List<InsuranceDisketteCostCenter>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<InsuranceDisketteItem> InsuranceDisketteItems { get; set; } = new List<InsuranceDisketteItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<InsuranceDiskette> InsuranceDiskettes { get; set; } = new List<InsuranceDiskette>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Insurance> Insurances { get; set; } = new List<Insurance>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<InterdictOrderCopy> InterdictOrderCopies { get; set; } = new List<InterdictOrderCopy>();

    [InverseProperty("ParentOrganisationChart")]
    public virtual ICollection<OrganisationChart> InverseParentOrganisationChart { get; set; } = new List<OrganisationChart>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Isar> Isars { get; set; } = new List<Isar>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<LoanType> LoanTypes { get; set; } = new List<LoanType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<MilitaryService> MilitaryServices { get; set; } = new List<MilitaryService>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<MinimumMonthlyWage> MinimumMonthlyWages { get; set; } = new List<MinimumMonthlyWage>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<NodeRoleRel> NodeRoleRels { get; set; } = new List<NodeRoleRel>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<NodeUserRel> NodeUserRels { get; set; } = new List<NodeUserRel>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Node> Nodes { get; set; } = new List<Node>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrderTempFile> OrderTempFiles { get; set; } = new List<OrderTempFile>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganProperty> OrganProperties { get; set; } = new List<OrganProperty>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationAgentOfPunishmentEncourageScoreInterval> OrganisationAgentOfPunishmentEncourageScoreIntervals { get; set; } = new List<OrganisationAgentOfPunishmentEncourageScoreInterval>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationAgentOfPunishmentEncourage> OrganisationAgentOfPunishmentEncourages { get; set; } = new List<OrganisationAgentOfPunishmentEncourage>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationChartImage> OrganisationChartImages { get; set; } = new List<OrganisationChartImage>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationCoefficient> OrganisationCoefficients { get; set; } = new List<OrganisationCoefficient>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<OrganisationCostCenter> OrganisationCostCenterCostCenters { get; set; } = new List<OrganisationCostCenter>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationCostCenter> OrganisationCostCenterOrganisationCharts { get; set; } = new List<OrganisationCostCenter>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeStatus> OrganisationEmployeeStatuses { get; set; } = new List<OrganisationEmployeeStatus>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; } = new List<OrganisationEmployeeTypeCoefficientBonusWageItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeCoefficient> OrganisationEmployeeTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeCoefficient>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItems { get; set; } = new List<OrganisationEmployeeTypeFicheItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitions { get; set; } = new List<OrganisationEmployeeTypeFundTypeDefinition>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeLeave> OrganisationEmployeeTypeLeaves { get; set; } = new List<OrganisationEmployeeTypeLeave>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeMrt> OrganisationEmployeeTypeMrts { get; set; } = new List<OrganisationEmployeeTypeMrt>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChanges { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCanChange>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCheck>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCoefficient>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeDescription> OrganisationEmployeeTypeOrderTypeDescriptions { get; set; } = new List<OrganisationEmployeeTypeOrderTypeDescription>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeSummaryCalc> OrganisationEmployeeTypeOrderTypeSummaryCalcs { get; set; } = new List<OrganisationEmployeeTypeOrderTypeSummaryCalc>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeOrderTypeWageItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeOrderType> OrganisationEmployeeTypeOrderTypes { get; set; } = new List<OrganisationEmployeeTypeOrderType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; } = new List<OrganisationEmployeeTypeSettlementItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeTypeWageItem> OrganisationEmployeeTypeWageItems { get; set; } = new List<OrganisationEmployeeTypeWageItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationEmployeeType> OrganisationEmployeeTypes { get; set; } = new List<OrganisationEmployeeType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationFicheItem> OrganisationFicheItems { get; set; } = new List<OrganisationFicheItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationFormula> OrganisationFormulas { get; set; } = new List<OrganisationFormula>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationFundType> OrganisationFundTypes { get; set; } = new List<OrganisationFundType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationInsJobList> OrganisationInsJobLists { get; set; } = new List<OrganisationInsJobList>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationJobCategory> OrganisationJobCategories { get; set; } = new List<OrganisationJobCategory>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationJobGroup> OrganisationJobGroups { get; set; } = new List<OrganisationJobGroup>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationJobSeries> OrganisationJobSeries { get; set; } = new List<OrganisationJobSeries>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationJobSkillYearSetting> OrganisationJobSkillYearSettings { get; set; } = new List<OrganisationJobSkillYearSetting>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationJob> OrganisationJobs { get; set; } = new List<OrganisationJob>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationLeave> OrganisationLeaves { get; set; } = new List<OrganisationLeave>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationMrt> OrganisationMrts { get; set; } = new List<OrganisationMrt>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationOrderTypeHistoryExclusion> OrganisationOrderTypeHistoryExclusions { get; set; } = new List<OrganisationOrderTypeHistoryExclusion>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationOrderType> OrganisationOrderTypes { get; set; } = new List<OrganisationOrderType>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPeymanRow> OrganisationPeymanRows { get; set; } = new List<OrganisationPeymanRow>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPositionJob> OrganisationPositionJobs { get; set; } = new List<OrganisationPositionJob>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPositionOccuptionMoreThanOneCach> OrganisationPositionOccuptionMoreThanOneCaches { get; set; } = new List<OrganisationPositionOccuptionMoreThanOneCach>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPositionSuggested> OrganisationPositionSuggesteds { get; set; } = new List<OrganisationPositionSuggested>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPositionSupervisor> OrganisationPositionSupervisors { get; set; } = new List<OrganisationPositionSupervisor>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationPosition> OrganisationPositions { get; set; } = new List<OrganisationPosition>();

    [InverseProperty("RelatedNode")]
    public virtual ICollection<OrganisationPosition> RelatedNodeOrganisationPositions { get; set; } = new List<OrganisationPosition>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationProject> OrganisationProjects { get; set; } = new List<OrganisationProject>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationSetting> OrganisationSettings { get; set; } = new List<OrganisationSetting>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationSettlementCause> OrganisationSettlementCauses { get; set; } = new List<OrganisationSettlementCause>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationSettlementItem> OrganisationSettlementItems { get; set; } = new List<OrganisationSettlementItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationTempFile> OrganisationTempFiles { get; set; } = new List<OrganisationTempFile>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationWageItem> OrganisationWageItems { get; set; } = new List<OrganisationWageItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganisationWorkPlace> OrganisationWorkPlaceOrganisationCharts { get; set; } = new List<OrganisationWorkPlace>();

    [InverseProperty("WorkPlace")]
    public virtual ICollection<OrganisationWorkPlace> OrganisationWorkPlaceWorkPlaces { get; set; } = new List<OrganisationWorkPlace>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OrganizationGoal> OrganizationGoals { get; set; } = new List<OrganizationGoal>();

    [ForeignKey("OrganizationTypeId")]
    [InverseProperty("OrganisationCharts")]
    public virtual OrganizationType? OrganizationType { get; set; }

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<OtherVeteran> OtherVeterans { get; set; } = new List<OtherVeteran>();

    [ForeignKey("ParentOrganisationChartId")]
    [InverseProperty("InverseParentOrganisationChart")]
    public virtual OrganisationChart? ParentOrganisationChart { get; set; }

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PayLocationProgressReport> PayLocationProgressReports { get; set; } = new List<PayLocationProgressReport>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PaymentPeriod> PaymentPeriods { get; set; } = new List<PaymentPeriod>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelFicheItem> PersonnelFicheItems { get; set; } = new List<PersonnelFicheItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; } = new List<PersonnelFunctionExcelFile>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelFunctionVisible> PersonnelFunctionVisibles { get; set; } = new List<PersonnelFunctionVisible>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelLeave> PersonnelLeaves { get; set; } = new List<PersonnelLeave>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelLoan> PersonnelLoans { get; set; } = new List<PersonnelLoan>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PersonnelManagerList> PersonnelManagerLists { get; set; } = new List<PersonnelManagerList>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<PunishmentEncourage> PunishmentEncourages { get; set; } = new List<PunishmentEncourage>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<RecruitOrder> RecruitOrderCostCenters { get; set; } = new List<RecruitOrder>();

    [InverseProperty("OrganizationUnit")]
    public virtual ICollection<RecruitOrder> RecruitOrderOrganizationUnits { get; set; } = new List<RecruitOrder>();

    [InverseProperty("PayLocation")]
    public virtual ICollection<RecruitOrder> RecruitOrderPayLocations { get; set; } = new List<RecruitOrder>();

    [InverseProperty("WorkPlace")]
    public virtual ICollection<RecruitOrder> RecruitOrderWorkPlaces { get; set; } = new List<RecruitOrder>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<RoleReport> RoleReports { get; set; } = new List<RoleReport>();

    [InverseProperty("CurrentServiceLocation")]
    public virtual ICollection<StatusListItem> StatusListItemCurrentServiceLocations { get; set; } = new List<StatusListItem>();

    [InverseProperty("OrganCodes")]
    public virtual ICollection<StatusListItem> StatusListItemOrganCodes { get; set; } = new List<StatusListItem>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TashkilatTempFile> TashkilatTempFiles { get; set; } = new List<TashkilatTempFile>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TaxCoefficientItem> TaxCoefficientItems { get; set; } = new List<TaxCoefficientItem>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<TaxDisketteCostCenter> TaxDisketteCostCenters { get; set; } = new List<TaxDisketteCostCenter>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TaxDiskette> TaxDiskettes { get; set; } = new List<TaxDiskette>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TaxNonCashPayment> TaxNonCashPayments { get; set; } = new List<TaxNonCashPayment>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TaxTable> TaxTables { get; set; } = new List<TaxTable>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TaxableIncome> TaxableIncomes { get; set; } = new List<TaxableIncome>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Tax> Taxes { get; set; } = new List<Tax>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TempEmployeeDeduction> TempEmployeeDeductions { get; set; } = new List<TempEmployeeDeduction>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TempPersonnelLeave> TempPersonnelLeaves { get; set; } = new List<TempPersonnelLeave>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<TempPunishmentEncourage> TempPunishmentEncourages { get; set; } = new List<TempPunishmentEncourage>();

    [InverseProperty("CostCenter")]
    public virtual ICollection<UserCostCenter> UserCostCenterCostCenters { get; set; } = new List<UserCostCenter>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<UserCostCenter> UserCostCenterOrganisationCharts { get; set; } = new List<UserCostCenter>();

    [InverseProperty("DefaultOrgan")]
    public virtual ICollection<UserDefaultSetting> UserDefaultSettings { get; set; } = new List<UserDefaultSetting>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<UserOrganizationUnit> UserOrganizationUnitOrganisationCharts { get; set; } = new List<UserOrganizationUnit>();

    [InverseProperty("OrganizationUnit")]
    public virtual ICollection<UserOrganizationUnit> UserOrganizationUnitOrganizationUnits { get; set; } = new List<UserOrganizationUnit>();

    [InverseProperty("PayLocation")]
    public virtual ICollection<UserPayLocation> UserPayLocations { get; set; } = new List<UserPayLocation>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<UserReport> UserReports { get; set; } = new List<UserReport>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<UserSignature> UserSignatures { get; set; } = new List<UserSignature>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<UserWorkPlace> UserWorkPlaceOrganisationCharts { get; set; } = new List<UserWorkPlace>();

    [InverseProperty("WorkPlace")]
    public virtual ICollection<UserWorkPlace> UserWorkPlaceWorkPlaces { get; set; } = new List<UserWorkPlace>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<War> Wars { get; set; } = new List<War>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<WorkFlow> WorkFlows { get; set; } = new List<WorkFlow>();

    [InverseProperty("OrgChartWorkPlace")]
    public virtual ICollection<Work> WorkOrgChartWorkPlaces { get; set; } = new List<Work>();

    [InverseProperty("OrganisationChart")]
    public virtual ICollection<Work> WorkOrganisationCharts { get; set; } = new List<Work>();
}
