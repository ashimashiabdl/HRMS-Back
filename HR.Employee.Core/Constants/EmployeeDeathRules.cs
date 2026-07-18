using HR.SharedKernel.DTOs;

namespace HR.Employee.Core.Constants;

/// <summary>
/// قواعد اعتبارسنجی تاریخ و علت فوت کارمند.
/// </summary>
public static class EmployeeDeathRules
{
    public const string DeathDateMustBeAfterBirthDateMessage = "تاریخ فوت باید بعد از تاریخ تولد باشد";

    /// <summary>
    /// در صورت وجود تاریخ فوت، باید حتماً بعد از تاریخ تولد باشد.
    /// </summary>
    public static OperationResult? Validate(Entities.Employee employee)
    {
        if (!employee.DeathDate.HasValue || !employee.BirthDate.HasValue)
        {
            return null;
        }

        if (employee.DeathDate.Value.Date <= employee.BirthDate.Value.Date)
        {
            return OperationResult.Failed(DeathDateMustBeAfterBirthDateMessage);
        }

        return null;
    }
}
