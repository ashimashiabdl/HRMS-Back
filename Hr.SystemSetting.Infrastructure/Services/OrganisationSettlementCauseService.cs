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

public class OrganisationSettlementCauseService : BaseService<OrganisationSettlementCause, SystemSettingContext, OrganisationSettlementCauseDTO>, IScopedServices
{
    public OrganisationSettlementCauseService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload:
            All()
                .Include(i => i.SettlementCause)
                .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.SettlementCauseId,
                    value = i.SettlementCause != null ? i.SettlementCause.title : ""
                })
                .ToList());
    }

    public bool Validate(OrganisationSettlementCause entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
