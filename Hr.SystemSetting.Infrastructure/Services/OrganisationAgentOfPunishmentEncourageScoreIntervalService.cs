using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationAgentOfPunishmentEncourageScoreIntervalService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<OrganisationAgentOfPunishmentEncourageScoreInterval, SystemSettingContext, OrganisationAgentOfPunishmentEncourageScoreIntervalDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{


    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All()

            .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair()
            {
                key = i.Id,
                value = i.title + " از : " + i.FromValue + " تا : " + i.ToValue
            }));
    }

    public bool Validate(OrganisationAgentOfPunishmentEncourageScoreInterval entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
