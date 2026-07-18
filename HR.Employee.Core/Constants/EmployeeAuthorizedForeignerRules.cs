namespace HR.Employee.Core.Constants;

/// <summary>
/// قواعد سازگاری داده برای وضعیت مجوز اتباع نسبت به ملیت.
/// </summary>
public static class EmployeeAuthorizedForeignerRules
{
    /// <summary>
    /// اگر ملیت نامشخص یا ایرانی باشد، مقدار مجوز اتباع را پاک می‌کند تا ناسازگاری داده ایجاد نشود.
    /// </summary>
    public static void ApplyAuthorizedForeignerRule(Entities.Employee employee)
    {
        if (!employee.NationalityId.HasValue || employee.NationalityId.Value == EmployeeNationalityIds.Iranian)
        {
            employee.IsAuthorizedForeigner = null;
        }
    }
}
