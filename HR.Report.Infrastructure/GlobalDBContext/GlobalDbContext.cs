using System;
using System.Collections.Generic;
using HR.Report.Infrastructure.GlobalDBContext.Models;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext;

public partial class GlobalDbContext : DbContext
{
    public GlobalDbContext()
    {
    }

    public GlobalDbContext(DbContextOptions<GlobalDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<Ability> Abilities { get; set; }

    public virtual DbSet<AbsenceRecord> AbsenceRecords { get; set; }

    public virtual DbSet<AbsenceType> AbsenceTypes { get; set; }

    public virtual DbSet<AbsenceTypeValue> AbsenceTypeValues { get; set; }

    public virtual DbSet<Abundance> Abundances { get; set; }

    public virtual DbSet<Models.Action> Actions { get; set; }

    public virtual DbSet<ActivityTemplate> ActivityTemplates { get; set; }

    public virtual DbSet<AgentOfPunishmentEncourage> AgentOfPunishmentEncourages { get; set; }

    public virtual DbSet<AgentOfPunishmentEncourageGroup> AgentOfPunishmentEncourageGroups { get; set; }

    public virtual DbSet<Appearance> Appearances { get; set; }

    public virtual DbSet<Arear> Arears { get; set; }

    public virtual DbSet<ArearFiche> ArearFiches { get; set; }

    public virtual DbSet<ArearFicheItem> ArearFicheItems { get; set; }

    public virtual DbSet<ArearsChangedFicheItem> ArearsChangedFicheItems { get; set; }

    public virtual DbSet<ArearsStatus> ArearsStatuses { get; set; }

    public virtual DbSet<AspNetRole> AspNetRoles { get; set; }

    public virtual DbSet<AspNetUser> AspNetUsers { get; set; }

    public virtual DbSet<Attendance> Attendances { get; set; }

    public virtual DbSet<AttendanceCalendar> AttendanceCalendars { get; set; }

    public virtual DbSet<AttendanceDevice> AttendanceDevices { get; set; }

    public virtual DbSet<AttendanceEmployeeShiftAssignment> AttendanceEmployeeShiftAssignments { get; set; }

    public virtual DbSet<AttendanceHoliday> AttendanceHolidays { get; set; }

    public virtual DbSet<AttendanceLocation> AttendanceLocations { get; set; }

    public virtual DbSet<AttendanceLog> AttendanceLogs { get; set; }

    public virtual DbSet<AttendanceShift> AttendanceShifts { get; set; }
    public virtual DbSet<AttendanceShiftDetail> AttendanceShiftDetails { get; set; }

    public virtual DbSet<AuditLog> AuditLogs { get; set; }

    public virtual DbSet<Bank> Banks { get; set; }

    public virtual DbSet<BankAccount> BankAccounts { get; set; }

    public virtual DbSet<BankBranch> BankBranches { get; set; }

    public virtual DbSet<BankDiskette> BankDiskettes { get; set; }

    public virtual DbSet<BankDisketteCostCenter> BankDisketteCostCenters { get; set; }

    public virtual DbSet<BankDisketteGroupAndFile> BankDisketteGroupAndFiles { get; set; }

    public virtual DbSet<BankDisketteItem> BankDisketteItems { get; set; }

    public virtual DbSet<BankDisketteTemplate> BankDisketteTemplates { get; set; }

    public virtual DbSet<BankDisketteTemplateRow> BankDisketteTemplateRows { get; set; }

    public virtual DbSet<BaseTable> BaseTables { get; set; }

    public virtual DbSet<BaseTableValue> BaseTableValues { get; set; }

    public virtual DbSet<BaseWorkPlace> BaseWorkPlaces { get; set; }

    public virtual DbSet<Basij> Basijs { get; set; }

    public virtual DbSet<BasijGrade> BasijGrades { get; set; }

    public virtual DbSet<BatchLog> BatchLogs { get; set; }

    public virtual DbSet<BatchPayRollRequest> BatchPayRollRequests { get; set; }

    public virtual DbSet<BatchPayRollRequestDetail> BatchPayRollRequestDetails { get; set; }

    public virtual DbSet<BatchRequest> BatchRequests { get; set; }

    public virtual DbSet<BatchRequestDetail> BatchRequestDetails { get; set; }

    public virtual DbSet<BatchRequestDetailReference> BatchRequestDetailReferences { get; set; }

    public virtual DbSet<BatchRequestFile> BatchRequestFiles { get; set; }

    public virtual DbSet<Bill> Bills { get; set; }

    public virtual DbSet<BillDetail> BillDetails { get; set; }

    public virtual DbSet<BillDetailException> BillDetailExceptions { get; set; }

    public virtual DbSet<BillInstance> BillInstances { get; set; }

    public virtual DbSet<BillItem> BillItems { get; set; }

    public virtual DbSet<BlackList> BlackLists { get; set; }

    public virtual DbSet<BlockedAccount> BlockedAccounts { get; set; }

    public virtual DbSet<BlockedIp> BlockedIps { get; set; }

    public virtual DbSet<CalclulationSetting> CalclulationSettings { get; set; }

    public virtual DbSet<Captivity> Captivities { get; set; }

    public virtual DbSet<Carousel> Carousels { get; set; }

    public virtual DbSet<Character> Characters { get; set; }

    public virtual DbSet<Coefficient> Coefficients { get; set; }

    public virtual DbSet<Coefficient1> Coefficients1 { get; set; }

    public virtual DbSet<CommonPassword> CommonPasswords { get; set; }

    public virtual DbSet<Competency> Competencies { get; set; }

    public virtual DbSet<Complexity> Complexities { get; set; }

    public virtual DbSet<ConfidentialityLevel> ConfidentialityLevels { get; set; }

    public virtual DbSet<ContactInfo> ContactInfos { get; set; }

    public virtual DbSet<CostCenterFicheItem> CostCenterFicheItems { get; set; }

    public virtual DbSet<Course> Courses { get; set; }

    public virtual DbSet<DeductedArear> DeductedArears { get; set; }

    public virtual DbSet<DeductedArearsDetail> DeductedArearsDetails { get; set; }

    public virtual DbSet<DeductionType> DeductionTypes { get; set; }

    public virtual DbSet<Definition> Definitions { get; set; }

    public virtual DbSet<Disability> Disabilities { get; set; }

    public virtual DbSet<DrivingLicense> DrivingLicenses { get; set; }

    public virtual DbSet<DynamicReport> DynamicReports { get; set; }

    public virtual DbSet<DynamicReportParameter> DynamicReportParameters { get; set; }

    public virtual DbSet<Education> Educations { get; set; }

    public virtual DbSet<EducationField> EducationFields { get; set; }

    public virtual DbSet<EducationGrade> EducationGrades { get; set; }

    public virtual DbSet<EducationGroup> EducationGroups { get; set; }

    public virtual DbSet<EducationOrientation> EducationOrientations { get; set; }

    public virtual DbSet<EfmigrationsHistroy> EfmigrationsHistroys { get; set; }

    public virtual DbSet<EfmigrationsHistroy1> EfmigrationsHistroys1 { get; set; }

    public virtual DbSet<EfmigrationsHistroy10> EfmigrationsHistroys10 { get; set; }

    public virtual DbSet<EfmigrationsHistroy11> EfmigrationsHistroys11 { get; set; }

    public virtual DbSet<EfmigrationsHistroy2> EfmigrationsHistroys2 { get; set; }

    public virtual DbSet<EfmigrationsHistroy3> EfmigrationsHistroys3 { get; set; }

    public virtual DbSet<EfmigrationsHistroy4> EfmigrationsHistroys4 { get; set; }

    public virtual DbSet<EfmigrationsHistroy5> EfmigrationsHistroys5 { get; set; }

    public virtual DbSet<EfmigrationsHistroy6> EfmigrationsHistroys6 { get; set; }

    public virtual DbSet<EfmigrationsHistroy7> EfmigrationsHistroys7 { get; set; }

    public virtual DbSet<EfmigrationsHistroy8> EfmigrationsHistroys8 { get; set; }

    public virtual DbSet<EfmigrationsHistroy9> EfmigrationsHistroys9 { get; set; }

    public virtual DbSet<Models.Employee> Employees { get; set; }

    public virtual DbSet<EmployeeDeduction> EmployeeDeductions { get; set; }

    public virtual DbSet<EmployeeDeductionPayment> EmployeeDeductionPayments { get; set; }

    public virtual DbSet<EmployeeDeductionUploadBatch> EmployeeDeductionUploadBatches { get; set; }

    public virtual DbSet<EmployeeFile> EmployeeFiles { get; set; }

    public virtual DbSet<EmployeeFund> EmployeeFunds { get; set; }

    public virtual DbSet<EmployeeLeaveEntitlement> EmployeeLeaveEntitlements { get; set; }

    public virtual DbSet<EmployeeLoginHistory> EmployeeLoginHistories { get; set; }

    public virtual DbSet<EmployeeOtp> EmployeeOtps { get; set; }

    public virtual DbSet<EmployeeProperty> EmployeeProperties { get; set; }

    public virtual DbSet<EmployeeRefreshToken> EmployeeRefreshTokens { get; set; }

    public virtual DbSet<EmployeeRequest> EmployeeRequests { get; set; }

    public virtual DbSet<EmployeeRequestDetail> EmployeeRequestDetails { get; set; }

    public virtual DbSet<EmployeeRequestStatus> EmployeeRequestStatuses { get; set; }

    public virtual DbSet<EmployeeSettlement> EmployeeSettlements { get; set; }

    public virtual DbSet<EmployeeSettlementItem> EmployeeSettlementItems { get; set; }

    public virtual DbSet<EmployeeSoftware> EmployeeSoftwares { get; set; }

    public virtual DbSet<EmployeeStatus> EmployeeStatuses { get; set; }

    public virtual DbSet<EmployeeStatusGroup> EmployeeStatusGroups { get; set; }

    public virtual DbSet<EmployeeType> EmployeeTypes { get; set; }

    public virtual DbSet<EmployeeTypeGroup> EmployeeTypeGroups { get; set; }

    public virtual DbSet<EvaluationResult> EvaluationResults { get; set; }

    public virtual DbSet<ExcelDefinitionType> ExcelDefinitionTypes { get; set; }

    public virtual DbSet<Experience> Experiences { get; set; }

    public virtual DbSet<Family> Families { get; set; }

    public virtual DbSet<Faq> Faqs { get; set; }

    public virtual DbSet<Feedback> Feedbacks { get; set; }

    public virtual DbSet<Fiche> Fiches { get; set; }

    public virtual DbSet<FicheItem> FicheItems { get; set; }

    public virtual DbSet<FicheLeaveItem> FicheLeaveItems { get; set; }

    public virtual DbSet<FichePdfArchive> FichePdfArchives { get; set; }

    public virtual DbSet<FicheReportArchive> FicheReportArchives { get; set; }

    public virtual DbSet<FicheStatus> FicheStatuses { get; set; }

    public virtual DbSet<FieldDataType> FieldDataTypes { get; set; }

    public virtual DbSet<FieldOperator> FieldOperators { get; set; }

    public virtual DbSet<Models.File> Files { get; set; }

    public virtual DbSet<File1> Files1 { get; set; }

    public virtual DbSet<ForeignLanguage> ForeignLanguages { get; set; }

    public virtual DbSet<ForeignTravel> ForeignTravels { get; set; }

    public virtual DbSet<Formula> Formulas { get; set; }

    public virtual DbSet<FormulaDatabaseFunctionDefinition> FormulaDatabaseFunctionDefinitions { get; set; }

    public virtual DbSet<FormulaDefinition> FormulaDefinitions { get; set; }

    public virtual DbSet<FormulaDefinitionHistory> FormulaDefinitionHistories { get; set; }

    public virtual DbSet<FormulaOperand> FormulaOperands { get; set; }

    public virtual DbSet<FormulaTable> FormulaTables { get; set; }

    public virtual DbSet<FormulaTableValue> FormulaTableValues { get; set; }

    public virtual DbSet<FormulaUsageLocation> FormulaUsageLocations { get; set; }

    public virtual DbSet<FunctionExcelDefinition> FunctionExcelDefinitions { get; set; }

    public virtual DbSet<FundType> FundTypes { get; set; }

    public virtual DbSet<G20ScoreDomainJobDegree> G20ScoreDomainJobDegrees { get; set; }

    public virtual DbSet<GroupPunishmentEncourage> GroupPunishmentEncourages { get; set; }

    public virtual DbSet<GroupPunishmentEncourageFile> GroupPunishmentEncourageFiles { get; set; }

    public virtual DbSet<HistoryStop> HistoryStops { get; set; }

    public virtual DbSet<HistoryType> HistoryTypes { get; set; }

    public virtual DbSet<Image> Images { get; set; }

    public virtual DbSet<ImageAttachment> ImageAttachments { get; set; }

    public virtual DbSet<Insurance> Insurances { get; set; }

    public virtual DbSet<InsuranceBranch> InsuranceBranches { get; set; }

    public virtual DbSet<InsuranceDetail> InsuranceDetails { get; set; }

    public virtual DbSet<InsuranceDiskette> InsuranceDiskettes { get; set; }

    public virtual DbSet<InsuranceDisketteCostCenter> InsuranceDisketteCostCenters { get; set; }

    public virtual DbSet<InsuranceDisketteFile> InsuranceDisketteFiles { get; set; }

    public virtual DbSet<InsuranceDisketteItem> InsuranceDisketteItems { get; set; }

    public virtual DbSet<InsurancePosition> InsurancePositions { get; set; }

    public virtual DbSet<InsuranceType> InsuranceTypes { get; set; }

    public virtual DbSet<InterdictOrder> InterdictOrders { get; set; }

    public virtual DbSet<InterdictOrderArchive> InterdictOrderArchives { get; set; }

    public virtual DbSet<InterdictOrderCoefficientItem> InterdictOrderCoefficientItems { get; set; }

    public virtual DbSet<InterdictOrderCopy> InterdictOrderCopies { get; set; }

    public virtual DbSet<InterdictOrderPromissory> InterdictOrderPromissories { get; set; }

    public virtual DbSet<InterdictOrderWageItem> InterdictOrderWageItems { get; set; }

    public virtual DbSet<Isar> Isars { get; set; }

    public virtual DbSet<Job> Jobs { get; set; }

    public virtual DbSet<JobAbundanceJobScoringFactorQuestion> JobAbundanceJobScoringFactorQuestions { get; set; }

    public virtual DbSet<JobActivityType> JobActivityTypes { get; set; }

    public virtual DbSet<JobCategory> JobCategories { get; set; }

    public virtual DbSet<JobComplexityJobScoringFactorQuestion> JobComplexityJobScoringFactorQuestions { get; set; }

    public virtual DbSet<JobGroup> JobGroups { get; set; }

    public virtual DbSet<JobLevel> JobLevels { get; set; }

    public virtual DbSet<JobScoreAbundanceComplexity> JobScoreAbundanceComplexities { get; set; }

    public virtual DbSet<JobScoringFactor> JobScoringFactors { get; set; }

    public virtual DbSet<JobSeries> JobSeries { get; set; }

    public virtual DbSet<LeaveType> LeaveTypes { get; set; }

    public virtual DbSet<LoanType> LoanTypes { get; set; }

    public virtual DbSet<Log> Logs { get; set; }

    public virtual DbSet<LoginCredentialLog> LoginCredentialLogs { get; set; }

    public virtual DbSet<ManagementAndStewardshipJob> ManagementAndStewardshipJobs { get; set; }

    public virtual DbSet<Message> Messages { get; set; }

    public virtual DbSet<MessageAttachment> MessageAttachments { get; set; }

    public virtual DbSet<MilitaryService> MilitaryServices { get; set; }

    public virtual DbSet<MinimumMonthlyWage> MinimumMonthlyWages { get; set; }

    public virtual DbSet<Node> Nodes { get; set; }

    public virtual DbSet<NodeRoleRel> NodeRoleRels { get; set; }

    public virtual DbSet<NodeUserRel> NodeUserRels { get; set; }

    public virtual DbSet<OrderStatus> OrderStatuses { get; set; }

    public virtual DbSet<OrderTempFile> OrderTempFiles { get; set; }

    public virtual DbSet<OrderType> OrderTypes { get; set; }

    public virtual DbSet<OrderTypeGroup> OrderTypeGroups { get; set; }

    public virtual DbSet<OrganProperty> OrganProperties { get; set; }

    public virtual DbSet<OrganisationAgentOfPunishmentEncourage> OrganisationAgentOfPunishmentEncourages { get; set; }

    public virtual DbSet<OrganisationAgentOfPunishmentEncourageScoreInterval> OrganisationAgentOfPunishmentEncourageScoreIntervals { get; set; }

    public virtual DbSet<OrganisationChart> OrganisationCharts { get; set; }

    public virtual DbSet<OrganisationChartImage> OrganisationChartImages { get; set; }

    public virtual DbSet<OrganisationCoefficient> OrganisationCoefficients { get; set; }

    public virtual DbSet<OrganisationCostCenter> OrganisationCostCenters { get; set; }

    public virtual DbSet<OrganisationEmployeeStatus> OrganisationEmployeeStatuses { get; set; }

    public virtual DbSet<OrganisationEmployeeType> OrganisationEmployeeTypes { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeCoefficient> OrganisationEmployeeTypeCoefficients { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeCoefficientBonusWageItem> OrganisationEmployeeTypeCoefficientBonusWageItems { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeFicheItem> OrganisationEmployeeTypeFicheItems { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeFundTypeDefinition> OrganisationEmployeeTypeFundTypeDefinitions { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeLeave> OrganisationEmployeeTypeLeaves { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeMrt> OrganisationEmployeeTypeMrts { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderType> OrganisationEmployeeTypeOrderTypes { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChanges { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeCheck> OrganisationEmployeeTypeOrderTypeChecks { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeCoefficient> OrganisationEmployeeTypeOrderTypeCoefficients { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeDescription> OrganisationEmployeeTypeOrderTypeDescriptions { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeSummaryCalc> OrganisationEmployeeTypeOrderTypeSummaryCalcs { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeOrderTypeWageItem> OrganisationEmployeeTypeOrderTypeWageItems { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeSettlementItem> OrganisationEmployeeTypeSettlementItems { get; set; }

    public virtual DbSet<OrganisationEmployeeTypeWageItem> OrganisationEmployeeTypeWageItems { get; set; }

    public virtual DbSet<OrganisationFicheItem> OrganisationFicheItems { get; set; }

    public virtual DbSet<OrganisationFormula> OrganisationFormulas { get; set; }

    public virtual DbSet<OrganisationFundType> OrganisationFundTypes { get; set; }

    public virtual DbSet<OrganisationInsJobList> OrganisationInsJobLists { get; set; }

    public virtual DbSet<OrganisationJob> OrganisationJobs { get; set; }

    public virtual DbSet<OrganisationJobCategory> OrganisationJobCategories { get; set; }

    public virtual DbSet<OrganisationJobGroup> OrganisationJobGroups { get; set; }

    public virtual DbSet<OrganisationJobSeries> OrganisationJobSeries { get; set; }

    public virtual DbSet<OrganisationJobSkillYearSetting> OrganisationJobSkillYearSettings { get; set; }

    public virtual DbSet<OrganisationLeave> OrganisationLeaves { get; set; }

    public virtual DbSet<OrganisationMrt> OrganisationMrts { get; set; }

    public virtual DbSet<OrganisationOrderType> OrganisationOrderTypes { get; set; }

    public virtual DbSet<OrganisationOrderTypeHistoryExclusion> OrganisationOrderTypeHistoryExclusions { get; set; }

    public virtual DbSet<OrganisationPeymanRow> OrganisationPeymanRows { get; set; }

    public virtual DbSet<OrganisationPosition> OrganisationPositions { get; set; }

    public virtual DbSet<OrganisationPositionJob> OrganisationPositionJobs { get; set; }

    public virtual DbSet<OrganisationPositionOccuptionMoreThanOneCach> OrganisationPositionOccuptionMoreThanOneCaches { get; set; }

    public virtual DbSet<OrganisationPositionSuggested> OrganisationPositionSuggesteds { get; set; }

    public virtual DbSet<OrganisationPositionSupervisor> OrganisationPositionSupervisors { get; set; }

    public virtual DbSet<OrganisationProject> OrganisationProjects { get; set; }

    public virtual DbSet<OrganisationSetting> OrganisationSettings { get; set; }

    public virtual DbSet<OrganisationSettlementCause> OrganisationSettlementCauses { get; set; }

    public virtual DbSet<OrganisationSettlementItem> OrganisationSettlementItems { get; set; }

    public virtual DbSet<OrganisationTempFile> OrganisationTempFiles { get; set; }

    public virtual DbSet<OrganisationWageItem> OrganisationWageItems { get; set; }

    public virtual DbSet<OrganisationWorkPlace> OrganisationWorkPlaces { get; set; }

    public virtual DbSet<OrganizationGoal> OrganizationGoals { get; set; }

    public virtual DbSet<OrganizationJobAbilityQualification> OrganizationJobAbilityQualifications { get; set; }

    public virtual DbSet<OrganizationJobCompetencyQualification> OrganizationJobCompetencyQualifications { get; set; }

    public virtual DbSet<OrganizationJobEducationFieldQualification> OrganizationJobEducationFieldQualifications { get; set; }

    public virtual DbSet<OrganizationJobEducationGradeQualification> OrganizationJobEducationGradeQualifications { get; set; }

    public virtual DbSet<OrganizationJobForeignLanguageQualification> OrganizationJobForeignLanguageQualifications { get; set; }

    public virtual DbSet<OrganizationJobInitialCourseQualification> OrganizationJobInitialCourseQualifications { get; set; }

    public virtual DbSet<OrganizationJobPerformanceEvaluationCriteriaDescription> OrganizationJobPerformanceEvaluationCriteriaDescriptions { get; set; }

    public virtual DbSet<OrganizationJobPeriodicTaskDescription> OrganizationJobPeriodicTaskDescriptions { get; set; }

    public virtual DbSet<OrganizationJobRequiredCharacterQualification> OrganizationJobRequiredCharacterQualifications { get; set; }

    public virtual DbSet<OrganizationJobRequiredSoftwaresQualification> OrganizationJobRequiredSoftwaresQualifications { get; set; }

    public virtual DbSet<OrganizationJobRiskAndFaultDescription> OrganizationJobRiskAndFaultDescriptions { get; set; }

    public virtual DbSet<OrganizationJobTaskDescription> OrganizationJobTaskDescriptions { get; set; }

    public virtual DbSet<OrganizationType> OrganizationTypes { get; set; }

    public virtual DbSet<OtherVeteran> OtherVeterans { get; set; }

    public virtual DbSet<PasswordChangeRateLimit> PasswordChangeRateLimits { get; set; }

    public virtual DbSet<PayLocationProgressReport> PayLocationProgressReports { get; set; }

    public virtual DbSet<PaymentPeriod> PaymentPeriods { get; set; }

    public virtual DbSet<PaymentPeriodEmployeeBonu> PaymentPeriodEmployeeBonus { get; set; }

    public virtual DbSet<PaymentType> PaymentTypes { get; set; }

    public virtual DbSet<Permission> Permissions { get; set; }

    public virtual DbSet<PermissionRoute> PermissionRoutes { get; set; }

    public virtual DbSet<PersonnelFicheItem> PersonnelFicheItems { get; set; }

    public virtual DbSet<PersonnelFunction> PersonnelFunctions { get; set; }

    public virtual DbSet<PersonnelFunctionExcelFile> PersonnelFunctionExcelFiles { get; set; }

    public virtual DbSet<PersonnelFunctionVisible> PersonnelFunctionVisibles { get; set; }

    public virtual DbSet<PersonnelLeave> PersonnelLeaves { get; set; }

    public virtual DbSet<PersonnelLoan> PersonnelLoans { get; set; }

    public virtual DbSet<PersonnelLoanPayment> PersonnelLoanPayments { get; set; }

    public virtual DbSet<PersonnelManagerList> PersonnelManagerLists { get; set; }

    public virtual DbSet<PersonnelPayment> PersonnelPayments { get; set; }

    public virtual DbSet<Place> Places { get; set; }

    public virtual DbSet<Position> Positions { get; set; }

    public virtual DbSet<PositionType> PositionTypes { get; set; }

    public virtual DbSet<Project> Projects { get; set; }

    public virtual DbSet<PunishmentEncourage> PunishmentEncourages { get; set; }

    public virtual DbSet<RecruitOrder> RecruitOrders { get; set; }

    public virtual DbSet<RefreshToken> RefreshTokens { get; set; }

    public virtual DbSet<RelatedOrganizationJobDescription> RelatedOrganizationJobDescriptions { get; set; }

    public virtual DbSet<ReportableEntity> ReportableEntities { get; set; }

    public virtual DbSet<ReportableField> ReportableFields { get; set; }

    public virtual DbSet<ReportMapColumn> ReportMapColumns { get; set; }

    public virtual DbSet<RequestDocumentRequirement> RequestDocumentRequirements { get; set; }

    public virtual DbSet<RequestDocumentRequirementDetail> RequestDocumentRequirementDetails { get; set; }

    public virtual DbSet<RoleClaim> RoleClaims { get; set; }

    public virtual DbSet<RoleMenu> RoleMenus { get; set; }

    public virtual DbSet<RoleReport> RoleReports { get; set; }

    public virtual DbSet<RoleReportableEntity> RoleReportableEntities { get; set; }

    public virtual DbSet<SecurityAuditLog> SecurityAuditLogs { get; set; }

    public virtual DbSet<SendedSm> SendedSms { get; set; }

    public virtual DbSet<Setting> Settings { get; set; }

    public virtual DbSet<SettlementCause> SettlementCauses { get; set; }

    public virtual DbSet<SettlementItem> SettlementItems { get; set; }

    public virtual DbSet<SettlementStatus> SettlementStatuses { get; set; }

    public virtual DbSet<SettlementDocumentAttachmentType> SettlementDocumentAttachmentTypes { get; set; }

    public virtual DbSet<SkillLevel> SkillLevels { get; set; }

    public virtual DbSet<StaffingRule> StaffingRules { get; set; }

    public virtual DbSet<StatusList> StatusLists { get; set; }

    public virtual DbSet<StatusListItem> StatusListItems { get; set; }

    public virtual DbSet<SystemGuide> SystemGuides { get; set; }

    public virtual DbSet<TaminInsuranceJobList> TaminInsuranceJobLists { get; set; }

    public virtual DbSet<TashkilatTempFile> TashkilatTempFiles { get; set; }

    public virtual DbSet<Tax> Taxes { get; set; }

    public virtual DbSet<TaxCoefficientItem> TaxCoefficientItems { get; set; }

    public virtual DbSet<TaxDiskette> TaxDiskettes { get; set; }

    public virtual DbSet<TaxDisketteCostCenter> TaxDisketteCostCenters { get; set; }

    public virtual DbSet<TaxDisketteFile> TaxDisketteFiles { get; set; }

    public virtual DbSet<TaxDisketteWh> TaxDisketteWhs { get; set; }

    public virtual DbSet<TaxDisketteWk> TaxDisketteWks { get; set; }

    public virtual DbSet<TaxDisketteWp> TaxDisketteWps { get; set; }

    public virtual DbSet<TaxExemptionType> TaxExemptionTypes { get; set; }

    public virtual DbSet<TaxNonCashPayment> TaxNonCashPayments { get; set; }

    public virtual DbSet<TaxOccupation> TaxOccupations { get; set; }

    public virtual DbSet<TaxTable> TaxTables { get; set; }

    public virtual DbSet<TaxableIncome> TaxableIncomes { get; set; }

    public virtual DbSet<TempEmployeeDeduction> TempEmployeeDeductions { get; set; }

    public virtual DbSet<TempGlobalFile> TempGlobalFiles { get; set; }

    public virtual DbSet<TempPersonnelFunction> TempPersonnelFunctions { get; set; }

    public virtual DbSet<TempPersonnelLeave> TempPersonnelLeaves { get; set; }

    public virtual DbSet<TempPunishmentEncourage> TempPunishmentEncourages { get; set; }

    public virtual DbSet<TestFull> TestFulls { get; set; }

    public virtual DbSet<University> Universities { get; set; }

    public virtual DbSet<UserClaim> UserClaims { get; set; }

    public virtual DbSet<UserCostCenter> UserCostCenters { get; set; }

    public virtual DbSet<UserDefaultSetting> UserDefaultSettings { get; set; }

    public virtual DbSet<UserFileUpload> UserFileUploads { get; set; }

    public virtual DbSet<UserIssueReport> UserIssueReports { get; set; }

    public virtual DbSet<UserLogin> UserLogins { get; set; }

    public virtual DbSet<UserLoginHistory> UserLoginHistories { get; set; }

    public virtual DbSet<UserMenu> UserMenus { get; set; }

    public virtual DbSet<UserOrganizationUnit> UserOrganizationUnits { get; set; }

    public virtual DbSet<UserPasswordHistory> UserPasswordHistories { get; set; }

    public virtual DbSet<UserPayLocation> UserPayLocations { get; set; }

    public virtual DbSet<UserReport> UserReports { get; set; }

    public virtual DbSet<UserReportableEntity> UserReportableEntities { get; set; }

    public virtual DbSet<UserRole> UserRoles { get; set; }

    public virtual DbSet<UserSignature> UserSignatures { get; set; }

    public virtual DbSet<UserToken> UserTokens { get; set; }

    public virtual DbSet<UserWorkPlace> UserWorkPlaces { get; set; }

    public virtual DbSet<Models.Version> Versions { get; set; }

    public virtual DbSet<VersionChangeLog> VersionChangeLogs { get; set; }

    public virtual DbSet<VwEmpWork> VwEmpWorks { get; set; }

    public virtual DbSet<VwInterdictOrder> VwInterdictOrders { get; set; }

    public virtual DbSet<VwOrgChartJob> VwOrgChartJobs { get; set; }

    public virtual DbSet<VwOrganisationChartTree> VwOrganisationChartTrees { get; set; }

    public virtual DbSet<WageItem> WageItems { get; set; }

    public virtual DbSet<War> Wars { get; set; }

    public virtual DbSet<Work> Works { get; set; }

    public virtual DbSet<WorkFlow> WorkFlows { get; set; }

    public virtual DbSet<WorkFlowInstance> WorkFlowInstances { get; set; }

    public virtual DbSet<WorkFlowType> WorkFlowTypes { get; set; }

    public virtual DbSet<Wwwwww> Wwwwwws { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    { 
    
    
    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.UseCollation("SQL_Latin1_General_CP1_CI_AS");

        modelBuilder.Entity<Ability>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Abilities).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AbsenceRecord>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.AbsenceTypeValue).WithMany(p => p.AbsenceRecords).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.AbsenceRecords).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AbsenceTypeValue>(entity =>
        {
            entity.HasOne(d => d.AbsenceType).WithMany(p => p.AbsenceTypeValues).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ActivityTemplate>(entity =>
        {
            entity.HasIndex(e => new { e.Pending, e.DoDate, e.WorkFlowInstanceId }, "IX_Activity_Template_Pending_DoDate_WorkFlowInstance").HasFilter("([Pending]=(1) AND [DoDate] IS NULL)");

            entity.HasIndex(e => new { e.ToNodeId, e.WorkFlowInstanceId }, "IX_Activity_Template_Pending_ToNodeId_Instance").HasFilter("([Pending]=(1) AND [DoDate] IS NULL)");

            entity.HasOne(d => d.Action).WithMany(p => p.ActivityTemplates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkFlowInstance).WithMany(p => p.ActivityTemplates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Appearance>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.SpecificSymptoms).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Appearances).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Arear>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.Arears).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.Arears).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArearFicheItem>(entity =>
        {
            entity.HasOne(d => d.WageItem).WithMany(p => p.ArearFicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArearsChangedFicheItem>(entity =>
        {
            entity.Property(e => e.CurrentAmount).HasComment("مبلغ جدید");
            entity.Property(e => e.LastAmount).HasComment("مبلغ فیش قبلی");

            entity.HasOne(d => d.WageItem).WithMany(p => p.ArearsChangedFicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ArearsStatus>(entity =>
        {
            entity.Property(e => e.Title).HasDefaultValue("");
        });

        modelBuilder.Entity<AspNetUser>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.Id }, "IX_AspNetUsers_EmployeeId_Id")
                .IsDescending(false, true)
                .HasFilter("([EmployeeId] IS NOT NULL)");

            entity.HasIndex(e => e.UserName, "IX_AspNetUsers_UserName")
                .IsUnique()
                .HasFilter("([UserName] IS NOT NULL)");
        });

        modelBuilder.Entity<Attendance>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.DeviceName).HasDefaultValue("");
            entity.Property(e => e.InOutCard).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Attendances).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AttendanceCalendar>(entity =>
        {
            entity.Property(e => e.Date).HasComment("تاریخ");
            entity.Property(e => e.HolidayId).HasComment("تعطیلات مرتبط");
            entity.Property(e => e.IsHoliday).HasComment("تعطیل");
            entity.Property(e => e.WeekDay).HasComment("روز هفته (مطابق DayOfWeek)");
        });

        modelBuilder.Entity<AttendanceDevice>(entity =>
        {
            entity.Property(e => e.BrandId).HasComment("base table value Id : 40300 (برند دستگاه)");
            entity.Property(e => e.Code).HasComment("کد دستگاه");
            entity.Property(e => e.DeviceIp).HasComment("آدرس IP دستگاه");
            entity.Property(e => e.DeviceType).HasComment("نوع دستگاه");
            entity.Property(e => e.LastSyncDate).HasComment("آخرین تاریخ همگام‌سازی");
            entity.Property(e => e.Model).HasComment("مدل دستگاه");
            entity.Property(e => e.Port).HasComment("پورت");
            entity.Property(e => e.SerialNumber).HasComment("شماره سریال");
            entity.Property(e => e.StatusId).HasComment("base table value Id : 40299 (وضعیت دستگاه)");
            entity.Property(e => e.SyncInterval).HasComment("فاصله همگام‌سازی (دقیقه)");
            entity.Property(e => e.TimeZone).HasComment("منطقه زمانی");

            entity.HasOne(d => d.AttendanceLocation).WithMany(p => p.AttendanceDevices).OnDelete(DeleteBehavior.ClientSetNull);

        });

        modelBuilder.Entity<AttendanceEmployeeShiftAssignment>(entity =>
        {
            entity.Property(e => e.Description).HasComment("توضیحات");

         
            entity.HasOne(d => d.Shift).WithMany(p => p.AttendanceEmployeeShiftAssignments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AttendanceHoliday>(entity =>
        {
        });

        modelBuilder.Entity<AttendanceLocation>(entity =>
        {
            entity.Property(e => e.Address).HasComment("آدرس");
            entity.Property(e => e.Code).HasComment("کد محل حضور");
            entity.Property(e => e.Latitude).HasComment("عرض جغرافیایی");
            entity.Property(e => e.Longitude).HasComment("طول جغرافیایی");
            entity.Property(e => e.Radius).HasComment("شعاع مجاز (متر)");

        });

        modelBuilder.Entity<AttendanceLog>(entity =>
        {
            entity.Property(e => e.DeviceUserId).HasComment("شناسه کاربر در دستگاه");
            entity.Property(e => e.Direction).HasComment("جهت تردد");
            entity.Property(e => e.LogDateTime).HasComment("زمان ثبت در دستگاه");
            entity.Property(e => e.Mask).HasComment("ماسک");
            entity.Property(e => e.RawData).HasComment("داده خام دستگاه");
            entity.Property(e => e.ReceiveDate).HasComment("زمان دریافت در سامانه");
            entity.Property(e => e.Status).HasComment("وضعیت پردازش");
            entity.Property(e => e.Temperature).HasComment("دمای اندازه‌گیری شده");
            entity.Property(e => e.VerifyMode).HasComment("نوع احراز هویت");
            entity.Property(e => e.WorkCode).HasComment("کد کار");
            entity.HasOne(d => d.AttendanceDevice).WithMany(p => p.AttendanceLogs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<AttendanceShift>(entity =>
        {
            entity.Property(e => e.Code).HasComment("کد شیفت");
            entity.Property(e => e.Color).HasComment("رنگ نمایشی (hex)");
            entity.Property(e => e.IsActive).HasComment("فعال");
        });

        modelBuilder.Entity<AttendanceShiftDetail>(entity =>
        {
            entity.Property(e => e.WeekDay).HasComment("روز هفته (مطابق DayOfWeek)");
            entity.Property(e => e.CrossDay).HasComment("عبور از نیمه‌شب");
            entity.Property(e => e.EndTime).HasComment("ساعت پایان کار");
            entity.Property(e => e.IsFlexible).HasComment("شیفت منعطف");
            entity.Property(e => e.MaxInTime).HasComment("حداکثر زمان ورود");
            entity.Property(e => e.MaxOutTime).HasComment("حداکثر زمان خروج");
            entity.Property(e => e.MinInTime).HasComment("حداقل زمان ورود");
            entity.Property(e => e.MinOutTime).HasComment("حداقل زمان خروج");
            entity.Property(e => e.NightShift).HasComment("شیفت شب");
            entity.Property(e => e.RequiredWorkSeconds).HasComment("مدت کار مورد نیاز (ثانیه)");
            entity.Property(e => e.RestEnd).HasComment("پایان استراحت");
            entity.Property(e => e.RestStart).HasComment("شروع استراحت");
            entity.Property(e => e.RoundType).HasComment("نوع گرد کردن زمان");
            entity.Property(e => e.StartTime).HasComment("ساعت شروع کار");

            entity.HasOne(d => d.Shift).WithMany(p => p.AttendanceShiftDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankAccount>(entity =>
        {
            entity.Property(e => e.AccountNumber).HasDefaultValue("");
            entity.Property(e => e.BankBranchId).HasDefaultValue(0);
            entity.Property(e => e.BonCardNumber).HasDefaultValue("");
            entity.Property(e => e.CardNumber).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.FromPrice).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.OldId).HasDefaultValue(0);
            entity.Property(e => e.Priority).HasDefaultValue(0);
            entity.Property(e => e.ShabaNumber).HasDefaultValue("");
            entity.Property(e => e.ToPrice).HasDefaultValue(0);

            entity.HasOne(d => d.Employee).WithMany(p => p.BankAccounts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankBranch>(entity =>
        {
            entity.HasOne(d => d.Bank).WithMany(p => p.BankBranches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDiskette>(entity =>
        {
            entity.HasIndex(e => new { e.PaymentPeriodId, e.BatchPayRollRequestId, e.IsDeleted }, "IX_Bank_Diskette_PaymentPeriodId_BatchPayRollRequestId_IsDeleted")
                .IsUnique()
                .HasFilter("([BatchPayRollRequestId] IS NOT NULL)");

            entity.Property(e => e.CalculateAllFichesInCurrentPeriod).HasComment("������ ��Ә� ���� ����� ��ǘ� ����� �� �� ���� ���� ��� ����� ����� ���");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BankDiskettes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.BankDiskettes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDisketteCostCenter>(entity =>
        {
            entity.HasOne(d => d.BankDiskette).WithMany(p => p.BankDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CostCenter).WithMany(p => p.BankDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDisketteGroupAndFile>(entity =>
        {
            entity.HasOne(d => d.BankDiskette).WithMany(p => p.BankDisketteGroupAndFiles).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.BankDisketteTemplate).WithMany(p => p.BankDisketteGroupAndFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDisketteItem>(entity =>
        {
            entity.HasOne(d => d.BankDiskette).WithMany(p => p.BankDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.BankDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDisketteTemplate>(entity =>
        {
            entity.HasOne(d => d.Bank).WithMany(p => p.BankDisketteTemplates).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BankDisketteTemplates).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BankDisketteTemplateRow>(entity =>
        {
            entity.HasOne(d => d.BankDisketteTemplate).WithMany(p => p.BankDisketteTemplateRows).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BaseTableValue>(entity =>
        {
            entity.HasOne(d => d.BaseTable).WithMany(p => p.BaseTableValues).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Basij>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.DurationDay).HasDefaultValue(0);
            entity.Property(e => e.DurationMonth).HasDefaultValue(0);
            entity.Property(e => e.DurationYear).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsContinues).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.TrackingCode).HasDefaultValue("");
            entity.Property(e => e.Year).HasDefaultValue(0);
            entity.Property(e => e.YearCoefficient).HasDefaultValue(0);

            entity.HasOne(d => d.Employee).WithMany(p => p.Basijs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BasijGrade>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.BasijGrades).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchLog>(entity =>
        {
            entity.ToTable("Batch_Log", "Payroll", tb => tb.HasTrigger("trg_LimitLogs"));
        });

        modelBuilder.Entity<BatchPayRollRequest>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BatchPayRollRequests).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.BatchPayRollRequests).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchPayRollRequestDetail>(entity =>
        {
            entity.HasIndex(e => new { e.BatchPayRollRequestId, e.FicheId }, "IX_Batch_PayRoll_Request_Detail_Batch_Fiche").HasFilter("([FicheId] IS NOT NULL)");

            entity.HasOne(d => d.BatchPayRollRequest).WithMany(p => p.BatchPayRollRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.BatchPayRollRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchRequest>(entity =>
        {
            entity.HasOne(d => d.OrderType).WithMany(p => p.BatchRequests).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BatchRequests).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchRequestDetail>(entity =>
        {
            entity.HasOne(d => d.BatchRequest).WithMany(p => p.BatchRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.BatchRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchRequestDetailReference>(entity =>
        {
            entity.HasOne(d => d.BatchRequestDetail).WithMany(p => p.BatchRequestDetailReferences).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BatchRequestFile>(entity =>
        {
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.BatchRequest).WithMany(p => p.BatchRequestFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Bill>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.Bills).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BillDetail>(entity =>
        {
            entity.HasOne(d => d.Bill).WithMany(p => p.BillDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BillDetailException>(entity =>
        {
            entity.HasOne(d => d.BillDetail).WithMany(p => p.BillDetailExceptions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CostCenter).WithMany(p => p.BillDetailExceptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BillInstance>(entity =>
        {
            entity.HasOne(d => d.Bill).WithMany(p => p.BillInstances).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.BuyerCostCenter).WithMany(p => p.BillInstanceBuyerCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.BillInstances).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SellerCostCenter).WithMany(p => p.BillInstanceSellerCostCenters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BillItem>(entity =>
        {
            entity.HasOne(d => d.Bill).WithMany(p => p.BillItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.BillItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BlackList>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.BlackLists).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BlackLists).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<BlockedAccount>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.BlockedAccounts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.BlockedAccounts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CalclulationSetting>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithOne(p => p.CalclulationSetting).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Captivity>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Day).HasDefaultValue(0);
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsContinues).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.Month).HasDefaultValue(0);
            entity.Property(e => e.SacrificePercent).HasDefaultValue(0.0);
            entity.Property(e => e.TrackingCode).HasDefaultValue("");
            entity.Property(e => e.Year).HasDefaultValue(0);

            entity.HasOne(d => d.Employee).WithMany(p => p.Captivities).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Character>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Characters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Coefficient1>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Value).HasDefaultValue(0m);

            entity.HasOne(d => d.Employee).WithMany(p => p.Coefficient1s).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Competency>(entity =>
        {
            entity.Property(e => e.Acceptable).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Competencies).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ContactInfo>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.IsLast, e.IsDeleted }, "IX_Contact_Info_EmployeeId_IsLast_IsDeleted").HasFilter("([IsLast]=(1) AND [IsDeleted]=(0))");

            entity.Property(e => e.Address).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.EmergencyPhone).HasDefaultValue("");
            entity.Property(e => e.Fax).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Mail).HasDefaultValue("");
            entity.Property(e => e.MobileNo).HasDefaultValue("");
            entity.Property(e => e.Phone).HasDefaultValue("");
            entity.Property(e => e.Zipcode).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.ContactInfos).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<CostCenterFicheItem>(entity =>
        {
            entity.HasOne(d => d.CostCenter).WithMany(p => p.CostCenterFicheItemCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.CostCenterFicheItemOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.CostCenterFicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Course>(entity =>
        {
            entity.Property(e => e.CourseMark).HasDefaultValue("");
            entity.Property(e => e.CourseSerial).HasDefaultValue("");
            entity.Property(e => e.CourseSession).HasDefaultValue(0);
            entity.Property(e => e.CourseTime).HasDefaultValue(0);
            entity.Property(e => e.CoursepPlace).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Courses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DeductedArear>(entity =>
        {
            entity.HasOne(d => d.Arear).WithMany(p => p.DeductedArears).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StartDeductedPaymentPeriod).WithMany(p => p.DeductedArears).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.DeductedArears).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DeductedArearsDetail>(entity =>
        {
            entity.HasOne(d => d.DeductedArears).WithMany(p => p.DeductedArearsDetails).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.DeductedArearsDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DeductionType>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.DeductionTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.DeductionTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Definition>(entity =>
        {
            entity.HasOne(d => d.Action).WithMany(p => p.Definitions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Definitions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Disability>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.DisabilityPercent).HasDefaultValue(0);
            entity.Property(e => e.HasCertification).HasDefaultValue(false);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Disabilities).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DrivingLicense>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Licencedescription).HasDefaultValue("");
            entity.Property(e => e.LicenseSerialNumber).HasDefaultValue("");
            entity.Property(e => e.PreviousDerivingNumber).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.DrivingLicenses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DynamicReport>(entity =>
        {
            entity.Property(e => e.ExportTypeId).HasComment("base table value Id : 40286 (excel or pdf)");

            entity.HasOne(d => d.ExportType).WithMany(p => p.DynamicReportExportTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FuctionType).WithMany(p => p.DynamicReportFuctionTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.DynamicReports).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<DynamicReportParameter>(entity =>
        {
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.DynamicReport).WithMany(p => p.DynamicReportParameters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Parameter).WithMany(p => p.DynamicReportParameters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Education>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.EducationAverage).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsBoursie).HasDefaultValue(false);
            entity.Property(e => e.IsDefaultEducation).HasDefaultValue(false);
            entity.Property(e => e.IsInDutyTime).HasDefaultValue(false);
            entity.Property(e => e.IsUsedInOrder).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LicenceNumber).HasDefaultValue("");
            entity.Property(e => e.OtherUniversityName).HasDefaultValue("");
            entity.Property(e => e.SetByEmployee).HasDefaultValue(false);
            entity.Property(e => e.ThesisTitle).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Educations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Models.Employee>(entity =>
        {
            entity.HasIndex(e => e.IdentityNo, "IX_Employee_IdentityNo").HasFilter("([IdentityNo] IS NOT NULL)");

            entity.HasIndex(e => e.NationalNo, "IX_Employee_NationalNo").HasFilter("([NationalNo] IS NOT NULL)");

            entity.HasIndex(e => e.PersonelCode, "IX_Employee_PersonelCode").HasFilter("([PersonelCode] IS NOT NULL)");

            entity.HasIndex(e => new { e.PhoneNumber, e.IsDeleted }, "IX_Employee_PhoneNumber_IsDeleted").HasFilter("([PhoneNumber] IS NOT NULL)");

            entity.Property(e => e.AccountingSystemEmployeeId).HasDefaultValue("");
            entity.Property(e => e.ActiveName).HasDefaultValue("");
            entity.Property(e => e.ConcurrencyStamp).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.FatherName).HasDefaultValue("");
            entity.Property(e => e.FirstName).HasDefaultValue("");
            entity.Property(e => e.IdentityNo).HasDefaultValue("");
            entity.Property(e => e.Imperfective).HasDefaultValue(0);
            entity.Property(e => e.InOutCard).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsCashBenefits).HasDefaultValue(false);
            entity.Property(e => e.IsHekmat).HasDefaultValue(false);
            entity.Property(e => e.IsWelfareBenefits).HasDefaultValue(false);
            entity.Property(e => e.IssueSerialChar).HasDefaultValue(0);
            entity.Property(e => e.IssueSerialOrder).HasDefaultValue("");
            entity.Property(e => e.IssueSerialString).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LastName).HasDefaultValue("");
            entity.Property(e => e.LostIssueSerialString).HasDefaultValue("");
            entity.Property(e => e.MartyrChildTrackingCode).HasDefaultValue("");
            entity.Property(e => e.NationalNo).HasDefaultValue("");
            entity.Property(e => e.PassportNo).HasDefaultValue("");
            entity.Property(e => e.PasswordHash).HasDefaultValue("");
            entity.Property(e => e.PersonelCode).HasDefaultValue("");
            entity.Property(e => e.PhoneNumber).HasDefaultValue("");
            entity.Property(e => e.PrivateJobStatus).HasDefaultValue(0);
            entity.Property(e => e.ReleaseReason).HasDefaultValue("");
            entity.Property(e => e.SectId).HasDefaultValue(0);
            entity.Property(e => e.SecurityStamp).HasDefaultValue("");
        });

        modelBuilder.Entity<EmployeeDeduction>(entity =>
        {
            entity.HasOne(d => d.DeductionType).WithMany(p => p.EmployeeDeductions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeDeductions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.EmployeeDeductions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StartDeductPaymentPeriod).WithMany(p => p.EmployeeDeductions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeDeductionPayment>(entity =>
        {
            entity.HasOne(d => d.EmployeeDeduction).WithMany(p => p.EmployeeDeductionPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Fiche).WithMany(p => p.EmployeeDeductionPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.EmployeeDeductionPayments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeDeductionUploadBatch>(entity =>
        {
        });

        modelBuilder.Entity<EmployeeFile>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Name).HasDefaultValue("");
            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeFiles).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.EmployeeFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeFund>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeFunds).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundType).WithMany(p => p.EmployeeFunds).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.EmployeeFunds).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StartDeductPaymentPeriod).WithMany(p => p.EmployeeFunds).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeLeaveEntitlement>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeLeaveEntitlements).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.EmployeeLeaveEntitlements).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.EmployeeLeaveEntitlements).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeLoginHistory>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
        });

        modelBuilder.Entity<EmployeeOtp>(entity =>
        {
            entity.Property(e => e.CodeHash).HasDefaultValue("");
            entity.Property(e => e.CreatedByIp).HasDefaultValue("");
            entity.Property(e => e.Purpose).HasDefaultValue("");

        });

        modelBuilder.Entity<EmployeeProperty>(entity =>
        {
            entity.HasIndex(e => e.NationalNo, "UX_Employee_Property_NationalNo")
                .IsUnique()
                .HasFilter("([NationalNo] IS NOT NULL AND [NationalNo]<>N'')");
        });

        modelBuilder.Entity<EmployeeRefreshToken>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.CreatedAt }, "IX_EmployeeRefreshToken_EmployeeId_CreatedAt")
                .IsDescending(false, true)
                .HasFilter("([RevokedAt] IS NULL)");

            entity.Property(e => e.CreatedByIp).HasDefaultValue("");
            entity.Property(e => e.ReplacedByToken).HasDefaultValue("");
            entity.Property(e => e.RevocationReason).HasDefaultValue("");
            entity.Property(e => e.RevokedByIp).HasDefaultValue("");
            entity.Property(e => e.Token).HasDefaultValue("");

        });

        modelBuilder.Entity<EmployeeRequest>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.EmployeeRequestStatus).WithMany(p => p.EmployeeRequests).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RequestDocumentRequirement).WithMany(p => p.EmployeeRequests).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeRequestDetail>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.EmployeeRequest).WithMany(p => p.EmployeeRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RequestDocumentRequirementDetail).WithMany(p => p.EmployeeRequestDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeSettlement>(entity =>
        {
            entity.Property(e => e.SettlementDate).HasComment("تاریخ تنظیم و امضای فرم تسویه حساب");

        });

        modelBuilder.Entity<EmployeeSettlementItem>(entity =>
        {
            entity.HasOne(d => d.EmployeeSettlement).WithMany(p => p.EmployeeSettlementItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EmployeeSoftware>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.EmployeeSoftwares).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<EvaluationResult>(entity =>
        {
            entity.Property(e => e.Average).HasDefaultValue(0m);
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.EvaluationCoefficent).HasDefaultValue((byte)0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.YearCoefficent).HasDefaultValue(0);

            entity.HasOne(d => d.Employee).WithMany(p => p.EvaluationResults).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Experience>(entity =>
        {
            entity.Property(e => e.AcceptablePercent).HasDefaultValue(0);
            entity.Property(e => e.CompanyTitle).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Duration).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Experiences).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.Experiences).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Family>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.NationalNo, e.IsDeleted }, "IX_Family_EmployeeId_NationalNo_IsDeleted")
                .IsUnique()
                .HasFilter("([NationalNo] IS NOT NULL)");

            entity.Property(e => e.AccountNumber).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.DisabilityPercent).HasDefaultValue(0);
            entity.Property(e => e.EffectivePercent).HasDefaultValue(0m);
            entity.Property(e => e.FatherName).HasDefaultValue("");
            entity.Property(e => e.FirstName).HasDefaultValue("");
            entity.Property(e => e.HasCertification).HasDefaultValue(false);
            entity.Property(e => e.IdentityNo).HasDefaultValue("");
            entity.Property(e => e.InsuranceNumber).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsCashBenefits).HasDefaultValue(false);
            entity.Property(e => e.IsCoveredInsurance).HasDefaultValue(false);
            entity.Property(e => e.IsDependent).HasDefaultValue(false);
            entity.Property(e => e.IsHekmat).HasDefaultValue(false);
            entity.Property(e => e.IsImperfective).HasDefaultValue(false);
            entity.Property(e => e.IsLast).HasDefaultValue(false);
            entity.Property(e => e.IsPremierStudent).HasDefaultValue(false);
            entity.Property(e => e.IsWelfareServices).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LastName).HasDefaultValue("");
            entity.Property(e => e.MaintenanceCost).HasDefaultValue(0f);
            entity.Property(e => e.NationalNo).HasDefaultValue("");
            entity.Property(e => e.UsedinOrder).HasDefaultValue(false);

            entity.HasOne(d => d.Employee).WithMany(p => p.Families).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Fiche>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.PaymentPeriodId, e.IsDeleted }, "IX_Fiche_EmployeeId_PaymentPeriodId_IsDeleted").HasFillFactor(90);

            entity.HasIndex(e => new { e.EmployeeId, e.PaymentPeriodId, e.IsDeleted }, "IX_Fiche_Employee_PaymentPeriod_IsDeleted")
                .IsUnique()
                .HasFilter("([EmployeeId] IS NOT NULL AND [PaymentPeriodId] IS NOT NULL AND [IsDeleted] IS NOT NULL)");

            entity.HasOne(d => d.CostCenter).WithMany(p => p.FicheCostCenters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_CostCenter_CostCenterId");

            entity.HasOne(d => d.Employee).WithMany(p => p.Fiches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.Fiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_EmployeeType_EmployeeTypeId");

            entity.HasOne(d => d.FicheStatus).WithMany(p => p.Fiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_FicheStatus_FicheStatusId");

            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.Fiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_InterdictOrder_InterdictOrderId");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.FicheOrganisationCharts)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_OrganisationChart_OrganisationChartId");

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.Fiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_PaymentPeriod_PaymentPeriodId");

            entity.HasOne(d => d.PersonnelFunction).WithMany(p => p.Fiches)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Fiche_PersonnelFunction_PersonnelFunctionId");

            entity.HasOne(d => d.PeymanRow).WithMany(p => p.Fiches).HasConstraintName("FK_Fiche_PeymanRow_PeymanRowId");
        });

        modelBuilder.Entity<FicheItem>(entity =>
        {
            entity.HasIndex(e => new { e.FicheId, e.IsDeleted }, "IX_Fiche_Item_FicheId_IsDeleted").HasFillFactor(90);

            entity.Property(e => e.IsArear).HasComment(" آیا این قلم منشا معوقه دارد ؟");

            entity.HasOne(d => d.Fiche).WithMany(p => p.FicheItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.FicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FicheLeaveItem>(entity =>
        {
            entity.HasIndex(e => new { e.FicheId, e.IsDeleted }, "IX_Fiche_Leave_Item_FicheId_IsDeleted").HasFillFactor(90);

            entity.HasOne(d => d.Fiche).WithMany(p => p.FicheLeaveItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.FicheLeaveItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PersonnelLeave).WithMany(p => p.FicheLeaveItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FichePdfArchive>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.FichePdfArchives).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.FichePdfArchives).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FicheReportArchive>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.FicheReportArchives).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.FicheReportArchives).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FieldOperator>(entity =>
        {
            entity.HasOne(d => d.FieldDataType).WithMany(p => p.FieldOperators).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<File1>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.MimeType).HasDefaultValue("");
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.File1s).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ForeignLanguage>(entity =>
        {
            entity.Property(e => e.Acceptable).HasDefaultValue(false);
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.Languagescore).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.OtherLanguageName).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.ForeignLanguages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ForeignTravel>(entity =>
        {
            entity.Property(e => e.ArchiveId).HasDefaultValue(0);
            entity.Property(e => e.CountryCount).HasDefaultValue(0);
            entity.Property(e => e.CountryList).HasDefaultValue("");
            entity.Property(e => e.CountryNames).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.MissionCost).HasDefaultValue(0);
            entity.Property(e => e.MissionSubject).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.ForeignTravels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormulaDatabaseFunctionDefinition>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.FormulaDatabaseFunctionDefinitions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormulaDefinition>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
            entity.Property(e => e.Description).HasDefaultValue("");

            entity.HasOne(d => d.IdNavigation).WithOne(p => p.FormulaDefinition).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormulaDefinitionHistory>(entity =>
        {
            entity.HasOne(d => d.FormulaDefinition).WithMany(p => p.FormulaDefinitionHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormulaTable>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.FormulaTables).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FormulaTableValue>(entity =>
        {
            entity.HasOne(d => d.FormulaTable).WithMany(p => p.FormulaTableValues).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<FunctionExcelDefinition>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.FunctionExcelDefinitions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ExcelDefinitionType).WithMany(p => p.FunctionExcelDefinitions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.FunctionExcelDefinitions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<G20ScoreDomainJobDegree>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.G20ScoreDomainJobDegrees).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<GroupPunishmentEncourage>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.EmPloyeeCount).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LastModifiedUser).HasDefaultValue("");
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.AgentOfPunishmentEncourage).WithMany(p => p.GroupPunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationAgentOfPunishmentEncourageScoreInterval).WithMany(p => p.GroupPunishmentEncourages).HasConstraintName("FK_Group_Punishment_Encourage_Organisation_Agent_Of_Punishment_Encourage_Score_Interval_OrganisationAgentOfPunishmentEncourageS~");
        });

        modelBuilder.Entity<GroupPunishmentEncourageFile>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Extension).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.MimeType).HasDefaultValue("");
            entity.Property(e => e.Title).HasDefaultValue("");
        });

        modelBuilder.Entity<HistoryStop>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.HistoryStopDays).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsComputable).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.HistoryStops).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Image>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Images).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Insurance>(entity =>
        {
            entity.Property(e => e.AccDay).HasDefaultValue(0);
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.HasSupplementaryInsurance).HasDefaultValue(false);
            entity.Property(e => e.InsuranceNumber).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsLast).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Insurances).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InsuranceBranch>(entity =>
        {
            entity.HasOne(d => d.InsuranceType).WithMany(p => p.InsuranceBranches).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.InsuranceBranches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InsuranceDetail>(entity =>
        {
            entity.Property(e => e.AccDay).HasDefaultValue(0);
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Desc).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsComputable).HasDefaultValue(false);
            entity.Property(e => e.IsFullInsurnce).HasDefaultValue(false);
            entity.Property(e => e.IsOptionalInsurnce).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.Month).HasDefaultValue(0);
            entity.Property(e => e.Year).HasDefaultValue(0);

            entity.HasOne(d => d.Insurance).WithMany(p => p.InsuranceDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InsuranceDiskette>(entity =>
        {
            entity.HasIndex(e => new { e.PaymentPeriodId, e.BatchPayRollRequestId, e.IsDeleted }, "IX_Insurance_Diskette_PaymentPeriodId_BatchPayRollRequestId_IsDeleted")
                .IsUnique()
                .HasFilter("([BatchPayRollRequestId] IS NOT NULL)");

            entity.Property(e => e.PeymanRowId).HasComment("ردیف پیمان متناظر");
            entity.Property(e => e.ReportTypeId).HasComment("basetable value Id = 40282");
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.BatchPayRollRequest).WithMany(p => p.InsuranceDiskettes).HasConstraintName("FK_InsDisk_BatchRequest");

            entity.HasOne(d => d.InsuranceBranch).WithMany(p => p.InsuranceDiskettes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_InsuranceBranchId");

            entity.HasOne(d => d.InsuranceDisketteStatus).WithMany(p => p.InsuranceDisketteInsuranceDisketteStatuses)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_Status");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.InsuranceDiskettes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_OrganisationChartId");

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.InsuranceDiskettes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_PaymentPeriodId");

            entity.HasOne(d => d.PeymanRow).WithMany(p => p.InsuranceDiskettes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_PeymanRow");

            entity.HasOne(d => d.ReportType).WithMany(p => p.InsuranceDisketteReportTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_InsDisk_ReportType");
        });

        modelBuilder.Entity<InsuranceDisketteCostCenter>(entity =>
        {
            entity.HasOne(d => d.CostCenter).WithMany(p => p.InsuranceDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InsuranceDiskette).WithMany(p => p.InsuranceDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InsuranceDisketteFile>(entity =>
        {
            entity.HasOne(d => d.InsuranceDiskette).WithMany(p => p.InsuranceDisketteFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InsuranceDisketteItem>(entity =>
        {
            entity.HasOne(d => d.CostCenter).WithMany(p => p.InsuranceDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.InsuranceDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InsuranceDiskette).WithMany(p => p.InsuranceDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.InsuranceDisketteItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InterdictOrder>(entity =>
        {
            entity.HasIndex(e => e.Code, "IX_InterdictOrder_Code").HasFilter("([Code] IS NOT NULL)");

            entity.HasIndex(e => e.UniqueId, "IX_InterdictOrder_UniqueId").HasFilter("([UniqueId] IS NOT NULL)");

            entity.HasIndex(e => new { e.RecruitOrderId, e.Serial, e.Id }, "IX_Interdict_Order_Fiche_LastLookup")
                .IsDescending(false, true, true)
                .HasFilter("([IsDeleted]<>(1) AND [PayRollAprove]=(1))");

            entity.HasIndex(e => new { e.RecruitOrderId, e.Serial, e.Id }, "IX_Interdict_Order_Order_LastLookup")
                .IsDescending(false, true, true)
                .HasFilter("([IsDeleted]<>(1) AND ([StatusId] IN ((7), (8), (9), (11))))");

            entity.HasIndex(e => new { e.RecruitOrderId, e.StatusId }, "IX_Interdict_Order_RecruitOrderId_Status9").HasFilter("([StatusId]=(9))");

            entity.HasIndex(e => new { e.StatusId, e.RecruitOrderId }, "IX_Interdict_Order_Status9_RecruitOrderId").HasFilter("([StatusId]=(9))");

            entity.HasIndex(e => e.StatusId, "IX_Interdict_Order_StatusId_INCLUDE_RecruitOrderId").HasFillFactor(90);

            entity.HasIndex(e => new { e.StatusId, e.IsDeleted, e.RecruitOrderId }, "IX_Interdict_Order_StatusId_IsDeleted_RecruitOrderId").HasFilter("([IsDeleted]=(0))");

            entity.Property(e => e.ApproveTimePaymentPeriod).HasComment(" ��� ���� ��� ������ ����� ������� �� ���");
            entity.Property(e => e.IsArrears).HasComment(" ��� ͘� �� ���� ����� �� ���� � ������ ����� ����� ���� ��� �");

            entity.HasOne(d => d.AspNetUsers).WithMany(p => p.InterdictOrders).HasConstraintName("FK_Interdict_Order_AspNetUsers");

            entity.HasOne(d => d.BirthPlace).WithMany(p => p.InterdictOrderBirthPlaces).HasConstraintName("FK_Interdict_Order_BirthPlace");

            entity.HasOne(d => d.IssuePlace).WithMany(p => p.InterdictOrderIssuePlaces).HasConstraintName("FK_Interdict_Order_IssuePlace");

            entity.HasOne(d => d.OrderType).WithMany(p => p.InterdictOrders).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RecruitOrder).WithMany(p => p.InterdictOrders).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Status).WithMany(p => p.InterdictOrders).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InterdictOrderArchive>(entity =>
        {
        });

        modelBuilder.Entity<InterdictOrderCoefficientItem>(entity =>
        {
            entity.HasIndex(e => new { e.InterdictOrderId, e.IsDeleted }, "IX_Interdict_Order_CoefficientItem_InterdictOrderId_IsDeleted").HasFillFactor(90);

            entity.HasOne(d => d.Coefficient).WithMany(p => p.InterdictOrderCoefficientItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.InterdictOrderCoefficientItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InterdictOrderCopy>(entity =>
        {
            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.InterdictOrderCopies).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InterdictOrderPromissory>(entity =>
        {
            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.InterdictOrderPromissories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<InterdictOrderWageItem>(entity =>
        {
            entity.HasIndex(e => new { e.InterdictOrderId, e.IsDeleted }, "IX_Interdict_Order_WageItem_InterdictOrderId_IsDeleted").HasFillFactor(90);

            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.InterdictOrderWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.InterdictOrderWageItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Isar>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsContinues).HasDefaultValue(false);
            entity.Property(e => e.IsarDurationDay).HasDefaultValue(0);
            entity.Property(e => e.IsarDurationMonth).HasDefaultValue(0);
            entity.Property(e => e.IsarDurationYear).HasDefaultValue(0);
            entity.Property(e => e.IsarInability).HasDefaultValue("");
            entity.Property(e => e.IsarInjuerdOrgan).HasDefaultValue("");
            entity.Property(e => e.Isarpercent).HasDefaultValue(0f);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.TrackingCode).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Isars).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<LoanType>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.LoanTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.LoanTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Log>(entity =>
        {
            entity.ToTable(tb => tb.HasTrigger("trg_LimitLogs"));
        });

        modelBuilder.Entity<Message>(entity =>
        {
            entity.HasOne(d => d.Receiver).WithMany(p => p.MessageReceivers).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Sender).WithMany(p => p.MessageSenders).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MilitaryService>(entity =>
        {
            entity.Property(e => e.ConfirmedLetterNo).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsComputable).HasDefaultValue(false);
            entity.Property(e => e.IsContinue).HasDefaultValue(false);
            entity.Property(e => e.IsLast).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.MilitariSerialNo).HasDefaultValue("");
            entity.Property(e => e.MilitaryDuration).HasDefaultValue("");
            entity.Property(e => e.MilitaryFullDuration).HasDefaultValue("");
            entity.Property(e => e.MilitaryMinDuration).HasDefaultValue("");
            entity.Property(e => e.NameOfPeriod).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.MilitaryServices).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<MinimumMonthlyWage>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.MinimumMonthlyWages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Node>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.Nodes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.Nodes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Node_WorkFlow");
        });

        modelBuilder.Entity<NodeRoleRel>(entity =>
        {

            entity.HasOne(d => d.Role).WithMany(p => p.NodeRoleRels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.NodeRoleRels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<NodeUserRel>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.NodeUserRels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Node).WithMany(p => p.NodeUserRels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.NodeUserRels).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkFlow).WithMany(p => p.NodeUserRels).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrderTempFile>(entity =>
        {
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrderTempFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganProperty>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganProperties).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationAgentOfPunishmentEncourage>(entity =>
        {
            entity.HasOne(d => d.AgentOfPunishmentEncourageGroup).WithMany(p => p.OrganisationAgentOfPunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.AgentOfPunishmentEncourage).WithMany(p => p.OrganisationAgentOfPunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationAgentOfPunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationAgentOfPunishmentEncourageScoreInterval>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationAgentOfPunishmentEncourageScoreIntervals).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationChart>(entity =>
        {
            entity.HasIndex(e => e.ParentOrganisationChartId, "IX_Organisation_Chart_Parent_Active_Tree").HasFilter("([IsDeleted]=(0))");

            entity.HasIndex(e => e.IsRoot, "IX_Organisation_Chart_Root_Active").HasFilter("([IsDeleted]=(0) AND [IsRoot]=(1))");

            entity.Property(e => e.Code).HasDefaultValue("", "DF_Organisation_Chart_Code");
            entity.Property(e => e.CreatedBy).HasDefaultValue("", "DF_Organisation_Chart_CreatedBy");
            entity.Property(e => e.Ipaddress).HasDefaultValue("", "DF_Organisation_Chart_IPAddress");
            entity.Property(e => e.IsCostCenter).HasDefaultValue(false, "DF_Organisation_Chart_IsCostCenter");
            entity.Property(e => e.IsIndependentOrg).HasDefaultValue(false, "DF_Organisation_Chart_IsIndependentOrg");
            entity.Property(e => e.IsOrg).HasDefaultValue(false, "DF_Organisation_Chart_IsOrg");
            entity.Property(e => e.IsRegister).HasDefaultValue(false, "DF_Organisation_Chart_IsRegister");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("", "DF_Organisation_Chart_LastModifiedBy");
            entity.Property(e => e.LetterCode).HasDefaultValue("", "DF_Organisation_Chart_LetterCode");
            entity.Property(e => e.Order).HasDefaultValue(0, "DF_Organisation_Chart_Order");
            entity.Property(e => e.Rank).HasDefaultValue("", "DF_Organisation_Chart_Rank");
            entity.Property(e => e.SystemCode).HasDefaultValue("", "DF_Organisation_Chart_SystemCode");
            entity.Property(e => e.Title).HasDefaultValue("", "DF_Organisation_Chart_title");
        });

        modelBuilder.Entity<OrganisationChartImage>(entity =>
        {
            entity.Property(e => e.Content).HasDefaultValueSql("(0x)");
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationChartImages).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationCoefficient>(entity =>
        {
            entity.HasOne(d => d.Coefficient).WithMany(p => p.OrganisationCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationCoefficients).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationCostCenter>(entity =>
        {
            entity.Property(e => e.CostCenterFinancialCode).HasComment("کد مالی مربوط به مرکز هزینه");
            entity.Property(e => e.CostCenterPercent).HasComment("در صد حق بالاسری مرکز هزینه");
            entity.Property(e => e.PeymanRowId).HasComment("ردیف پیمان متناظر");

            entity.HasOne(d => d.CostCenter).WithMany(p => p.OrganisationCostCenterCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationCostCenterOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeStatus>(entity =>
        {
            entity.HasOne(d => d.EmployeeStatusGroup).WithMany(p => p.OrganisationEmployeeStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeStatus).WithMany(p => p.OrganisationEmployeeStatuses).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeStatuses).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeType>(entity =>
        {
            entity.HasOne(d => d.EmployeeTypeGroup).WithMany(p => p.OrganisationEmployeeTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeCoefficient>(entity =>
        {
            entity.HasOne(d => d.Coefficient).WithMany(p => p.OrganisationEmployeeTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeCoefficientBonusWageItem>(entity =>
        {
            entity.HasOne(d => d.Coefficient).WithMany(p => p.OrganisationEmployeeTypeCoefficientBonusWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeCoefficientBonusWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeCoefficientBonusWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationEmployeeTypeCoefficientBonusWageItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeFicheItem>(entity =>
        {
            entity.Property(e => e.IsAnnualEydi).HasComment("عیدی سالانه- ریالی");
            entity.Property(e => e.IsBonusExceptYearEndServiceEndProductivity).HasComment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی");
            entity.Property(e => e.IsCarWageItemForTax).HasComment("از اين قلم براي محاسبه رقم کسر شده بابت خودرو سازماني استفاده مي شود");
            entity.Property(e => e.IsCaseBonusWageItemForTax).HasComment("پاداش هاي موردي ماه جاري - ريالي");
            entity.Property(e => e.IsConsultingFeesAndSimilar).HasComment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف");
            entity.Property(e => e.IsContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی");
            entity.Property(e => e.IsContinuousNonCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsContinuousNonCashOtherBenefitsCost).HasComment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی");
            entity.Property(e => e.IsDismissalCompensation).HasComment("خسارت اخراج- ریالی");
            entity.Property(e => e.IsEmployeeCarDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی");
            entity.Property(e => e.IsEmployeeHousingDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی");
            entity.Property(e => e.IsEndOfServiceBonus).HasComment("پاداش پایان خدمت- ریالی");
            entity.Property(e => e.IsGrossContinuousCashCurrentMonth).HasComment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی");
            entity.Property(e => e.IsHouseWageItemForTax).HasComment("از اين قلم براي محاسبه رقم کسر شده بابت منزل سازماني استفاده مي شود");
            entity.Property(e => e.IsInsuranceWageItemForTax).HasComment("حق بيمه پرداختي موضوع ماده 137 قانون ماليات هاي مستقيم");
            entity.Property(e => e.IsKaraneh).HasComment("کارانه- ریالی");
            entity.Property(e => e.IsLifeInsuranceArticle137).HasComment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م");
            entity.Property(e => e.IsMainTaxItem).HasComment("قلم اصلی مالیات");
            entity.Property(e => e.IsTaxableContinuousCash).HasComment("مالیات پذیر مستمر نقدی");
            entity.Property(e => e.IsTaxableNonContinuousCash).HasComment("مالیات پذیر غیر مستمر نقدی");
            entity.Property(e => e.IsTaxableContinuousNonCash).HasComment("مالیات پذیر مستمر غیر نقدی");
            entity.Property(e => e.IsTaxableNonContinuousNonCash).HasComment("مالیات پذیر غیر مستمر غیر نقدی");
            entity.Property(e => e.ZeroNegativeArears).HasComment("صفر شدن معوقه منفی");
            entity.Property(e => e.IsSpecialTax).HasComment("مالیات خاص");
            entity.Property(e => e.IsTaxDiscount).HasComment("تخفیف مالیاتی");
            entity.Property(e => e.CurrentYearArearsCovered).HasComment("معوقه پذیر سال جاری");
            entity.Property(e => e.IsMedicalExpensesArticle137WageItemForTax).HasComment("هزينه هاي درماني موضوع ماده 137 قانون ماليات هاي مستقيم");
            entity.Property(e => e.IsMedicalInsuranceArticle137).HasComment("حق بیمه های درمان موضوع ماده  137ق.م.م");
            entity.Property(e => e.IsMissionAllowance).HasComment("فوق العاده مسافرت (ماموریت) - ریالی");
            entity.Property(e => e.IsNonContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsNonContinuousCashCurrentMonth).HasComment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی");
            entity.Property(e => e.IsNonContinuousNonCashArearsNoTax).HasComment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsNonContinuousNonCashCostCurrentMonth).HasComment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی");
            entity.Property(e => e.IsOnCallPay).HasComment("حق کشیک");
            entity.Property(e => e.IsOvertime).HasComment("اضافه کاری- ریالی");
            entity.Property(e => e.IsResearchContracts).HasComment("مبلغ قراردادهای پژوهشی- ریالی");
            entity.Property(e => e.IsServiceBuyback).HasComment("بازخرید خدمت- ریالی");
            entity.Property(e => e.IsSeverancePay).HasComment("حق سنوات- ریالی");
            entity.Property(e => e.IsTeachingResearchFees).HasComment("حق التدریس/حق التحقیق/ حق پژوهش");
            entity.Property(e => e.IsTravelExpense).HasComment("هزینه سفر- ریالی");
            entity.Property(e => e.IsUnusedLeavePay).HasComment("حقوق ایام مرخصی استفاده نشده- ریالی");
            entity.Property(e => e.IsWelfareMotivationProductivity).HasComment("رفاهی و انگیزشی و بهره وری");
            entity.Property(e => e.IsWorkEffortExcludingWageSalaryBonus).HasComment("حق السعی ( به استثنای مزد، حقوق، پاداش )");
            entity.Property(e => e.IsYearEndBonus).HasComment("پاداش آخر سال- ریالی");

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeFicheItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeFicheItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationEmployeeTypeFicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeFundTypeDefinition>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeFundTypeDefinitions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeWageItem).WithMany(p => p.OrganisationEmployeeTypeFundTypeDefinitionEmployeeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployerWageItem).WithMany(p => p.OrganisationEmployeeTypeFundTypeDefinitionEmployerWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.FundType).WithMany(p => p.OrganisationEmployeeTypeFundTypeDefinitions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeFundTypeDefinitions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeLeave>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.OrganisationEmployeeTypeLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeLeaves).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeMrt>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeMrts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeMrts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationMrt).WithMany(p => p.OrganisationEmployeeTypeMrts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderType>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCanChange>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCanChangeEmployeeTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCanChanges).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCanChanges).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCheck>(entity =>
        {
            entity.HasOne(d => d.CheckType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeChecks).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeChecks).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeChecks).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeChecks).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationFormula).WithMany(p => p.OrganisationEmployeeTypeOrderTypeChecks).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeCoefficient>(entity =>
        {
            entity.HasOne(d => d.Coefficient).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeCoefficients).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeDescription>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeDescriptions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeDescriptions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeDescriptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeSummaryCalc>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeSummaryCalcs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeSummaryCalcs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeSummaryCalcs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeOrderTypeWageItem>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationEmployeeTypeOrderTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeOrderTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationEmployeeTypeOrderTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeSettlementItem>(entity =>
        {
            entity.HasOne(d => d.PaymentType).WithMany(p => p.OrganisationEmployeeTypeSettlementItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationEmployeeTypeWageItem>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.OrganisationEmployeeTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationEmployeeTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationEmployeeTypeWageItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationFicheItem>(entity =>
        {
            entity.Property(e => e.IsAnnualEydi).HasComment("عیدی سالانه- ریالی");
            entity.Property(e => e.IsBonusExceptYearEndServiceEndProductivity).HasComment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی");
            entity.Property(e => e.IsCarWageItemForTax).HasComment("�� ��� ��� ���� ������ ��� ��� ��� ���� ����� ������� ������� �� ���");
            entity.Property(e => e.IsCaseBonusWageItemForTax).HasComment("����� ��� ����� ��� ���� - �����");
            entity.Property(e => e.IsConsultingFeesAndSimilar).HasComment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف");
            entity.Property(e => e.IsContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی");
            entity.Property(e => e.IsContinuousNonCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsContinuousNonCashOtherBenefitsCost).HasComment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی");
            entity.Property(e => e.IsDismissalCompensation).HasComment("خسارت اخراج- ریالی");
            entity.Property(e => e.IsEmployeeCarDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی");
            entity.Property(e => e.IsEmployeeHousingDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی");
            entity.Property(e => e.IsEndOfServiceBonus).HasComment("پاداش پایان خدمت- ریالی");
            entity.Property(e => e.IsGrossContinuousCashCurrentMonth).HasComment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی");
            entity.Property(e => e.IsHouseWageItemForTax).HasComment("�� ��� ��� ���� ������ ��� ��� ��� ���� ���� ������� ������� �� ���");
            entity.Property(e => e.IsInsuranceWageItemForTax).HasComment("�� ���� ������� ����� ���� 137 ����� ������ ��� ������");
            entity.Property(e => e.IsKaraneh).HasComment("کارانه- ریالی");
            entity.Property(e => e.IsLifeInsuranceArticle137).HasComment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م");
            entity.Property(e => e.IsMainTaxItem).HasComment("قلم اصلی مالیات");
            entity.Property(e => e.IsTaxableContinuousCash).HasComment("مالیات پذیر مستمر نقدی");
            entity.Property(e => e.IsTaxableNonContinuousCash).HasComment("مالیات پذیر غیر مستمر نقدی");
            entity.Property(e => e.IsTaxableContinuousNonCash).HasComment("مالیات پذیر مستمر غیر نقدی");
            entity.Property(e => e.IsTaxableNonContinuousNonCash).HasComment("مالیات پذیر غیر مستمر غیر نقدی");
            entity.Property(e => e.ZeroNegativeArears).HasComment("صفر شدن معوقه منفی");
            entity.Property(e => e.IsSpecialTax).HasComment("مالیات خاص");
            entity.Property(e => e.IsTaxDiscount).HasComment("تخفیف مالیاتی");
            entity.Property(e => e.CurrentYearArearsCovered).HasComment("معوقه پذیر سال جاری");
            entity.Property(e => e.IsMedicalExpensesArticle137WageItemForTax).HasComment("هزينه هاي درماني موضوع ماده 137 قانون ماليات هاي مستقيم");
            entity.Property(e => e.IsMedicalInsuranceArticle137).HasComment("حق بیمه های درمان موضوع ماده  137ق.م.م");
            entity.Property(e => e.IsMissionAllowance).HasComment("فوق العاده مسافرت (ماموریت) - ریالی");
            entity.Property(e => e.IsNonContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsNonContinuousCashCurrentMonth).HasComment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی");
            entity.Property(e => e.IsNonContinuousNonCashArearsNoTax).HasComment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.IsNonContinuousNonCashCostCurrentMonth).HasComment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی");
            entity.Property(e => e.IsOnCallPay).HasComment("حق کشیک");
            entity.Property(e => e.IsOvertime).HasComment("اضافه کاری- ریالی");
            entity.Property(e => e.IsResearchContracts).HasComment("مبلغ قراردادهای پژوهشی- ریالی");
            entity.Property(e => e.IsServiceBuyback).HasComment("بازخرید خدمت- ریالی");
            entity.Property(e => e.IsSeverancePay).HasComment("حق سنوات- ریالی");
            entity.Property(e => e.IsTeachingResearchFees).HasComment("حق التدریس/حق التحقیق/ حق پژوهش");
            entity.Property(e => e.IsTravelExpense).HasComment("هزینه سفر- ریالی");
            entity.Property(e => e.IsUnusedLeavePay).HasComment("حقوق ایام مرخصی استفاده نشده- ریالی");
            entity.Property(e => e.IsWelfareMotivationProductivity).HasComment("رفاهی و انگیزشی و بهره وری");
            entity.Property(e => e.IsWorkEffortExcludingWageSalaryBonus).HasComment("حق السعی ( به استثنای مزد، حقوق، پاداش )");
            entity.Property(e => e.IsYearEndBonus).HasComment("پاداش آخر سال- ریالی");

            entity.HasOne(d => d.EnterType).WithMany(p => p.OrganisationFicheItemEnterTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OFI_EnterType");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationFicheItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OFI_OrganisationChart");

            entity.HasOne(d => d.OrganisationCheckFormula).WithMany(p => p.OrganisationFicheItemOrganisationCheckFormulas).HasConstraintName("FK_OFI_OrganisationCheckFormula");

            entity.HasOne(d => d.OrganisationFormula).WithMany(p => p.OrganisationFicheItemOrganisationFormulas).HasConstraintName("FK_OFI_OrganisationFormula");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.OrganisationFicheItemPaymentTypes)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OFI_PaymentType");

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationFicheItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_OFI_WageItem");
        });

        modelBuilder.Entity<OrganisationFormula>(entity =>
        {
            entity.HasOne(d => d.Formula).WithMany(p => p.OrganisationFormulas).OnDelete(DeleteBehavior.ClientSetNull);
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationFormulas).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationFundType>(entity =>
        {
            entity.HasOne(d => d.FundType).WithMany(p => p.OrganisationFundTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationFundTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationInsJobList>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationInsJobLists).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationJob>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationJobs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationJobCategory>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationJobCategories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationJobGroup>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationJobGroups).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationJobCategory).WithMany(p => p.OrganisationJobGroups).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationJobSeries>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationJobSeries).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationJobCategory).WithMany(p => p.OrganisationJobSeries).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationJobGroup).WithMany(p => p.OrganisationJobSeries).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationJobSkillYearSetting>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationJobSkillYearSettings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganisationJobSkillYearSettings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SkillLevel).WithMany(p => p.OrganisationJobSkillYearSettings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationLeave>(entity =>
        {
            entity.HasOne(d => d.LeaveType).WithMany(p => p.OrganisationLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationLeaves).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationMrt>(entity =>
        {
            entity.Property(e => e.Content).HasDefaultValueSql("(0x)");
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);
            entity.Property(e => e.Title).HasDefaultValue("");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationMrts).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationOrderType>(entity =>
        {
            entity.HasOne(d => d.OrderTypeGroup).WithMany(p => p.OrganisationOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationOrderTypes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationOrderTypeHistoryExclusion>(entity =>
        {
            entity.HasOne(d => d.OrderType).WithMany(p => p.OrganisationOrderTypeHistoryExclusions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationOrderTypeHistoryExclusions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPeymanRow>(entity =>
        {
            entity.Property(e => e.Code).HasComment("کد ردیف پیمان");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPeymanRows).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPosition>(entity =>
        {
            entity.Property(e => e.LockEndDate).HasColumnType("datetime");
            entity.Property(e => e.LockStartDate).HasColumnType("datetime");
            entity.Property(e => e.PositionCode).HasMaxLength(25);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPositions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InsurancePosition).WithMany(p => p.OrganisationPositions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Position).WithMany(p => p.OrganisationPositions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PositionType).WithMany(p => p.OrganisationPositions).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.RelatedNode).WithMany(p => p.RelatedNodeOrganisationPositions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPositionJob>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPositionJobs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationPosition).WithMany(p => p.OrganisationPositionJobs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganisationPositionJobs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPositionOccuptionMoreThanOneCach>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPositionOccuptionMoreThanOneCaches).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPositionSuggested>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPositionSuggesteds).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationPosition).WithMany(p => p.OrganisationPositionSuggesteds).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationPositionSupervisor>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationPositionSupervisors).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationPosition).WithMany(p => p.OrganisationPositionSupervisors).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationProject>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationProjects).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.OrganisationProjects).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationSetting>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationSettings).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Setting).WithMany(p => p.OrganisationSettings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationSettlementCause>(entity =>
        {
        });

        modelBuilder.Entity<OrganisationSettlementItem>(entity =>
        {
        });

        modelBuilder.Entity<OrganisationTempFile>(entity =>
        {
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationTempFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationWageItem>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationWageItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.OrganisationWageItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganisationWorkPlace>(entity =>
        {
            entity.HasOne(d => d.BaseWorkPlace).WithMany(p => p.OrganisationWorkPlaces).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganisationWorkPlaceOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkPlace).WithMany(p => p.OrganisationWorkPlaceWorkPlaces).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationGoal>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.OrganizationGoals).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobAbilityQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobAbilityQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobCompetencyQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobCompetencyQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobEducationFieldQualification>(entity =>
        {
            entity.HasOne(d => d.EducationField).WithMany(p => p.OrganizationJobEducationFieldQualifications).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobEducationFieldQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobEducationGradeQualification>(entity =>
        {
            entity.HasOne(d => d.EducationGrade).WithMany(p => p.OrganizationJobEducationGradeQualifications).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobEducationGradeQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobForeignLanguageQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobForeignLanguageQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobInitialCourseQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobInitialCourseQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobPerformanceEvaluationCriteriaDescription>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobPerformanceEvaluationCriteriaDescriptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobPeriodicTaskDescription>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobPeriodicTaskDescriptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobRequiredCharacterQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobRequiredCharacterQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobRequiredSoftwaresQualification>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobRequiredSoftwaresQualifications).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobRiskAndFaultDescription>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobRiskAndFaultDescriptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OrganizationJobTaskDescription>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.OrganizationJobTaskDescriptions).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<OtherVeteran>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.DurationDay).HasDefaultValue(0);
            entity.Property(e => e.DurationMonth).HasDefaultValue(0);
            entity.Property(e => e.DurationYear).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsActive).HasDefaultValue(false);
            entity.Property(e => e.IsComputeable).HasDefaultValue(false);
            entity.Property(e => e.IsLast).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.SacrificePercent).HasDefaultValue(0);
            entity.Property(e => e.TrackingCode).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.OtherVeterans).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PasswordChangeRateLimit>(entity =>
        {
            entity.HasOne(d => d.AspNetUser).WithMany(p => p.PasswordChangeRateLimits).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PayLocationProgressReport>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PayLocationProgressReports).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PaymentPeriod>(entity =>
        {
            entity.HasIndex(e => new { e.ShamsiYear, e.ShamsiMonth, e.IsDeleted }, "IX_Payment_Period_ShamsiYear_ShamsiMonth_IsDeleted").HasFillFactor(90);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PaymentPeriods).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PaymentPeriodEmployeeBonu>(entity =>
        {
            entity.HasOne(d => d.Coefficient).WithMany(p => p.PaymentPeriodEmployeeBonus).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.PaymentPeriodEmployeeBonus).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.PaymentPeriodEmployeeBonus).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Permission>(entity =>
        {
            entity.Property(e => e.Id).ValueGeneratedNever();
        });

        modelBuilder.Entity<PersonnelFicheItem>(entity =>
        {
            entity.Property(e => e.EnterTypeId).HasDefaultValue(11231L);

            entity.HasOne(d => d.Employee).WithMany(p => p.PersonnelFicheItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EnterType).WithMany(p => p.PersonnelFicheItems)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_PFI_EnterType");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelFicheItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.PersonnelFicheItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelFunction>(entity =>
        {
            entity.HasIndex(e => new { e.EmployeeId, e.Year, e.Month, e.IsDeleted }, "IX_Personnel_Function_EmployeeId_Year_Month_IsDeleted").HasFillFactor(90);

            entity.HasIndex(e => new { e.EmployeeId, e.OrganisationChartId, e.Year, e.Month, e.Id }, "IX_Personnel_Function_ForFiche_Lookup")
                .IsDescending(false, false, false, false, true)
                .HasFilter("([IsConfirmed]=(1))");
        });

        modelBuilder.Entity<PersonnelFunctionExcelFile>(entity =>
        {
            entity.HasOne(d => d.AspNetUsers).WithMany(p => p.PersonnelFunctionExcelFiles).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.PersonnelFunctionExcelFiles).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelFunctionExcelFiles).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.PersonnelFunctionExcelFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelFunctionVisible>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelFunctionVisibles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelLeave>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.PersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.PersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.PersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelLoan>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.PersonnelLoans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LoanType).WithMany(p => p.PersonnelLoans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelLoans).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StartDeductPaymentPeriod).WithMany(p => p.PersonnelLoans).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelLoanPayment>(entity =>
        {
            entity.HasOne(d => d.PaymentType).WithMany(p => p.PersonnelLoanPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PersonnelLoan).WithMany(p => p.PersonnelLoanPayments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelManagerList>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.PersonnelManagerLists).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.PersonnelManagerLists).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PersonnelPayment>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.PersonnelPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Fiche).WithMany(p => p.PersonnelPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.PersonnelPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PersonnelFicheItem).WithMany(p => p.PersonnelPayments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<PunishmentEncourage>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");

            entity.HasOne(d => d.AgentOfPunishmentEncourage).WithMany(p => p.PunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Employee).WithMany(p => p.PunishmentEncourages).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationAgentOfPunishmentEncourageScoreInterval).WithMany(p => p.PunishmentEncourages).HasConstraintName("FK_Punishment_Encourage_Organisation_Agent_Of_Punishment_Encourage_Score_Interval_OrganisationAgentOfPunishmentEncourageScoreIn~");
        });

        modelBuilder.Entity<RecruitOrder>(entity =>
        {
            entity.HasOne(d => d.CostCenter).WithMany(p => p.RecruitOrderCostCenters)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruit_Order_CostCenter");

            entity.HasOne(d => d.Employee).WithMany(p => p.RecruitOrders)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK_Recruit_Order_Employee");

            entity.HasOne(d => d.EmployeeStatus).WithMany(p => p.RecruitOrders).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.RecruitOrders).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationPosition).WithMany(p => p.RecruitOrders).HasForeignKey(d => d.OrganisationPositionId);

            entity.HasOne(d => d.PayLocation).WithMany(p => p.RecruitOrderPayLocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Project).WithMany(p => p.RecruitOrders).HasConstraintName("FK_Recruit_Order_Project");
        });

        modelBuilder.Entity<RelatedOrganizationJobDescription>(entity =>
        {
            entity.HasOne(d => d.OrganizationJob).WithMany(p => p.RelatedOrganizationJobDescriptionOrganizationJobs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationRelatedJob).WithMany(p => p.RelatedOrganizationJobDescriptionOrganizationRelatedJobs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<ReportableField>(entity =>
        {
            entity.HasOne(d => d.FieldDataType).WithMany(p => p.ReportableFields).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.ReportableEntity).WithMany(p => p.ReportableFields).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RequestDocumentRequirementDetail>(entity =>
        {
            entity.HasOne(d => d.RequestDocumentRequirement).WithMany(p => p.RequestDocumentRequirementDetails).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoleClaim>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.RoleClaims).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoleMenu>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.RoleMenus).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoleReport>(entity =>
        {
            entity.Property(e => e.DynamicReportId).HasComment("fill from DynamicReport Table in schema rpt");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.RoleReports).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Role).WithMany(p => p.RoleReports).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<RoleReportableEntity>(entity =>
        {
            entity.HasOne(d => d.Role).WithMany(p => p.RoleReportableEntities).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<SendedSm>(entity =>
        {
            entity.HasIndex(e => new { e.Ipaddress, e.CreateDate }, "IX_SendedSMS_IPAddress_CreateDate").HasFilter("([IPAddress] IS NOT NULL AND [IPAddress]<>'')");

            entity.HasIndex(e => new { e.MobileNumber, e.CreateDate }, "IX_SendedSMS_MobileNumber_CreateDate").HasFilter("([MobileNumber] IS NOT NULL)");
        });

        modelBuilder.Entity<StatusList>(entity =>
        {
            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.StatusLists).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<StatusListItem>(entity =>
        {
            entity.HasOne(d => d.BankDiskette).WithMany(p => p.StatusListItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.CurrentServiceLocation).WithMany(p => p.StatusListItemCurrentServiceLocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeStatus).WithMany(p => p.StatusListItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.StatusListItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganCodes).WithMany(p => p.StatusListItemOrganCodes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.StatusList).WithMany(p => p.StatusListItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.StatusListItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TashkilatTempFile>(entity =>
        {
            entity.Property(e => e.Extension).HasComputedColumnSql("(left(case when [title] like '%.%' then reverse(left(reverse([title]),charindex('.',reverse([title])))) else '' end,(30)))", false);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TashkilatTempFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Tax>(entity =>
        {
            entity.HasOne(d => d.EmployeeType).WithMany(p => p.Taxes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.Taxes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.Taxes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxCoefficientItem>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TaxCoefficientItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Tax).WithMany(p => p.TaxCoefficientItems).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.TaxCoefficientItems).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDiskette>(entity =>
        {
            entity.HasIndex(e => new { e.PaymentPeriodId, e.BatchPayRollRequestId, e.IsDeleted }, "IX_Tax_Diskette_PaymentPeriodId_BatchPayRollRequestId_IsDeleted")
                .IsUnique()
                .HasFilter("([BatchPayRollRequestId] IS NOT NULL)");

            entity.Property(e => e.CalculateAllFichesInCurrentPeriod).HasComment("محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TaxDiskettes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.TaxDiskettes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDisketteCostCenter>(entity =>
        {
            entity.Property(e => e.TaxDisketteId).HasComment("شناسه جدول دیسکت");

            entity.HasOne(d => d.CostCenter).WithMany(p => p.TaxDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TaxDiskette).WithMany(p => p.TaxDisketteCostCenters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDisketteFile>(entity =>
        {
            entity.HasOne(d => d.TaxDiskette).WithMany(p => p.TaxDisketteFiles).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDisketteWh>(entity =>
        {
            entity.Property(e => e.AnnualEydi).HasComment("عیدی سالانه- ریالی");
            entity.Property(e => e.BonusExceptYearEndServiceEndProductivity).HasComment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی");
            entity.Property(e => e.ConsultingFeesAndSimilar).HasComment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف");
            entity.Property(e => e.ContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی");
            entity.Property(e => e.ContinuousNonCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.ContinuousNonCashOtherBenefitsCost).HasComment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی");
            entity.Property(e => e.DismissalCompensation).HasComment("خسارت اخراج- ریالی");
            entity.Property(e => e.EmployeeCarDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی");
            entity.Property(e => e.EmployeeHousingDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی");
            entity.Property(e => e.EmployeeId).HasComment("شناسه کارمند");
            entity.Property(e => e.EndOfServiceBonus).HasComment("پاداش پایان خدمت- ریالی");
            entity.Property(e => e.GrossContinuousCashCurrentMonth).HasComment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی");
            entity.Property(e => e.Karaneh).HasComment("کارانه- ریالی");
            entity.Property(e => e.LifeInsuranceArticle137).HasComment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م");
            entity.Property(e => e.MedicalInsuranceArticle137).HasComment("حق بیمه های درمان موضوع ماده  137ق.م.م");
            entity.Property(e => e.MissionAllowance).HasComment("فوق العاده مسافرت (ماموریت) - ریالی");
            entity.Property(e => e.NonContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.NonContinuousCashCurrentMonth).HasComment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی");
            entity.Property(e => e.NonContinuousNonCashArearsNoTax).HasComment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.NonContinuousNonCashCostCurrentMonth).HasComment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی");
            entity.Property(e => e.OnCallPay).HasComment("حق کشیک");
            entity.Property(e => e.Overtime).HasComment("اضافه کاری- ریالی");
            entity.Property(e => e.ResearchContracts).HasComment("مبلغ قراردادهای پژوهشی- ریالی");
            entity.Property(e => e.ServiceBuyback).HasComment("بازخرید خدمت- ریالی");
            entity.Property(e => e.SeverancePay).HasComment("حق سنوات- ریالی");
            entity.Property(e => e.TeachingResearchFees).HasComment("حق التدریس/حق التحقیق/ حق پژوهش");
            entity.Property(e => e.Title).HasDefaultValue("");
            entity.Property(e => e.TravelExpense).HasComment("هزینه سفر- ریالی");
            entity.Property(e => e.UnusedLeavePay).HasComment("حقوق ایام مرخصی استفاده نشده- ریالی");
            entity.Property(e => e.WelfareMotivationProductivity).HasComment("رفاهی و انگیزشی و بهره وری");
            entity.Property(e => e.WorkEffortExcludingWageSalaryBonus).HasComment("حق السعی ( به استثنای مزد، حقوق، پاداش )");
            entity.Property(e => e.YearEndBonus).HasComment("پاداش آخر سال- ریالی");

            entity.HasOne(d => d.Employee).WithMany(p => p.TaxDisketteWhs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Fiche).WithMany(p => p.TaxDisketteWhs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.InterdictOrder).WithMany(p => p.TaxDisketteWhs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentType).WithMany(p => p.TaxDisketteWhs).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TaxDiskette).WithMany(p => p.TaxDisketteWhs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDisketteWk>(entity =>
        {
            entity.Property(e => e.AnnualEydi).HasComment("عیدی سالانه- ریالی");
            entity.Property(e => e.BonusExceptYearEndServiceEndProductivity).HasComment("پاداش (به استثنای پاداش آخر سال و پاداش پایان خدمت و پاداش بهره وری) - ریالی");
            entity.Property(e => e.ConsultingFeesAndSimilar).HasComment("مبلغ حق الزحمه/حق مشاوره/حق حضور/حق نظارت/حق التالیف/ حق فنی/ پاداش شورای حل اختلاف");
            entity.Property(e => e.ContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است - ریالی");
            entity.Property(e => e.ContinuousNonCashArearsNoTax).HasComment("مبلغ حقوق و مزایای مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.ContinuousNonCashOtherBenefitsCost).HasComment("مبلغ قیمت تمام شده سایر مزایای مستمر غیرنقدی- ریالی");
            entity.Property(e => e.DismissalCompensation).HasComment("خسارت اخراج- ریالی");
            entity.Property(e => e.EmployeeCarDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت اتومبیل اختصاصی ماه جاری- ریالی");
            entity.Property(e => e.EmployeeHousingDeductionCurrentMonth).HasComment("مبلغ کسر شده از حقوق کارمند بابت مسکن ماه جاری- ریالی");
            entity.Property(e => e.EndOfServiceBonus).HasComment("پاداش پایان خدمت- ریالی");
            entity.Property(e => e.GrossContinuousCashCurrentMonth).HasComment("مبلغ جمع ناخالص حقوق و مزایای مستمر نقدی ماه جاری - ریالی");
            entity.Property(e => e.Karaneh).HasComment("کارانه- ریالی");
            entity.Property(e => e.LifeInsuranceArticle137).HasComment("حق بیمه های عمر و زندگی موضوع ماده  137ق.م.م");
            entity.Property(e => e.MedicalInsuranceArticle137).HasComment("حق بیمه های درمان موضوع ماده  137ق.م.م");
            entity.Property(e => e.MissionAllowance).HasComment("فوق العاده مسافرت (ماموریت) - ریالی");
            entity.Property(e => e.NonContinuousCashArearsNoTax).HasComment("مبلغ حقوق و مزایای غیر مستمر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.NonContinuousCashCurrentMonth).HasComment("سایر حقوق و مزایای غیر مستمر نقدی ماه جاری- ریالی");
            entity.Property(e => e.NonContinuousNonCashArearsNoTax).HasComment("مبلغ مزایای غیر مستمر غیر نقدی معوق که مالیاتی برای آنها محاسبه نشده است- ریالی");
            entity.Property(e => e.NonContinuousNonCashCostCurrentMonth).HasComment("مبلغ قیمت تمام شده مزایای غیر مستمر غیرنقدی ماه جاری- ریالی");
            entity.Property(e => e.OnCallPay).HasComment("حق کشیک");
            entity.Property(e => e.Overtime).HasComment("اضافه کاری- ریالی");
            entity.Property(e => e.ResearchContracts).HasComment("مبلغ قراردادهای پژوهشی- ریالی");
            entity.Property(e => e.ServiceBuyback).HasComment("بازخرید خدمت- ریالی");
            entity.Property(e => e.SeverancePay).HasComment("حق سنوات- ریالی");
            entity.Property(e => e.TeachingResearchFees).HasComment("حق التدریس/حق التحقیق/ حق پژوهش");
            entity.Property(e => e.TravelExpense).HasComment("هزینه سفر- ریالی");
            entity.Property(e => e.UnusedLeavePay).HasComment("حقوق ایام مرخصی استفاده نشده- ریالی");
            entity.Property(e => e.WelfareMotivationProductivity).HasComment("رفاهی و انگیزشی و بهره وری");
            entity.Property(e => e.WorkEffortExcludingWageSalaryBonus).HasComment("حق السعی ( به استثنای مزد، حقوق، پاداش )");
            entity.Property(e => e.YearEndBonus).HasComment("پاداش آخر سال- ریالی");

            entity.HasOne(d => d.PaymentType).WithMany(p => p.TaxDisketteWks).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TaxDiskette).WithMany(p => p.TaxDisketteWks).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxDisketteWp>(entity =>
        {
            entity.Property(e => e.EmployeeId).HasComment("شناسه کارمند");
            entity.Property(e => e.ExemptionType).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.TaxDisketteWps).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Fiche).WithMany(p => p.TaxDisketteWps).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.TaxDiskette).WithMany(p => p.TaxDisketteWps).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxNonCashPayment>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.TaxNonCashPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TaxNonCashPayments).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.TaxNonCashPayments).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxTable>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TaxTables).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.Tax).WithMany(p => p.TaxTables).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TaxableIncome>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.TaxableIncomes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TaxableIncomes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.TaxableIncomes).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WageItem).WithMany(p => p.TaxableIncomes).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TempEmployeeDeduction>(entity =>
        {

            entity.HasOne(d => d.EmployeeDeductionUploadBatch).WithMany(p => p.TempEmployeeDeductions).OnDelete(DeleteBehavior.ClientSetNull);

        });

        modelBuilder.Entity<TempPersonnelLeave>(entity =>
        {
            entity.HasOne(d => d.Employee).WithMany(p => p.TempPersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.LeaveType).WithMany(p => p.TempPersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.TempPersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.PaymentPeriod).WithMany(p => p.TempPersonnelLeaves).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<TempPunishmentEncourage>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.NationalNo).HasDefaultValue("");
            entity.Property(e => e.Value).HasDefaultValue(0);
        });

        modelBuilder.Entity<UserClaim>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserClaims).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserCostCenter>(entity =>
        {
            entity.HasOne(d => d.CostCenter).WithMany(p => p.UserCostCenterCostCenters).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.UserCostCenterOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserCostCenters).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserDefaultSetting>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserDefaultSettings).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserFileUpload>(entity =>
        {
            entity.HasOne(d => d.File).WithMany(p => p.UserFileUploads).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserLogin>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserLogins).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserLoginHistory>(entity =>
        {
            entity.HasIndex(e => new { e.AspNetUserId, e.IsSuccess }, "IX_User_Login_History_Failed_Logins")
                .HasFilter("([IsSuccess]=(0) AND [IsDeleted]=(0))")
                .HasFillFactor(90);

            entity.HasIndex(e => new { e.AspNetUserId, e.IsSuccess }, "IX_User_Login_History_Success_Logins")
                .HasFilter("([IsSuccess]=(1) AND [IsDeleted]=(0))")
                .HasFillFactor(90);

            entity.Property(e => e.Title).HasDefaultValue("");
        });

        modelBuilder.Entity<UserMenu>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserMenus).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserOrganizationUnit>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.UserOrganizationUnitOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganizationUnit).WithMany(p => p.UserOrganizationUnitOrganizationUnits).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserOrganizationUnits).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserPasswordHistory>(entity =>
        {
            entity.HasOne(d => d.AspNetUser).WithMany(p => p.UserPasswordHistories).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserPayLocation>(entity =>
        {
            entity.HasOne(d => d.PayLocation).WithMany(p => p.UserPayLocations).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserPayLocations).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserReport>(entity =>
        {
            entity.Property(e => e.DynamicReportId).HasComment("fill from DynamicReport Table in schema rpt");

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.UserReports).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserReports).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserReportableEntity>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserReportableEntities).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserSignature>(entity =>
        {
            entity.HasOne(d => d.AspNetUsers).WithMany(p => p.UserSignatures).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.UserSignatures).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.SignatureImage).WithMany(p => p.UserSignatures).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserToken>(entity =>
        {
            entity.HasOne(d => d.User).WithMany(p => p.UserTokens).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<UserWorkPlace>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.UserWorkPlaceOrganisationCharts).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.User).WithMany(p => p.UserWorkPlaces).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.WorkPlace).WithMany(p => p.UserWorkPlaceWorkPlaces).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<VersionChangeLog>(entity =>
        {
            entity.HasOne(d => d.Version).WithMany(p => p.VersionChangeLogs).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<VwEmpWork>(entity =>
        {
            entity.ToView("vwEmp_Work", "emp");
        });

        modelBuilder.Entity<VwInterdictOrder>(entity =>
        {
            entity.ToView("vw_Interdict_Order", "Order");
        });

        modelBuilder.Entity<VwOrgChartJob>(entity =>
        {
            entity.ToView("vw_OrgChart_Job", "Org");
        });

        modelBuilder.Entity<VwOrganisationChartTree>(entity =>
        {
            entity.ToView("vwOrganisationChartTree", "Org");
        });

        modelBuilder.Entity<War>(entity =>
        {
            entity.Property(e => e.AcceptableDurationForTaxExemption).HasDefaultValue("");
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Descriptions).HasDefaultValue("");
            entity.Property(e => e.DurationDay).HasDefaultValue(0);
            entity.Property(e => e.DurationMonth).HasDefaultValue(0);
            entity.Property(e => e.DurationYear).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsContinues).HasDefaultValue(false);
            entity.Property(e => e.JebheOperations).HasDefaultValue("");
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.PercentAnnualIncrease).HasDefaultValue(0.0);
            entity.Property(e => e.TrackingCode).HasDefaultValue("");

            entity.HasOne(d => d.Employee).WithMany(p => p.Wars).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<Work>(entity =>
        {
            entity.Property(e => e.CreatedBy).HasDefaultValue("");
            entity.Property(e => e.Description).HasDefaultValue("");
            entity.Property(e => e.ExperienceMult).HasDefaultValue(0);
            entity.Property(e => e.Ipaddress).HasDefaultValue("");
            entity.Property(e => e.IsComputeable).HasDefaultValue(false);
            entity.Property(e => e.LastModifiedBy).HasDefaultValue("");
            entity.Property(e => e.LastTitle).HasDefaultValue("");
            entity.Property(e => e.LetterNumber).HasDefaultValue("");
            entity.Property(e => e.RetiredMult).HasDefaultValue(0);
            entity.Property(e => e.Title).HasDefaultValue("");
            entity.Property(e => e.WorkPlaceDesc).HasDefaultValue("");
            entity.Property(e => e.YearMult).HasDefaultValue(0);

            entity.HasOne(d => d.Employee).WithMany(p => p.Works).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeStatus).WithMany(p => p.Works).OnDelete(DeleteBehavior.ClientSetNull);

            entity.HasOne(d => d.EmployeeType).WithMany(p => p.Works).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WorkFlow>(entity =>
        {
            entity.HasOne(d => d.OrganisationChart).WithMany(p => p.WorkFlows).OnDelete(DeleteBehavior.ClientSetNull);
        });

        modelBuilder.Entity<WorkFlowInstance>(entity =>
        {
            entity.HasOne(d => d.WorkFlow).WithMany(p => p.WorkFlowInstances).OnDelete(DeleteBehavior.ClientSetNull);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
