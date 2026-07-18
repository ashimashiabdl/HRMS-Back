using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationSettlementItemService : BaseService<OrganisationSettlementItem, SystemSettingContext, OrganisationSettlementItemDTO>, IScopedServices
{
    public OrganisationSettlementItemService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload:
            All()
                .Include(i => i.SettlementItem)
                .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.SettlementItemId,
                    value = i.SettlementItem != null ? i.SettlementItem.title : ""
                }));
    }

    public bool Validate(OrganisationSettlementItem entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
