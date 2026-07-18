using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class BasijService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<HR.Employee.Core.Entities.Basij, EmployeeContext, BasijDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(HR.Employee.Core.Entities.Basij entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
