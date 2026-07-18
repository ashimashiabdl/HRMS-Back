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

public class OrganisationPeymanRowService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationPeymanRow, SystemSettingContext, OrganisationPeymanRowDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public  OperationResult GetAsKeyValuePair(long currentUserDefaultOrganId)
    {
        return OperationResult.Succeeded(payload: All().Where(i=>i.OrganisationChartId == currentUserDefaultOrganId).OrderByDescending(i => i.Id).Select(i => new HR.SharedKernel.Data.KeyValuePair()
        {
            key = i.Id,
            value = i.Code + " - " + i.title
        }));
    }
}
