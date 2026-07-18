using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Microsoft.Extensions.Configuration;

namespace HR.WorkFlow.Infrastructure.Services;

public class WorkFlowService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.WorkFlow.Core.Data.WorkFlow, WorkFlowContext, WorkFlowDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetAsKeyValuePairByWorkFlowTypeId(long workFlowTypeId)
    {
        if (workFlowTypeId <= 0)
        {
            return GetAsKeyValuePair();
        }

        var items = _unitOfWork.Context.Set<Core.Data.WorkFlow>()
            .AsNoTracking()
            .Where(w => w.WorkFlowTypeId == workFlowTypeId)
            .OrderByDescending(w => w.IsActive)
            .ThenBy(w => w.title)
            .Select(w => new HR.SharedKernel.Data.KeyValuePair
            {
                key = w.Id,
                value = w.title
            })
            .ToList();

        return OperationResult.Succeeded(payload: items);
    }
}
