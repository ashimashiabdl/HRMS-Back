using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Service;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using HR.BaseInfo.infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class CoefficientService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IConfiguration configuration, UserResolverService userService
) : BaseService<Coefficient, BaseInfoContext, CoefficientDTO>(unitOfWork.Context, mapper, unitOfWork, null, configuration, userService), IScopedServices
{
  
}
