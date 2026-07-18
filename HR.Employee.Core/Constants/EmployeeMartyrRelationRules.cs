namespace HR.Employee.Core.Constants;

public static class EmployeeMartyrRelationRules
{
    public static void ApplyMartyrChildTrackingCodeRule(Entities.Employee employee)
    {
        if (employee.MartyrRelationId != EmployeeMartyrRelationIds.MartyrChild)
        {
            employee.MartyrChildTrackingCode = null;
        }
    }
}
