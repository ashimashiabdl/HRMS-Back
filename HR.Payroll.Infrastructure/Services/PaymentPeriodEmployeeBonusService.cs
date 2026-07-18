

using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;

using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class PaymentPeriodEmployeeBonusService : BaseService<PaymentPeriodEmployeeBonus, PayrollContext, PaymentPeriodEmployeeBonusDTO>, IScopedServices
{
    public PaymentPeriodEmployeeBonusService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    {

    }

    public bool Validate(PaymentPeriodEmployeeBonus entity, object etc = null)
    {
        throw new NotImplementedException();
    }
}