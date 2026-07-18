using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class ConfidentialityLevelService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<ConfidentialityLevel, BaseInfoContext, ConfidentialityLevelDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All().OrderBy(i => i.title).Select(i => new HR.SharedKernel.Data.KeyValuePair
        {
            key = i.Id,
            value = i.title
        }));
    }
    public bool Validate(ConfidentialityLevel entity, object etc = null)
    {
        return true;
    }
}


