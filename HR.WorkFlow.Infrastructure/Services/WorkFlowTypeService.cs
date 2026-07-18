using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.WorkFlow.Infrastructure.Services
{
    public class WorkFlowTypeService : BaseService<HR.WorkFlow.Core.Data.WorkFlowType, WorkFlowContext, WorkFlowTypeDTO>, IScopedServices
    {
        public WorkFlowTypeService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        public bool Validate(Core.Data.WorkFlowType entity, object etc = null)
        {
            throw new NotImplementedException();
        }
    }
}
