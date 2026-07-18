using AutoMapper;
using HR.Order.Core.DTOs;
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
    public class FicheItemService : BaseService<FicheItem, PayrollContext, FicheItemDTO>, IScopedServices
    {
        public FicheItemService(IMapper mapper, IUnitOfWork<PayrollContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(FicheItem entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
