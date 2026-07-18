using AutoMapper;
using Hr.Employee.infrastructure.Data;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class ExperienceService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
    : BaseService<Experience, EmployeeContext, ExperienceDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
}


