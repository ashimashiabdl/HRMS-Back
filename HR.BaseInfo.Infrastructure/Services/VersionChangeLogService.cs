using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class VersionChangeLogService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<VersionChangeLog, BaseInfoContext, VersionChangeLogDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetByVersionId(long versionId)
    {
        try
        {
            var changeLogs = All(true)
                .Where(x => x.VersionId == versionId && !x.IsDeleted)
                .OrderBy(x => x.ChangeType)
                .ThenBy(x => x.Id)
                .ToList();

            var dtos = _mapper.Map<List<VersionChangeLogDTO>>(changeLogs);
            return OperationResult.Succeeded(payload: dtos);
        }
        catch (Exception ex)
        {
            return OperationResult.Failed(ex.Message);
        }
    }

    public bool Validate(VersionChangeLog entity, object etc = null)
    {
        // Validate ChangeType
        var validChangeTypes = new[] { "Added", "Changed", "Fixed", "Removed", "Security" };
        if (!validChangeTypes.Contains(entity.ChangeType))
        {
            return false;
        }

        // Validate VersionId exists
        if (entity.VersionId <= 0)
        {
            return false;
        }

        var versionExists = _unitOfWork.Context.Versions.Any(v => v.Id == entity.VersionId && !v.IsDeleted);
        if (!versionExists)
        {
            return false;
        }

        return true;
    }
}
