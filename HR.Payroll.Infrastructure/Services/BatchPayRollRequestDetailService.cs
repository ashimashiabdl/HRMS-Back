using AutoMapper;
using HR.Payroll.Core.Data;
using HR.Payroll.Core.DTOs;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;


using Microsoft.Extensions.Configuration;

namespace HR.Payroll.Infrastructure.Services
{
    public class BatchPayRollRequestDetailService : BaseService<BatchPayRollRequestDetail, PayrollContext, BatchPayRollRequestDetailDTO>, IScopedServices
    {
        public BatchPayRollRequestDetailService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(BatchPayRollRequestDetail entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
