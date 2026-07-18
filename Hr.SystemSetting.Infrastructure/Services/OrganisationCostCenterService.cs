using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Microsoft.EntityFrameworkCore;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;


using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationCostCenterService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationCostCenter, SystemSettingContext, OrganisationCostCenterDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetCurrentOrganCostCentersAsKeyValuePair(DateTime dt)
    {
        return OperationResult.Succeeded(payload: All(ImpleDate: dt).Include(i => i.CostCenter).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.CostCenterId,
            value = i.CostCenter == null ? "" : i.CostCenter.title
        }));
    }
    public OperationResult GetCurrentOrganPeymanCostCentersAsKeyValuePair(DateTime dt)
    {
        return OperationResult.Succeeded(payload: All(ImpleDate: dt).Include(i => i.CostCenter).Where(i => i.OrganisationChartId == _currentUserDefaultOrganId && i.PeymanRowId > 0).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.CostCenter == null ? "" : i.CostCenter.title
        }));
    }
    public OperationResult GetCostCentersByOrganisationChartId(long organisationChartId, DateTime dt)
    {
        return OperationResult.Succeeded(payload: All(ImpleDate: dt).Include(i => i.CostCenter).Where(i => i.OrganisationChartId == organisationChartId).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.CostCenterId,
            value = i.CostCenter == null ? "" : i.CostCenter.title
        }));
    }
 
}
