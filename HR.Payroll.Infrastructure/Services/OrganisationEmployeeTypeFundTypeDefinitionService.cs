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

public class OrganisationEmployeeTypeFundTypeDefinitionService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationEmployeeTypeFundTypeDefinition, PayrollContext, OrganisationEmployeeTypeFundTypeDefinitionDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public new OperationResult GetPagedData(int currentPage = 1, int pageSize = 10, string filter = "", string activeSortColumn = "", string Sortdirection = "", bool IgnoreExpired = true, long? FundTypeId = null)
    {
        IQueryable<OrganisationEmployeeTypeFundTypeDefinition> query = All(IgnoreExpired);

        if (FundTypeId != null && FundTypeId > 0)
        {
            query = query.Where(i => i.FundTypeId == FundTypeId);
        }

        query = query
            .Include(i => i.OrganisationChart)
            .Include(i => i.EmployeeType)
            .Include(i => i.FundType)
            .Include(i => i.EmployeeWageItem)
            .Include(i => i.EmployerWageItem)
            .Include(i => i.EmployeeFormula)
                .ThenInclude(f => f.Formula)
            .Include(i => i.EmployerFormula)
                .ThenInclude(f => f.Formula);

        return base.GetPagedData(currentPage, pageSize, filter, activeSortColumn, Sortdirection, IgnoreExpired, null, null, query, false);
    }
}

