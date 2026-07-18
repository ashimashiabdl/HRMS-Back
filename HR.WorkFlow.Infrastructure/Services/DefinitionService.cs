using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Share;
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

namespace HR.WorkFlow.Infrastructure.Services
{
    public class DefinitionService : BaseService<HR.WorkFlow.Core.Data.Definition, WorkFlowContext, DefinitionDTO>, IScopedServices
    {
        public DefinitionService(IMapper mapper, IUnitOfWork<WorkFlowContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : base(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService)
        {

        }

        /// <summary>
        /// حذف فیزیکی مسیر گردش کار از جدول wf.Definition (نه soft delete)
        /// </summary>
        public new OperationResult DeleteRecord(long id)
        {
            var entity = _unitOfWork.Context.Set<Core.Data.Definition>()
                .FirstOrDefault(e => e.Id == id);

            if (entity == null)
            {
                return OperationResult.NotFound("مسیر گردش کار یافت نشد");
            }

            _unitOfWork.Context.Set<Core.Data.Definition>().Remove(entity);

            if (_unitOfWork.Save().Result > 0)
            {
                return OperationResult.Succeeded("مسیر گردش کار با موفقیت حذف شد", payload: 1);
            }

            return OperationResult.Failed("حذف مسیر گردش کار انجام نشد");
        }

        private IQueryable<Core.Data.WorkFlow> ScopedWorkFlows(long? workFlowTypeId = null)
        {
            var query = _unitOfWork.Context.Set<Core.Data.WorkFlow>().AsQueryable();

            if (workFlowTypeId > 0)
            {
                query = query.Where(w => w.WorkFlowTypeId == workFlowTypeId);
            }

            return query.Where(w => w.OrganisationChartId == _currentUserDefaultOrganId);
        }

        private IQueryable<Core.Data.Definition> BuildOrganScopedDefinitionsQuery(bool ignoreExpired, long? workFlowTypeId = null)
        {
            var workflowIds = ScopedWorkFlows(workFlowTypeId).Select(w => w.Id);

            return All(ignoreExpired)
                .Where(d => workflowIds.Contains(d.WorkFlowId))
                .Include(d => d.FromNode)
                .Include(d => d.ToNode)
                .Include(d => d.Action)
                .Include(d => d.WorkFlow);
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
            IQueryable<Core.Data.Definition>? customDataSource = null,
            bool ignoreDefaultOrganId = false)
        {
            var query = customDataSource ?? BuildOrganScopedDefinitionsQuery(ignoreExpired);

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
                customDataSource: BuildOrganScopedDefinitionsQuery(ignoreExpired, workFlowTypeId));
        }

