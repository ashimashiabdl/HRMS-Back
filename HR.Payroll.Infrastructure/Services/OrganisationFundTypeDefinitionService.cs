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

public class OrganisationFundTypeDefinitionService : BaseService<OrganisationFundType, PayrollContext, OrganisationFundTypeDefinitionDTO>, IScopedServices
{
    public OrganisationFundTypeDefinitionService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetAsKeyValuePair()
    {
        var pairs = All()
            .Include(i => i.FundType)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .AsNoTracking()
            .GroupBy(i => new { i.FundTypeId, Title = i.FundType == null ? "" : i.FundType.title })
            .Select(g => new HR.SharedKernel.Data.KeyValuePair
            {
                key = g.Key.FundTypeId,
                value = g.Key.Title
            })
            .OrderBy(i => i.value)
            .ToList();

        return OperationResult.Succeeded(payload: pairs);
    }
}

