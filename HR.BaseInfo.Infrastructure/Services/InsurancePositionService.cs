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

namespace HR.BaseInfo.infrastructure.Services
{
    public class InsurancePositionService : BaseService<InsurancePosition, BaseInfoContext, InsurancePositionDTO>, IScopedServices
    {
        public InsurancePositionService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(InsurancePosition entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
