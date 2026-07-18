using AutoMapper;
using HR.Employee.Core.DTOs;
using HR.Order.Core.Data;
using HR.Order.Core.DTOs;
using HR.Order.Infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.Order.Infrastructure.Services
{
    //public class BatchRequestDetailReferenceService : BaseService<BatchRequestDetailReference, OrderContext, BatchRequestDetailReferenceDTO>, IScopedServices
    //{
    //    public BatchRequestDetailReferenceService(IMapper mapper, IUnitOfWork<OrderContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
    //    {

    //    }

    //    public bool Validate(BatchRequestDetailReference entity, object etc = null)
    //    {
    //        throw new NotImplementedException();
    //    }
    //}
}
