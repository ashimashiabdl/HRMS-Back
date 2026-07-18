using AutoMapper;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class TempPunishmentEncourageService(IMapper mapper, IUnitOfWork<EmployeeContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService
) : BaseService<HR.Employee.Core.Entities.TempPunishmentEncourage, EmployeeContext, TempPunishmentEncourageDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(HR.Employee.Core.Entities.TempPunishmentEncourage entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}