        public OperationResult GetDiagram(long workFlowTypeId, long? workFlowId = null)
        {
            if (workFlowTypeId <= 0)
            {
                return OperationResult.Failed("نوع گردش کار انتخاب نشده است");
            }

            var workFlowType = _unitOfWork.Context.Set<Core.Data.WorkFlowType>().Find(workFlowTypeId);
            if (workFlowType == null)
            {
                return OperationResult.NotFound("نوع گردش کار یافت نشد");
            }

            var workflows = ScopedWorkFlows(workFlowTypeId)
                .OrderByDescending(w => w.IsActive)
                .ThenBy(w => w.title)
                .Select(w => new WorkflowDiagramWorkFlowItemDTO
                {
                    Id = w.Id,
                    Title = w.title,
                    IsActive = w.IsActive
                })
                .ToList();

            if (workflows.Count == 0)
            {
                return OperationResult.Succeeded(payload: new WorkflowDiagramDTO
                {
                    WorkFlowTypeId = workFlowTypeId,
                    WorkFlowTypeTitle = workFlowType.title,
                    WorkFlows = workflows
                });
            }

            var selectedWorkFlowId = workFlowId > 0 && workflows.Any(w => w.Id == workFlowId)
                ? workFlowId.Value
                : workflows[0].Id;

            var definitions = All(true)
                .Where(d => d.WorkFlowId == selectedWorkFlowId)
                .Include(d => d.FromNode)
                .Include(d => d.ToNode)
                .Include(d => d.Action)
                .ToList();

            var nodeIds = definitions
                .SelectMany(d => new long?[] { d.FromNodeId, d.ToNodeId })
                .Where(id => id.HasValue)
                .Select(id => id!.Value)
                .Distinct()
                .ToList();

            var diagramNodes = nodeIds.Count == 0
                ? new List<WorkflowDiagramNodeDTO>()
                : _unitOfWork.Context.Set<Core.Data.Node>()
                    .Where(n => nodeIds.Contains(n.Id) && n.WorkFlowId == selectedWorkFlowId)
                    .OrderBy(n => n.Priority)
                    .ThenBy(n => n.title)
                    .Select(n => new WorkflowDiagramNodeDTO
                    {
                        Id = n.Id,
                        Title = n.title,
                        Description = n.Description,
                        Priority = n.Priority
                    })
                    .ToList();

            foreach (var node in diagramNodes)
            {
                // FromNode خالی = شروع فرآیند → مقصد همان ToNode است
                // ToNode خالی = پایان فرآیند → مبدأ همان FromNode است
                node.IsStart = definitions.Any(d => d.ToNodeId == node.Id && d.FromNodeId == null);
                node.IsEnd = definitions.Any(d => d.FromNodeId == node.Id && d.ToNodeId == null);
            }

            diagramNodes.Insert(0, new WorkflowDiagramNodeDTO
            {
                Id = 0,
                Title = "شروع فرآیند",
                Description = "FromNode خالی — ابتدای فرآیند",
                Priority = -1,
                IsStart = true,
                IsEnd = false
            });
            diagramNodes.Add(new WorkflowDiagramNodeDTO
            {
                Id = -1,
                Title = "پایان فرآیند (گره نهایی)",
                Description = "ToNode خالی — پایان فرآیند",
                Priority = 9999,
                IsStart = false,
                IsEnd = true
            });

            var edges = definitions.Select(d => new WorkflowDiagramEdgeDTO
            {
                Id = d.Id,
                WorkFlowId = d.WorkFlowId,
                FromNodeId = d.FromNodeId,
                ToNodeId = d.ToNodeId,
                FromNodeTitle = d.FromNodeId == null ? "شروع فرآیند" : d.FromNode?.title,
                ToNodeTitle = d.ToNodeId == null ? "پایان فرآیند (گره نهایی)" : d.ToNode?.title,
                ActionId = d.ActionId,
                ActionTitle = d.Action?.title,
                AllowComment = d.AllowComment,
                IsCommentRequired = d.IsCommentRequired,
                NeedSignature = d.NeedSignature,
                IsFinalTransition = d.IsFinalTransition,
                StartDate = d.StartDate?.ToString("yyyy-MM-dd"),
                EndDate = d.EndDate?.ToString("yyyy-MM-dd")
            }).ToList();

            var paletteNodes = _unitOfWork.Context.Set<Core.Data.Node>()
                .Where(n => n.WorkFlowId == selectedWorkFlowId)
                .OrderBy(n => n.Priority)
                .ThenBy(n => n.title)
                .Select(n => new WorkflowDiagramPaletteNodeDTO
                {
                    Id = n.Id,
                    Title = n.title
                })
                .ToList();

            var actions = _unitOfWork.Context.Set<Core.Data.Action>()
                .OrderBy(a => a.title)
                .Select(a => new WorkflowDiagramActionDTO
                {
                    Id = a.Id,
                    Title = a.title
                })
                .ToList();

            return OperationResult.Succeeded(payload: new WorkflowDiagramDTO
            {
                WorkFlowTypeId = workFlowTypeId,
                WorkFlowTypeTitle = workFlowType.title,
                SelectedWorkFlowId = selectedWorkFlowId,
                WorkFlows = workflows,
                Nodes = diagramNodes,
                Edges = edges,
                PaletteNodes = paletteNodes,
                Actions = actions
            });
        }

        private static void NormalizeDefinitionDto(DefinitionDTO dto)
        {
            if (dto.FromNodeId is 0)
            {
                dto.FromNodeId = null;
            }

            if (dto.ToNodeId is null or <= 0)
            {
                dto.ToNodeId = null;
            }
        }

