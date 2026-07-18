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

public class ActivityTemplateService(
    IMapper mapper,
    IUnitOfWork<WorkFlowContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<HR.WorkFlow.Core.Data.ActivityTemplate, WorkFlowContext, ActivityTemplateDTO>(
        unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private IQueryable<Core.Data.WorkFlow> ScopedWorkFlows(long? workFlowTypeId = null, long? workFlowId = null)
    {
        var query = _unitOfWork.Context.Set<Core.Data.WorkFlow>().AsQueryable();

        if (workFlowTypeId > 0)
        {
            query = query.Where(w => w.WorkFlowTypeId == workFlowTypeId);
        }

        if (workFlowId > 0)
        {
            query = query.Where(w => w.Id == workFlowId);
        }

        return query.Where(w => w.OrganisationChartId == _currentUserDefaultOrganId);
    }

    private IQueryable<Core.Data.ActivityTemplate> BuildOrganScopedActivitiesQuery(
        bool ignoreExpired,
        long? workFlowTypeId = null,
        long? workFlowId = null,
        long? workFlowInstanceId = null,
        bool? pending = null)
    {
        var workflowIds = ScopedWorkFlows(workFlowTypeId, workFlowId).Select(w => w.Id);

        var query = All(ignoreExpired)
            .Include(a => a.WorkFlowInstance!)
                .ThenInclude(i => i.WorkFlow)
            .Include(a => a.WorkFlowInstance!)
                .ThenInclude(i => i.InterdictOrder)
            .Include(a => a.FromNode)
            .Include(a => a.ToNode)
            .Include(a => a.Action)
            .Include(a => a.UserSignature!)
                .ThenInclude(u => u.AspNetUsers)
            .Where(a => a.WorkFlowInstance != null && workflowIds.Contains(a.WorkFlowInstance.WorkFlowId));

        if (workFlowInstanceId > 0)
        {
            query = query.Where(a => a.WorkFlowInstanceId == workFlowInstanceId);
        }

        if (pending.HasValue)
        {
            query = query.Where(a => a.Pending == pending.Value);
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
        IQueryable<Core.Data.ActivityTemplate>? customDataSource = null,
        bool ignoreDefaultOrganId = false)
    {
        var query = customDataSource ?? BuildOrganScopedActivitiesQuery(ignoreExpired);

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
            IgnoreDefaultOrganId: true);
    }

    public OperationResult GetPagedDataFiltered(
        long? workFlowTypeId,
        long? workFlowId,
        long? workFlowInstanceId,
        bool? pending,
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        bool ignoreExpired = true)
    {
        return GetPagedData(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            sortDirection,
            ignoreExpired,
            customDataSource: BuildOrganScopedActivitiesQuery(
                ignoreExpired,
                workFlowTypeId,
                workFlowId,
                workFlowInstanceId,
                pending));
    }
}
