using HR.Identity.infrastructure.Services;
using Microsoft.Data.SqlClient;
using Dapper;

namespace Hr.Employee.infrastructure.Services;

/// <summary>
/// محدوده دسترسی کاربر به کارمندان بر اساس مرکز هزینه، واحد سازمانی، محل کار و محل پرداخت.
/// </summary>
internal sealed class EmployeeAccessScope
{
    public IReadOnlyList<long> CostCenterIds { get; init; } = [];
    public IReadOnlyList<long> OrganizationUnitIds { get; init; } = [];
    public IReadOnlyList<long> WorkPlaceIds { get; init; } = [];
    public IReadOnlyList<long> PayLocationIds { get; init; } = [];

    public bool HasAnyAccess =>
        CostCenterIds.Count > 0 ||
        OrganizationUnitIds.Count > 0 ||
        WorkPlaceIds.Count > 0 ||
        PayLocationIds.Count > 0;

    public IEnumerable<SqlParameter> ToSqlParameters()
    {
        yield return new SqlParameter("@CostCenterIds", ToCsv(CostCenterIds));
        yield return new SqlParameter("@OrganizationUnitIds", ToCsv(OrganizationUnitIds));
        yield return new SqlParameter("@WorkPlaceIds", ToCsv(WorkPlaceIds));
        yield return new SqlParameter("@PayLocationIds", ToCsv(PayLocationIds));
    }

    public void AddTo(DynamicParameters parameters)
    {
        parameters.Add("@CostCenterIds", ToCsv(CostCenterIds));
        parameters.Add("@OrganizationUnitIds", ToCsv(OrganizationUnitIds));
        parameters.Add("@WorkPlaceIds", ToCsv(WorkPlaceIds));
        parameters.Add("@PayLocationIds", ToCsv(PayLocationIds));
    }

    public static EmployeeAccessScope FromUserServices(
        long currentUserId,
        UserCostCenterService costCenterService,
        UserOrganizationUnitService organizationUnitService,
        UserWorkPlaceService workPlaceService,
        UserPayLocationService payLocationService)
    {
        static List<long> ToIds(object? payload) =>
            payload is IEnumerable<HR.SharedKernel.Data.KeyValuePair> items
                ? items.Select(i => Convert.ToInt64(i.key)).ToList()
                : [];

        return new EmployeeAccessScope
        {
            CostCenterIds = ToIds(costCenterService.GetAsKeyValuePair(currentUserId).Payload),
            OrganizationUnitIds = ToIds(organizationUnitService.GetAsKeyValuePair(currentUserId).Payload),
            WorkPlaceIds = ToIds(workPlaceService.GetAsKeyValuePair(currentUserId).Payload),
            PayLocationIds = ToIds(payLocationService.GetAsKeyValuePair(currentUserId).Payload),
        };
    }

    private static string ToCsv(IReadOnlyList<long> ids) =>
        ids.Count > 0 ? string.Join(",", ids) : "-1";
}
