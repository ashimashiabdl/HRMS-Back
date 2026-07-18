using HR.SharedKernel;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Share;
using HR.WorkFlow.Core.Data;
using HR.WorkFlow.Infrastructure.Data;
using LinqKit;
using Microsoft.EntityFrameworkCore;

namespace HR.WorkFlow.Infrastructure.Services;

/// <summary>
/// ایجاد نمونه گردش کار و قالب فعالیت‌ها (مشترک بین احکام و تسویه حساب).
/// </summary>
public class WorkFlowInstanceStarterService(IUnitOfWork<WorkFlowContext> unitOfWork) : IScopedServices
{
    private readonly WorkFlowContext _db = unitOfWork.Context;

    public OperationResult StartInstance(
        long workFlowId,
        string? createBy,
        long? interdictOrderId = null,
        long? employeeSettlementId = null)
    {
        var hasOrder = interdictOrderId.HasValue && interdictOrderId.Value > 0;
        var hasSettlement = employeeSettlementId.HasValue && employeeSettlementId.Value > 0;

        if (hasOrder == hasSettlement)
        {
            return OperationResult.Failed("شناسه مرجع گردش کار (حکم یا تسویه) نامعتبر است");
        }

        if (workFlowId <= 0)
        {
            return OperationResult.Failed("شناسه گردش کار نامعتبر است");
        }

        var definitions = _db.Set<Definition>()
            .Where(DateValidityExtension<Definition>.GetDateValidationPredicate().And(i => i.WorkFlowId == workFlowId))
            .ToList();

        if (definitions.Count == 0)
        {
            var hasAnyDefinition = _db.Set<Definition>()
                .AsNoTracking()
                .Any(d => d.WorkFlowId == workFlowId && d.IsDeleted != true);

            if (hasAnyDefinition)
            {
                return OperationResult.Failed(
                    "مسیرهای گردش کار منقضی شده‌اند یا هنوز در بازه اعتبار نیستند؛ تاریخ شروع و پایان مسیرها را در تعریف گردش کار بررسی کنید");
            }

            return OperationResult.Failed("تعریف گردش کار برای این فرآیند یافت نشد");
        }

        var starterNode = definitions.SingleOrDefault(i => i.FromNodeId == null);
        if (starterNode == null)
        {
            return OperationResult.Failed("گره شروع گردش کار یافت نشد");
        }

        var instance = new WorkFlowInstance
        {
            CreateDate = DateTime.Now,
            InterdictOrderId = hasOrder ? interdictOrderId : null,
            EmployeeSettlementId = hasSettlement ? employeeSettlementId : null,
            CreateBy = createBy,
            WorkFlowId = workFlowId,
            IPAddress = string.Empty,
            IsDeleted = false,
        };

        _db.Set<WorkFlowInstance>().Add(instance);
        _db.SaveChanges();

        var initial = new ActivityTemplate
        {
            WorkFlowInstanceId = instance.Id,
            FromNodeId = null,
            ToNodeId = starterNode.ToNodeId,
            ActionId = (long)Enums.WorkFlowActions.Approve,
            CreateDate = DateTime.Now,
            IPAddress = string.Empty,
            IsDeleted = false,
            Pending = false,
            LastModifiedDate = DateTime.Now,
            CreatedBy = createBy,
            LastModifiedBy = createBy,
            title = "گره شروع",
            DoDate = DateTime.Now,
        };

        _db.Set<ActivityTemplate>().Add(initial);
        _db.SaveChanges();

        instance.LastActivityId = initial.Id;
        _db.Update(instance);
        _db.SaveChanges();

        var definition = new Definition { Id = starterNode.Id };
        long? toNodeId = starterNode.ToNodeId;
        long? fromNodeId = null;
        long actionId = (long)Enums.WorkFlowActions.Approve;
        var init = true;

        while (definition.ToNodeId != null || init)
        {
            var step = new ActivityTemplate
            {
                WorkFlowInstanceId = instance.Id,
                FromNodeId = fromNodeId,
                ToNodeId = toNodeId,
                ActionId = actionId,
                CreateDate = DateTime.Now,
                IPAddress = string.Empty,
                IsDeleted = false,
                Pending = init,
                LastModifiedDate = DateTime.Now,
                title = definition.Id.ToString(),
            };

            if (definition.Id != starterNode.Id)
            {
                _db.Set<ActivityTemplate>().Add(step);
                _db.SaveChanges();
                init = false;
            }

            var nextDefinition = definitions.SingleOrDefault(i => i.FromNodeId == toNodeId && i.ActionId == 1);
            if (nextDefinition == null)
            {
                break;
            }

            definition = nextDefinition;
            actionId = definition.ActionId;
            fromNodeId = definition.FromNodeId;
            toNodeId = definition.ToNodeId;
        }

        var lastNode = definitions.SingleOrDefault(i => i.ToNodeId == null && i.ActionId == 1);
        if (lastNode != null)
        {
            var lastActivityTemplate = new ActivityTemplate
            {
                WorkFlowInstanceId = instance.Id,
                FromNodeId = lastNode.FromNodeId,
                ToNodeId = lastNode.ToNodeId,
                ActionId = (long)Enums.WorkFlowActions.Approve,
                CreateDate = DateTime.Now,
                IPAddress = string.Empty,
                IsDeleted = false,
                Pending = false,
                LastModifiedDate = DateTime.Now,
                title = "گره پایان",
                IsFinalTransition = true,
            };

            _db.Set<ActivityTemplate>().Add(lastActivityTemplate);
            _db.SaveChanges();
        }

        return OperationResult.Succeeded("گردش کار ایجاد شد", payload: instance.Id);
    }

    public long? ResolveActiveWorkFlowId(long workFlowTypeId, long organisationChartId)
    {
        return FindWorkFlowWithValidDefinitions(workFlowTypeId, organisationChartId);
    }

    /// <summary>
    /// همان ترتیب انتخاب طراح بصری (GetDiagram): فعال اول، سپس گردش کاری که مسیر معتبر دارد.
    /// </summary>
    private long? FindWorkFlowWithValidDefinitions(long workFlowTypeId, long organisationChartId)
    {
        if (workFlowTypeId <= 0 || organisationChartId <= 0)
        {
            return null;
        }

        var datePredicate = DateValidityExtension<Definition>.GetDateValidationPredicate();

        var candidateIds = _db.Set<Core.Data.WorkFlow>()
            .AsNoTracking()
            .Where(w => w.WorkFlowTypeId == workFlowTypeId
                && w.OrganisationChartId == organisationChartId
                && !w.IsDeleted)
            .OrderByDescending(w => w.IsActive)
            .ThenBy(w => w.title)
            .ThenByDescending(w => w.Id)
            .Select(w => w.Id)
            .ToList();

        foreach (var wfId in candidateIds)
        {
            var hasValidDefinitions = _db.Set<Definition>()
                .AsNoTracking()
                .Any(datePredicate.And(d => d.WorkFlowId == wfId));

            if (hasValidDefinitions)
            {
                return wfId;
            }
        }

        return null;
    }
}
