using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services;

public class BatchSettlementRequestDetailService(
    IMapper mapper,
    IUnitOfWork<PayrollContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService) : BaseService<BatchSettlementRequestDetail, PayrollContext, BatchSettlementRequestDetailDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public bool Validate(BatchSettlementRequestDetail entity, object? etc = null) => throw new NotImplementedException();
}
