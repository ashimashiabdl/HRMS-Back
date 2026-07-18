using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using System.Linq;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class EmployeeLeaveEntitlementService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<EmployeeLeaveEntitlement, PayrollContext, EmployeeLeaveEntitlementDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? EmployeeId = null, int? Year = null)
    {
        IQueryable<EmployeeLeaveEntitlement> dataSource = All(IgnoreExpired)
            .Include(x => x.Employee)
            .Include(x => x.LeaveType)
            .Include(x => x.OrganisationChart);
        
        // Filter by EmployeeId if provided
        if (EmployeeId.HasValue && EmployeeId.Value > 0)
        {
            dataSource = dataSource.Where(x => x.EmployeeId == EmployeeId.Value);
        }
        
        // Filter by Year if provided
        if (Year.HasValue && Year.Value > 0)
        {
            dataSource = dataSource.Where(x => x.Year == Year.Value);
        }
        
        // Pass CustomDataSource with IgnoreDefaultOrganId = true to prevent base filtering issues
        return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, null, null, dataSource, false);
    }

    public OperationResult GetDistinctYears(long? EmployeeId = null)
    {
        var query = All(IgnoreExpired: false)
                   .Where(x => x.OrganisationChartId == _currentUserDefaultOrganId);

        // Filter by EmployeeId if provided
        if (EmployeeId.HasValue && EmployeeId.Value > 0)
        {
            query = query.Where(x => x.EmployeeId == EmployeeId.Value);
        }

        var distinctYears = query
            .Select(x => x.Year)
            .Distinct()
            .OrderByDescending(x => x)
            .ToList()
            .Select(year => new { Key = year, Value = year.ToString() })
            .ToList();

        return OperationResult.Succeeded(payload: distinctYears);
    }
}
