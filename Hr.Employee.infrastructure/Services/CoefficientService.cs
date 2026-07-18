using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.BaseInfo.Core.Entities;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class CoefficientService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.Employee.Core.Entities.Coefficient, EmployeeContext, CoefficientDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(HR.Employee.Core.Entities.Coefficient entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
