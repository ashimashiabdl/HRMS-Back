using HR.SharedKernel.Share;

namespace Hr.Employee.infrastructure.Services;

internal static class EmployeeAccessSql
{
    private const int FinalInterdictStatusId = (int)Enums.OrderStatus.FinalOrder;

    private const string RecruitOrderScopePredicate = """
        (
            ro.CostCenterId IN (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@CostCenterIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL)
         OR ro.OrganizationUnitId IN (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@OrganizationUnitIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL)
         OR ro.PayLocationId IN (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@PayLocationIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL)
         OR ro.WorkPlaceId IN (SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@WorkPlaceIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL)
        )
        """;

    public static string EmployeeAccessWhere => $"""
        (
            EXISTS (
                SELECT 1
                FROM [Order].[Recruit_Order] ro
                WHERE ro.EmployeeId = e.Id
                  AND {RecruitOrderScopePredicate}
            )
            OR e.BaseOrganisationId IN (
                SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@PayLocationIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL
            )
        )
        """;

    public static string SelectAccessibleEmployees => $"""
        SELECT e.*
        FROM [emp].[Employee] e
        WHERE {EmployeeAccessWhere}
        """;

    public static string SelectAccessibleEmployeesWithOrganFilter => $"""
        {SelectAccessibleEmployees}
          AND (
            e.BaseOrganisationId = @BaseOrganisationId
            OR EXISTS (
                SELECT 1
                FROM [Order].[Recruit_Order] ro
                INNER JOIN [Order].[Interdict_Order] io ON io.RecruitOrderId = ro.Id AND io.StatusId = {FinalInterdictStatusId}
                WHERE ro.EmployeeId = e.Id
                  AND ro.PayLocationId = @BaseOrganisationId
            )
          )
        """;

    public static string SelectAccessibleEmployeeIds => $"""
        WITH AccessByOrder AS (
            SELECT DISTINCT ro.EmployeeId
            FROM [Order].[Recruit_Order] ro
            WHERE {RecruitOrderScopePredicate}
        ),
        AccessByBaseOrg AS (
            SELECT e.Id AS EmployeeId
            FROM [emp].[Employee] e
            WHERE e.BaseOrganisationId IN (
                SELECT TRY_CAST(value AS BIGINT) FROM STRING_SPLIT(@PayLocationIds, ',') WHERE TRY_CAST(value AS BIGINT) IS NOT NULL
            )
        )
        SELECT DISTINCT EmployeeId FROM (
            SELECT EmployeeId FROM AccessByOrder
            UNION ALL
            SELECT EmployeeId FROM AccessByBaseOrg
        ) x;
        """;

    public static string CountAccessibleEmployeesWithoutFinalOrder => $"""
        SELECT COUNT(*)
        FROM [emp].[Employee] e
        WHERE {EmployeeAccessWhere}
        AND NOT EXISTS (
            SELECT 1
            FROM [Order].[Recruit_Order] ro2
            JOIN [Order].[Interdict_Order] io ON io.RecruitOrderId = ro2.Id AND io.StatusId = {FinalInterdictStatusId}
            WHERE ro2.EmployeeId = e.Id
        )
        """;
}
