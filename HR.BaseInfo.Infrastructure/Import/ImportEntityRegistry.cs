using HR.BaseInfo.Core.Entities;

namespace HR.BaseInfo.infrastructure.Import;

/// <summary>
/// Simple BaseInfo entities supported by <see cref="SimpleEntityImportHandler"/> (Phase 1 + Phase 4).
/// </summary>
public static class ImportEntityRegistry
{
    private static readonly Dictionary<string, Type> EntityTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        // Phase 1
        [nameof(TaxOccupation)] = typeof(TaxOccupation),
        [nameof(TaxExemptionType)] = typeof(TaxExemptionType),
        [nameof(HistoryType)] = typeof(HistoryType),
        [nameof(SettlementCause)] = typeof(SettlementCause),
        [nameof(Rank)] = typeof(Rank),
        [nameof(PositionManagementLevel)] = typeof(PositionManagementLevel),
        [nameof(PositionState)] = typeof(PositionState),
        [nameof(SettlementStatus)] = typeof(SettlementStatus),
        [nameof(SettlementDocumentAttachmentType)] = typeof(SettlementDocumentAttachmentType),
        [nameof(LeaveType)] = typeof(LeaveType),
        [nameof(SkillLevel)] = typeof(SkillLevel),
        [nameof(FundType)] = typeof(FundType),
        [nameof(SettlementItem)] = typeof(SettlementItem),
        [nameof(MeasurementUnit)] = typeof(MeasurementUnit),
        [nameof(FormulaUsageLocation)] = typeof(FormulaUsageLocation),
        // Phase 4
        [nameof(EmployeeStatus)] = typeof(EmployeeStatus),
        [nameof(EmployeeType)] = typeof(EmployeeType),
        [nameof(OrderStatus)] = typeof(OrderStatus),
        [nameof(OrderType)] = typeof(OrderType),
        [nameof(PositionType)] = typeof(PositionType),
        [nameof(JobLevel)] = typeof(JobLevel),
        [nameof(JobActivityType)] = typeof(JobActivityType),
        [nameof(EmployeeStatusGroup)] = typeof(EmployeeStatusGroup),
        [nameof(EmployeeTypeGroup)] = typeof(EmployeeTypeGroup),
        [nameof(OrderTypeGroup)] = typeof(OrderTypeGroup),
        [nameof(OrganizationType)] = typeof(OrganizationType),
        [nameof(WageItem)] = typeof(WageItem),
        [nameof(ConfidentialityLevel)] = typeof(ConfidentialityLevel),
        [nameof(InsurancePosition)] = typeof(InsurancePosition),
        [nameof(TaminInsuranceJobList)] = typeof(TaminInsuranceJobList),
        [nameof(ManagementAndStewardshipJob)] = typeof(ManagementAndStewardshipJob),
        [nameof(EmployeeRequestStatus)] = typeof(EmployeeRequestStatus),
        [nameof(ReportMapColumn)] = typeof(ReportMapColumn),
        [nameof(EducationGroup)] = typeof(EducationGroup),
        [nameof(AbsenceType)] = typeof(AbsenceType),
    };

    public static bool IsSupported(string? entityName) =>
        !string.IsNullOrWhiteSpace(entityName) && EntityTypes.ContainsKey(entityName);

    public static Type? GetEntityType(string? entityName) =>
        entityName != null && EntityTypes.TryGetValue(entityName, out var type) ? type : null;

    public static IReadOnlyCollection<string> SupportedEntityNames => EntityTypes.Keys;
}
