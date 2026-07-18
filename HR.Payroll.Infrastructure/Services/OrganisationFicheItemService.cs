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

public class OrganisationFicheItemService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationFicheItem, PayrollContext, OrganisationFicheItemDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetAsKeyValuePair(long currentUserDefaultOrganId)
    {
        return OperationResult.Succeeded(payload: All().Where(i => i.OrganisationChartId == currentUserDefaultOrganId).Include(i => i.WageItem).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.WageItem.Id,
            value = i.WageItem.title
        }));
    }

    public OperationResult GetNextPriority(long organisationChartId)
    {
        return OperationResult.Succeeded(payload: GetNextPriorityValue(organisationChartId));
    }

    private int GetNextPriorityValue(long organisationChartId)
    {
        var maxPriority = All()
            .Where(i => i.OrganisationChartId == organisationChartId)
            .Select(i => (int?)i.Priority)
            .Max();

        return (maxPriority ?? 0) + 1;
    }
}
