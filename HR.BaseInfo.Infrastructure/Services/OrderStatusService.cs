using AutoMapper;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class OrderStatusService : BaseService<OrderStatus, BaseInfoContext, OrderStatusDTO>, IScopedServices
    {
        public OrderStatusService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(OrderStatus entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
