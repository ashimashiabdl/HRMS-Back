using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class EmployeeFundService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<EmployeeFund, PayrollContext, EmployeeFundDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().Include(i=>i.FundType).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.FundTypeId,
            value = i.FundType.title
        }));
    }


    public   OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? EmployeeId = null)
    {
        IQueryable<EmployeeFund> query = All(IgnoreExpired)
            .Include(i => i.Employee)
            .Include(i => i.FundType)
            .Include(i => i.StartDeductPaymentPeriod);

        if (EmployeeId != null && EmployeeId > 0)
        {
            query = query.Where(i => i.EmployeeId == EmployeeId);
        }

        return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, null, null, query, false);
    }
}

