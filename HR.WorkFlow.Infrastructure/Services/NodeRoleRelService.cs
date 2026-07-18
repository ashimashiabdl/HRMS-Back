using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.Extensions.Configuration;

namespace HR.WorkFlow.Infrastructure.Services;

public class NodeRoleRelService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.WorkFlow.Core.Data.NodeRoleRel, WorkFlowContext, NodeRoleRelDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private OperationResult? ValidateNodeBelongsToWorkFlow(long workFlowId, long nodeId)
    {
        var node = _unitOfWork.Context.Set<Core.Data.Node>().Find(nodeId);
        if (node == null)
        {
            return OperationResult.NotFound("گره یافت نشد");
        }

        if (node.WorkFlowId != workFlowId)
        {
            return OperationResult.Failed("گره انتخاب‌شده متعلق به گردش کار انتخاب‌شده نیست");
        }

        return null;
    }

    public new async Task<OperationResult> CreateForAsync(NodeRoleRelDTO entityToCreate)
    {
        var validation = ValidateNodeBelongsToWorkFlow(entityToCreate.WorkFlowId, entityToCreate.NodeId);
        if (validation != null)
        {
            return validation;
        }

        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(NodeRoleRelDTO entityToUpdate)
    {
        var validation = ValidateNodeBelongsToWorkFlow(entityToUpdate.WorkFlowId, entityToUpdate.NodeId);
        if (validation != null)
        {
            return validation;
        }

        return await base.UpdateForAsync(entityToUpdate);
    }
}
