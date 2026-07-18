using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using HR.SharedKernel.Dapper;
using HR.BaseInfo.infrastructure.Data;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services
{
    public class OrderTypeGroupService : BaseService<OrderTypeGroup, BaseInfoContext, OrderTypeGroupDTO>, IScopedServices
    {
        public OrderTypeGroupService(IMapper mapper, IUnitOfWork<BaseInfoContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(OrderTypeGroup entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
