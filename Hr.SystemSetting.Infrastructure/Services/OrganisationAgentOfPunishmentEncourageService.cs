using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;

using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services
{
    public class OrganisationAgentOfPunishmentEncourageService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<OrganisationAgentOfPunishmentEncourage, SystemSettingContext, OrganisationAgentOfPunishmentEncourageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
    {
        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All()
                .Include(i => i.AgentOfPunishmentEncourage)
                .Include(i => i.AgentOfPunishmentEncourageGroup)
                .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair()
                {
                    key = i.AgentOfPunishmentEncourageId,
                    value = i.AgentOfPunishmentEncourage == null ? "" : i.AgentOfPunishmentEncourage.title+ " ( " + (i.AgentOfPunishmentEncourage.IsPunishment == true ? "عامل تنبیه" : "عامل تشویقی") + " ) " + " مصداق : " + (i.AgentOfPunishmentEncourageGroup == null ? "" :  i.AgentOfPunishmentEncourageGroup.title)
                }));
        }
        public bool Validate(OrganisationAgentOfPunishmentEncourage entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
