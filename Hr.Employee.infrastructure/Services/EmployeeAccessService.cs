using Dapper;
using Hr.Employee.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeAccessService(
    EmployeeContext context,
    IDapper dapper,
    UserResolverService userResolverService,
    UserCostCenterService userCostCenterService,
    UserOrganizationUnitService userOrganizationUnitService,
    UserPayLocationService userPayLocationService,
    UserWorkPlaceService userWorkPlaceService) : IScopedServices
{
    private readonly EmployeeContext _context = context;
    private readonly IDapper _dapper = dapper;
    private readonly UserResolverService _userResolverService = userResolverService;
    private readonly UserCostCenterService _userCostCenterService = userCostCenterService;
    private readonly UserOrganizationUnitService _userOrganizationUnitService = userOrganizationUnitService;
    private readonly UserPayLocationService _userPayLocationService = userPayLocationService;
    private readonly UserWorkPlaceService _userWorkPlaceService = userWorkPlaceService;

    private EmployeeAccessScope GetUserAccessScope(long currentUserId) =>
        EmployeeAccessScope.FromUserServices(
            currentUserId,
            _userCostCenterService,
            _userOrganizationUnitService,
            _userWorkPlaceService,
            _userPayLocationService);

    public bool CheckAccess(long currentUserId, long employeeId)
    {
        if (_userResolverService.IsAdmin())
        {
            return true;
        }

        return GetAccessibleEmployeesQueryable(currentUserId)
            .AsNoTracking()
            .Any(e => e.Id == employeeId);
    }

    public IEnumerable<long> GetAccessibleEmployeeIds(long currentUserId)
    {
        var scope = GetUserAccessScope(currentUserId);
        if (!scope.HasAnyAccess)
        {
            return [];
        }

        var parameters = new DynamicParameters();
        scope.AddTo(parameters);

        using var connection = _dapper.GetDbconnection();
        return connection.Query<long>(EmployeeAccessSql.SelectAccessibleEmployeeIds, parameters, commandType: System.Data.CommandType.Text)
            .Distinct()
            .ToList();
    }

    public IQueryable<HR.Employee.Core.Entities.Employee> GetAccessibleEmployeesQueryable(long currentUserId, long? baseOrganisationId = null)
    {
        var scope = GetUserAccessScope(currentUserId);
        if (!scope.HasAnyAccess)
        {
            return _context.Employees.Where(e => false);
        }

        var parameters = scope.ToSqlParameters().ToList();
        string sql;
        if (baseOrganisationId.HasValue && baseOrganisationId.Value > 0)
        {
            sql = EmployeeAccessSql.SelectAccessibleEmployeesWithOrganFilter;
            parameters.Add(new SqlParameter("@BaseOrganisationId", baseOrganisationId.Value));
        }
        else
        {
            sql = EmployeeAccessSql.SelectAccessibleEmployees;
        }

        return _context.Employees
            .FromSqlRaw(sql, parameters.ToArray())
            .Include(e => e.BaseOrganisation)
            .Include(e => e.ServicePlace)
            .Include(e => e.Gender);
    }

    public OperationResult GetAccessibleEmployeesWithoutFinalOrderCount(long currentUserId)
    {
        var scope = GetUserAccessScope(currentUserId);
        if (!scope.HasAnyAccess)
        {
            return OperationResult.Succeeded(payload: 0);
        }

        var parameters = new DynamicParameters();
        scope.AddTo(parameters);

        using var connection = _dapper.GetDbconnection();
        var result = connection.ExecuteScalar(EmployeeAccessSql.CountAccessibleEmployeesWithoutFinalOrder, parameters, commandType: System.Data.CommandType.Text);
        var count = result is null or DBNull ? 0 : Convert.ToInt32(result);
        return OperationResult.Succeeded(payload: count);
    }
}
