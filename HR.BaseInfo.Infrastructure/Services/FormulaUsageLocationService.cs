using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class FormulaUsageLocationService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<FormulaUsageLocation, BaseInfoContext, FormulaUsageLocationDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
}
