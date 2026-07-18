using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.WorkFlow.Infrastructure.Services;

public class NodeService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<HR.WorkFlow.Core.Data.Node, WorkFlowContext, NodeDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private IQueryable<Core.Data.WorkFlow> ScopedWorkFlows(long? workFlowTypeId = null)
    {
        var query = _unitOfWork.Context.Set<Core.Data.WorkFlow>().AsQueryable();

        if (workFlowTypeId > 0)
        {
            query = query.Where(w => w.WorkFlowTypeId == workFlowTypeId);
        }

        return query.Where(w => w.OrganisationChartId == _currentUserDefaultOrganId);
    }

    private IQueryable<Core.Data.Node> BuildOrganScopedNodesQuery(bool ignoreExpired, long? workFlowTypeId = null)
    {
        IQueryable<Core.Data.Node> query = All(ignoreExpired)
            .Include(n => n.WorkFlow);

        if (workFlowTypeId > 0)
        {
            var workflowIds = ScopedWorkFlows(workFlowTypeId).Select(w => w.Id);
            query = query.Where(n => workflowIds.Contains(n.WorkFlowId));
        }

        return query;
    }

    public new OperationResult GetPagedData(
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        bool ignoreExpired = true,
        long? selectedEmployeeTypeId = null,
        long? employeeId = null,
        IQueryable<Core.Data.Node>? customDataSource = null,
        bool ignoreDefaultOrganId = false)
    {
        var query = customDataSource ?? BuildOrganScopedNodesQuery(ignoreExpired);

        return base.GetPagedData(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            sortDirection,
            ignoreExpired,
            selectedEmployeeTypeId,
            employeeId,
            query,
            IgnoreDefaultOrganId: ignoreDefaultOrganId);
    }

    public OperationResult GetPagedDataByWorkFlowType(
        long workFlowTypeId,
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        bool ignoreExpired = true)
    {
        if (workFlowTypeId <= 0)
        {
            return GetPagedData(currentPage, pageSize, filter, activeSortColumn, sortDirection, ignoreExpired);
        }

        return GetPagedData(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            sortDirection,
            ignoreExpired,
            customDataSource: BuildOrganScopedNodesQuery(ignoreExpired, workFlowTypeId));
    }

    public OperationResult GetAsKeyValuePairByWorkFlowId(long workFlowId)
    {
        if (workFlowId <= 0)
        {
            return GetAsKeyValuePair();
        }

        var items = _unitOfWork.Context.Set<Core.Data.Node>()
            .AsNoTracking()
            .Where(n => n.WorkFlowId == workFlowId && n.OrganisationChartId == _currentUserDefaultOrganId)
            .OrderBy(n => n.Priority)
            .ThenBy(n => n.title)
            .Select(n => new HR.SharedKernel.Data.KeyValuePair
            {
                key = n.Id,
                value = n.title
            })
            .ToList();

        return OperationResult.Succeeded(payload: items);
    }

    private OperationResult? ValidateNodeDto(NodeDTO dto)
    {
        if (dto.WorkFlowId <= 0)
        {
            return OperationResult.Failed("گردش کار انتخاب نشده است");
        }

        var workFlow = _unitOfWork.Context.Set<Core.Data.WorkFlow>()
            .AsNoTracking()
            .FirstOrDefault(w => w.Id == dto.WorkFlowId);

        if (workFlow == null)
        {
            return OperationResult.NotFound("گردش کار یافت نشد");
        }

        if (workFlow.OrganisationChartId != _currentUserDefaultOrganId)
        {
            return OperationResult.Failed("گردش کار متعلق به واحد سازمانی جاری نیست");
        }

        return null;
    }

    public new async Task<OperationResult> CreateForAsync(NodeDTO entityToCreate)
    {
        var validation = ValidateNodeDto(entityToCreate);
        if (validation != null)
        {
            return validation;
        }

        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(NodeDTO entityToUpdate)
    {
        var validation = ValidateNodeDto(entityToUpdate);
        if (validation != null)
        {
            return validation;
        }

        return await base.UpdateForAsync(entityToUpdate);
    }
}