        private OperationResult? ValidateDefinitionDto(DefinitionDTO dto)
        {
            if (dto.WorkFlowId <= 0)
            {
                return OperationResult.Failed("گردش کار انتخاب نشده است");
            }

            if (dto.ActionId <= 0)
            {
                return OperationResult.Failed("عملیات مسیر انتخاب نشده است");
            }

            if (dto.StartDate == null)
            {
                return OperationResult.Failed("تاریخ شروع اعتبار مسیر الزامی است");
            }

            NormalizeDefinitionDto(dto);

            if (dto.FromNodeId == null && dto.ToNodeId == null)
            {
                return OperationResult.Failed("برای مسیر آغازین، گره مقصد (به) را انتخاب کنید");
            }

            if (dto.FromNodeId == null && dto.ToNodeId <= 0)
            {
                return OperationResult.Failed("گره مقصد مسیر آغازین معتبر نیست");
            }

            var nodeValidation = ValidateDefinitionNodes(dto);
            if (nodeValidation != null)
            {
                return nodeValidation;
            }

            if (dto.ActionId == (long)Enums.WorkFlowActions.Reject
                && dto.FromNodeId.HasValue
                && !dto.ToNodeId.HasValue
                && !dto.IsFinalTransition)
            {
                return OperationResult.Failed("برای عملیات رد (بازگشت به مرحله قبل)، گره مقصد را انتخاب کنید یا مسیر را به پایان گردش کار وصل کنید");
            }

            return null;
        }

        private OperationResult? ValidateDefinitionNodes(DefinitionDTO dto)
        {
            var nodeIds = new List<long>();
            if (dto.FromNodeId > 0)
            {
                nodeIds.Add(dto.FromNodeId.Value);
            }

            if (dto.ToNodeId > 0)
            {
                nodeIds.Add(dto.ToNodeId.Value);
            }

            if (nodeIds.Count == 0)
            {
                return null;
            }

            var invalidNode = _unitOfWork.Context.Set<Core.Data.Node>()
                .AsNoTracking()
                .Any(n => nodeIds.Contains(n.Id) && n.WorkFlowId != dto.WorkFlowId);

            if (invalidNode)
            {
                return OperationResult.Failed("گره‌های انتخاب‌شده متعلق به گردش کار انتخاب‌شده نیستند");
            }

            return null;
        }

        public new async Task<OperationResult> CreateForAsync(DefinitionDTO entityToCreate)
        {
            var validation = ValidateDefinitionDto(entityToCreate);
            if (validation != null)
            {
                return validation;
            }

            return await base.CreateForAsync(entityToCreate);
        }

        public new async Task<OperationResult> UpdateForAsync(DefinitionDTO entityToUpdate)
        {
            var validation = ValidateDefinitionDto(entityToUpdate);
            if (validation != null)
            {
                return validation;
            }

            return await base.UpdateForAsync(entityToUpdate);
        }

        public OperationResult GetDefinitionsByInstanceId(long instanceId)
        {
            var instance = _unitOfWork.Context.Set<Core.Data.WorkFlowInstance>()
                .AsNoTracking()
                .FirstOrDefault(i => i.Id == instanceId);
            if (instance == null)
            {
                return OperationResult.NotFound("نمونه گردش کار یافت نشد");
            }

            var actionFromNodeId = ResolveActionFromNodeId(instanceId, instance.LastActivityId);
            if (!actionFromNodeId.HasValue)
            {
                return OperationResult.NotFound("مرحله فعلی گردش کار برای تعیین عملیات یافت نشد");
            }

            // همان منطق DoActionOnInstance: مسیرهایی که FromNodeId برابر گره مبدأ انتظار است
            var definitions = All(true)
                .Where(d => d.WorkFlowId == instance.WorkFlowId && d.FromNodeId == actionFromNodeId)
                .Include(d => d.Action)
                .AsNoTracking()
                .ToList();

            var definitionDTOs = _mapper.Map<List<DefinitionDTO>>(definitions);
            return OperationResult.Succeeded(payload: definitionDTOs);
        }

        private long? ResolveActionFromNodeId(long instanceId, long lastActivityId)
        {
            var lastActivity = _unitOfWork.Context.Set<Core.Data.ActivityTemplate>()
                .AsNoTracking()
                .FirstOrDefault(a => a.Id == lastActivityId);

            if (lastActivity?.ToNodeId is > 0)
            {
                return lastActivity.ToNodeId;
            }

            var pendingActivity = _unitOfWork.Context.Set<Core.Data.ActivityTemplate>()
                .AsNoTracking()
                .Where(a => a.WorkFlowInstanceId == instanceId && a.Pending && a.DoDate == null)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (pendingActivity?.FromNodeId is > 0)
            {
                return pendingActivity.FromNodeId;
            }

            return null;
        }

    }
}
