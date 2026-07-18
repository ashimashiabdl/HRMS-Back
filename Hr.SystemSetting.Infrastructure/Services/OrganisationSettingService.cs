using AutoMapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationSettingService(IMapper mapper,  IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationSetting, SystemSettingContext, OrganisationSettingDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult CheckCurrentSettingIsActive(int SettingId)
    {
        var setting = All().Where(i => i.SettingId == SettingId && i.OrganisationChartId == _currentUserDefaultOrganId);
        if (setting != null)
        {
            if (setting.Any())
            {
                return OperationResult.Succeeded(payload: setting.SingleOrDefault()!.IsActive);
            }
        }
        return OperationResult.Succeeded(payload: false);
    }

    public OperationResult CheckCurrentSettingValue(int SettingId)
    {
        var organisationSetting = All()
            .Where(i => i.SettingId == SettingId && i.OrganisationChartId == _currentUserDefaultOrganId);

        if (organisationSetting.Any())
        {
            return OperationResult.Succeeded(payload: ParseSettingValueAsBool(organisationSetting.SingleOrDefault()!.Value));
        }

        var globalSetting = _db.Set<Setting>()
            .FirstOrDefault(s => s.Id == SettingId && !s.IsDeleted);

        if (globalSetting != null)
        {
            return OperationResult.Succeeded(payload: ParseSettingValueAsBool(globalSetting.Value));
        }

        return OperationResult.Succeeded(payload: false);
    }

    private static bool ParseSettingValueAsBool(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return false;
        }

        if (bool.TryParse(value.Trim(), out var result))
        {
            return result;
        }

        return value.Trim() switch
        {
            "1" => true,
            "0" => false,
            _ => false
        };
    }

    public bool Validate(OrganisationSetting entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
