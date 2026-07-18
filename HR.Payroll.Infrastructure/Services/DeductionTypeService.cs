using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class DeductionTypeService : BaseService<DeductionType, PayrollContext, DeductionTypeDTO>, IScopedServices
    {
    public DeductionTypeService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService)
        : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {
    }

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All(false)
            .OrderByDescending(i => i.Id)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.title
            })
            .ToList());
    }
}
}
