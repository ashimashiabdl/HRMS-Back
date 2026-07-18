using HR.Employee.Core.Entities;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// Employee-module child entities supported by <see cref="GenericEmployeeEntityImportHandler"/>.
/// </summary>
public static class EmployeeImportEntityRegistry
{
    private static readonly Dictionary<string, Type> EntityTypes = new(StringComparer.OrdinalIgnoreCase)
    {
        [nameof(Ability)] = typeof(Ability),
        [nameof(AbsenceRecord)] = typeof(AbsenceRecord),
        [nameof(Appearance)] = typeof(Appearance),
        [nameof(Attendance)] = typeof(Attendance),
        [nameof(BankAccount)] = typeof(BankAccount),
        [nameof(Basij)] = typeof(Basij),
        [nameof(BasijGrade)] = typeof(BasijGrade),
        [nameof(Captivity)] = typeof(Captivity),
        [nameof(Character)] = typeof(Character),
        [nameof(Coefficient)] = typeof(Coefficient),
        [nameof(Competency)] = typeof(Competency),
        [nameof(ContactInfo)] = typeof(ContactInfo),
        [nameof(Course)] = typeof(Course),
        [nameof(Disability)] = typeof(Disability),
        [nameof(DrivingLicense)] = typeof(DrivingLicense),
        [nameof(EmployeeRequest)] = typeof(EmployeeRequest),
        [nameof(EmployeeSoftware)] = typeof(EmployeeSoftware),
        [nameof(EvaluationResult)] = typeof(EvaluationResult),
        [nameof(Experience)] = typeof(Experience),
        [nameof(ForeignLanguage)] = typeof(ForeignLanguage),
        [nameof(ForeignTravel)] = typeof(ForeignTravel),
        [nameof(HistoryStop)] = typeof(HistoryStop),
        [nameof(Insurance)] = typeof(Insurance),
        [nameof(Isar)] = typeof(Isar),
        [nameof(MilitaryService)] = typeof(MilitaryService),
        [nameof(OtherVeteran)] = typeof(OtherVeteran),
        [nameof(PunishmentEncourage)] = typeof(PunishmentEncourage),
        [nameof(War)] = typeof(War),
        [nameof(Work)] = typeof(Work),
    };

    public static bool IsSupported(string? entityName) =>
        !string.IsNullOrWhiteSpace(entityName) && EntityTypes.ContainsKey(entityName);

    public static Type? GetEntityType(string? entityName) =>
        entityName != null && EntityTypes.TryGetValue(entityName, out var type) ? type : null;

    public static IReadOnlyCollection<string> SupportedEntityNames => EntityTypes.Keys;
}
