using AutoMapper;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HR.WorkFlow.Core.DTOs;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using HR.SharedKernel;
using System.Text;
using System.Threading.Tasks;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using HR.WorkFlow.Core.Data;
using HR.SharedKernel.Share;
using HR.Order.Core.Data;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Core.Helpers;
using Microsoft.EntityFrameworkCore;

namespace HR.WorkFlow.Infrastructure.Services
{
    public class WorkFlowInstanceService(
        IMapper mapper,
        UserResolverService UserResolverService,
        IUnitOfWork<WorkFlowContext> unitOfWork,
        IDapper dapper,
        IConfiguration configuration,
        UserResolverService userService,
        OrderService orderService,
        ILogger<WorkFlowInstanceService> logger
    ) : BaseService<HR.WorkFlow.Core.Data.WorkFlowInstance, WorkFlowContext, WorkFlowInstanceDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
    {
        private readonly UserResolverService _userService = UserResolverService;
        private readonly OrderService _orderService = orderService;
        private readonly ILogger<WorkFlowInstanceService> _logger = logger;
        private readonly WorkFlowContext _workFlowDb = unitOfWork.Context;

        private const int MaxBatchActionCount = 200;

        private static bool IsForwardAction(int actionId) =>
            actionId == (long)Enums.WorkFlowActions.Approve
            || actionId == (long)Enums.WorkFlowActions.ApproveOnBehalf
            || actionId == (long)Enums.WorkFlowActions.Sign;

        private static bool IsReturnRejectAction(int actionId) =>
            actionId == (long)Enums.WorkFlowActions.Reject;

        private static bool IsFullRejectAction(int actionId) =>
            actionId == (long)Enums.WorkFlowActions.FullReject;

        /// <summary>
        /// Reject that returns the instance to a previous node (requires ToNodeId).
        /// </summary>
        private static bool IsReturnToPreviousReject(Definition definition, int actionId) =>
            IsReturnRejectAction(actionId) && definition.ToNodeId is > 0;

        /// <summary>
        /// Terminal rejection — ends the workflow unsuccessfully (FullReject, or Reject to process end with no destination node).
        /// </summary>
        private static bool IsTerminalReject(Definition definition, int actionId) =>
            IsFullRejectAction(actionId)
            || (IsReturnRejectAction(actionId) && definition.IsFinalTransition && !definition.ToNodeId.HasValue);

        private sealed record ReturnRejectResolution(ActivityTemplate WaitingActivity, long LastActivityId);

        private ReturnRejectResolution ResolveReturnRejectPendingActivity(
            long instanceId,
            IQueryable<ActivityTemplate> relatedActivities,
            IQueryable<Definition> definistion,
            ActivityTemplate relatedTransition,
            Definition newActivityDef)
        {
            if (!newActivityDef.ToNodeId.HasValue || newActivityDef.ToNodeId.Value <= 0)
            {
                throw new InvalidOperationException("گره مقصد برای عملیات رد تعریف نشده است");
            }

            var returnToNodeId = newActivityDef.ToNodeId.Value;

            foreach (var openPending in relatedActivities
                         .Where(a => a.Pending && a.DoDate == null && a.Id != relatedTransition.Id)
                         .ToList())
            {
                openPending.Pending = false;
                _db.Update(openPending);
            }

            var waitingAtReturnNode = relatedActivities
                .Where(a => a.ToNodeId == returnToNodeId && a.Id != relatedTransition.Id)
                .OrderByDescending(a => a.Id)
                .FirstOrDefault();

            if (waitingAtReturnNode != null)
            {
                waitingAtReturnNode.Pending = true;
                waitingAtReturnNode.DoDate = null;
                waitingAtReturnNode.LastModifiedDate = DateTime.Now;
                waitingAtReturnNode.IPAddress = _userService.GetIP();
                _db.Update(waitingAtReturnNode);
            }
            else
            {
                var approveIncoming = definistion
                    .FirstOrDefault(d => d.ToNodeId == returnToNodeId && d.ActionId == (long)Enums.WorkFlowActions.Approve);

                waitingAtReturnNode = new ActivityTemplate
                {
                    WorkFlowInstanceId = instanceId,
                    FromNodeId = approveIncoming?.FromNodeId,
                    ToNodeId = returnToNodeId,
                    ActionId = (long)Enums.WorkFlowActions.Approve,
                    CreateDate = DateTime.Now,
                    LastModifiedDate = DateTime.Now,
                    IPAddress = _userService.GetIP(),
                    IsDeleted = false,
                    Pending = true,
                    IsFinalTransition = false,
                    title = newActivityDef.Id.ToString()
                };
                _db.Set<ActivityTemplate>().Add(waitingAtReturnNode);
                _unitOfWork.Context.SaveChanges();
            }

            EnsureForwardTransitionFromNode(
                instanceId,
                relatedActivities,
                definistion,
                returnToNodeId,
                relatedTransition.Id);

            var incomingActivity = waitingAtReturnNode.FromNodeId is > 0
                ? relatedActivities
                    .Where(a => a.ToNodeId == waitingAtReturnNode.FromNodeId && a.Id != relatedTransition.Id)
                    .OrderByDescending(a => a.Id)
                    .FirstOrDefault()
                : null;

            var lastActivityId = incomingActivity?.Id ?? waitingAtReturnNode.Id;
            return new ReturnRejectResolution(waitingAtReturnNode, lastActivityId);
        }

        private void EnsureForwardTransitionFromNode(
            long instanceId,
            IQueryable<ActivityTemplate> relatedActivities,
            IQueryable<Definition> definistion,
            long fromNodeId,
            long excludeActivityId)
        {
            var hasOpenForward = relatedActivities
                .Any(a => a.FromNodeId == fromNodeId && a.DoDate == null && a.Id != excludeActivityId);
            if (hasOpenForward)
            {
                return;
            }

            var forwardDef = definistion
                .FirstOrDefault(d => d.FromNodeId == fromNodeId && d.ActionId == (long)Enums.WorkFlowActions.Approve);
            if (forwardDef == null)
            {
                return;
            }

            var forward = new ActivityTemplate
            {
                WorkFlowInstanceId = instanceId,
                FromNodeId = fromNodeId,
                ToNodeId = forwardDef.ToNodeId,
                ActionId = (long)Enums.WorkFlowActions.Approve,
                CreateDate = DateTime.Now,
                LastModifiedDate = DateTime.Now,
                IPAddress = _userService.GetIP(),
                IsDeleted = false,
                Pending = false,
                IsFinalTransition = forwardDef.IsFinalTransition || !forwardDef.ToNodeId.HasValue,
                title = forwardDef.Id.ToString()
            };
            _db.Set<ActivityTemplate>().Add(forward);
            _unitOfWork.Context.SaveChanges();
        }

        private void ClearOpenPendingActivities(IQueryable<ActivityTemplate> relatedActivities, long? exceptActivityId = null)
        {
            foreach (var openPending in relatedActivities
                         .Where(a => a.Pending && a.DoDate == null && (!exceptActivityId.HasValue || a.Id != exceptActivityId.Value))
                         .ToList())
            {
                openPending.Pending = false;
                _db.Update(openPending);
            }
        }

        public OperationResult GetCartable(long userId, string? filter = null, DateTime? fromDate = null, DateTime? toDate = null, long? workFlowTypeId = null)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[wf].[GetCartable]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userId);

                    var filterParam = cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 100);
                    filterParam.Value = string.IsNullOrWhiteSpace(filter) ? (object)DBNull.Value : filter;

                    var fromDateParam = cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    fromDateParam.Value = fromDate.HasValue ? fromDate.Value : (object)DBNull.Value;

                    var toDateParam = cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    toDateParam.Value = toDate.HasValue ? toDate.Value : (object)DBNull.Value;

                    var workFlowTypeIdParam = cmd.Parameters.Add("@WorkFlowTypeId", SqlDbType.BigInt);
                    workFlowTypeIdParam.Value = workFlowTypeId.HasValue && workFlowTypeId.Value > 0
                        ? workFlowTypeId.Value
                        : (object)DBNull.Value;

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    List<GetCartable_Result> ret = new();
                    int index = 1;
                    while (rdr.Read())
                    {
                        var row = rdr.ConvertToObject<GetCartable_Result>();
                        row.Index = index;
                        ret.Add(row);
                        index++;
                    }
                    con.Close();
                    return OperationResult.Succeeded(payload: ret);
                }
            }
        }

        public OperationResult GetCartableArchive(long userId, string? filter = null, DateTime? fromDate = null, DateTime? toDate = null)
        {
            using (SqlConnection con = new SqlConnection(_connectionString))
            {
                using (SqlCommand cmd = new SqlCommand("[wf].[GetCartableArchive]", con))
                {
                    cmd.CommandType = CommandType.StoredProcedure;

                    cmd.Parameters.AddWithValue("@UserId", userId);

                    var filterParam = cmd.Parameters.Add("@Filter", SqlDbType.NVarChar, 100);
                    filterParam.Value = string.IsNullOrWhiteSpace(filter) ? (object)DBNull.Value : filter;

                    var fromDateParam = cmd.Parameters.Add("@FromDate", SqlDbType.DateTime);
                    fromDateParam.Value = fromDate.HasValue ? fromDate.Value : (object)DBNull.Value;

                    var toDateParam = cmd.Parameters.Add("@ToDate", SqlDbType.DateTime);
                    toDateParam.Value = toDate.HasValue ? toDate.Value : (object)DBNull.Value;

                    con.Open();
                    SqlDataReader rdr = cmd.ExecuteReader();
                    List<GetCartableArchive_Result> ret = new();
                    int index = 1;
                    while (rdr.Read())
                    {
                        var row = rdr.ConvertToObject<GetCartableArchive_Result>();
                        row.Index = index;
                        ret.Add(row);
                        index++;
                    }
                    con.Close();
                    return OperationResult.Succeeded(payload: ret);
                }
            }
        }

        #region DoActionOnInstance

        public OperationResult DoActionOnInstance(int ActionId, long InstanceId, string? comment = null, long? userSignatureId = null)
        {
            try
            {
                var normalizedComment = string.IsNullOrWhiteSpace(comment) ? null : comment.Trim();
                _logger.LogInformation(
                    "Starting workflow action. ActionId={ActionId}, InstanceId={InstanceId}, HasComment={HasComment}",
                    ActionId,
                    InstanceId,
                    !string.IsNullOrEmpty(normalizedComment));

                _unitOfWork.CreateTransaction();
                var instance = GetIdAsync(InstanceId).Result;
                if (instance == null)
                {
                    _logger.LogWarning("Workflow instance not found. InstanceId={InstanceId}", InstanceId);
                    _unitOfWork.Rollback();
                    return OperationResult.NotFound("گردش کار مورد نظر یافت نشد");
                }

                var definistion = _db.Set<Definition>().Where(i => i.WorkFlowId == instance.WorkFlowId);
                var lastActivity = _db.Set<ActivityTemplate>().Find(instance.LastActivityId);
                if (lastActivity == null)
                {
                    _logger.LogWarning("Last activity not found. InstanceId={InstanceId}, LastActivityId={LastActivityId}", InstanceId, instance.LastActivityId);
                    _unitOfWork.Rollback();
                    return OperationResult.NotFound("آخرین فعالیت یافت نشد");
                }
                else
                {


                    if (definistion.Any(i => i.ActionId == ActionId && i.FromNodeId == lastActivity.ToNodeId))
                    {
                        var newActivityDef = definistion.Single(i => i.ActionId == ActionId && i.FromNodeId == lastActivity.ToNodeId);
                        
                        // اگر ActionId = امضا است، باید UserSignature ارسال شده باشد
                        if (ActionId == (long)Enums.WorkFlowActions.Sign)
                        {
                            if (!userSignatureId.HasValue || userSignatureId.Value <= 0)
                            {
                                _logger.LogWarning("Sign action without UserSignature. InstanceId={InstanceId}, ActionId={ActionId}", InstanceId, ActionId);
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("انتخاب نقش امضا برای عملیات امضا الزامی است.");
                            }

                            // بررسی اینکه UserSignature مربوط به کاربر فعلی و سازمان فعلی است
                            var userSignature = _db.Set<Core.Data.UserSignature>().Find(userSignatureId.Value);
                            if (userSignature == null)
                            {
                                _logger.LogWarning("UserSignature not found. InstanceId={InstanceId}, UserSignatureId={UserSignatureId}", InstanceId, userSignatureId.Value);
                                _unitOfWork.Rollback();
                                return OperationResult.NotFound("نقش امضا یافت نشد.");
                            }

                            var currentUserId = _userService.GetUserId();
                            if (userSignature.AspNetUsersId != currentUserId || userSignature.OrganisationChartId != _currentUserDefaultOrganId)
                            {
                                _logger.LogWarning("UserSignature does not belong to current user or organization. InstanceId={InstanceId}, UserSignatureId={UserSignatureId}, CurrentUserId={CurrentUserId}, UserSignatureUserId={UserSignatureUserId}, CurrentOrgId={CurrentOrgId}, UserSignatureOrgId={UserSignatureOrgId}", 
                                    InstanceId, userSignatureId.Value, currentUserId, userSignature.AspNetUsersId, _currentUserDefaultOrganId, userSignature.OrganisationChartId);
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("نقش امضا انتخابی متعلق به شما نیست یا متعلق به سازمان فعلی شما نیست.");
                            }

                            // بررسی اینکه UserSignature فعال است
                            if (!userSignature.Enabled)
                            {
                                _logger.LogWarning("UserSignature is disabled. InstanceId={InstanceId}, UserSignatureId={UserSignatureId}", InstanceId, userSignatureId.Value);
                                _unitOfWork.Rollback();
                                return OperationResult.Failed("نقش امضا انتخابی غیرفعال است.");
                            }
                        }

                        if ((IsReturnRejectAction(ActionId) || IsFullRejectAction(ActionId))
                            && (newActivityDef?.IsCommentRequired ?? false)
                            && string.IsNullOrWhiteSpace(normalizedComment))
                        {
                            _logger.LogWarning("Reject action without required comment. InstanceId={InstanceId}, DefinitionId={DefinitionId}", InstanceId, newActivityDef.Id);
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("ثبت توضیح برای رد این مرحله الزامی است.");
                        }

                        if (IsReturnRejectAction(ActionId)
                            && !newActivityDef.ToNodeId.HasValue
                            && !newActivityDef.IsFinalTransition)
                        {
                            _logger.LogWarning(
                                "Reject definition missing destination node. InstanceId={InstanceId}, DefinitionId={DefinitionId}, FromNodeId={FromNodeId}",
                                InstanceId,
                                newActivityDef.Id,
                                newActivityDef.FromNodeId);
                            _unitOfWork.Rollback();
                            return OperationResult.Failed("گره مقصد برای عملیات رد تعریف نشده است");
                        }

                        var relatedActivities = _db.Set<ActivityTemplate>().Where(i => i.WorkFlowInstanceId == InstanceId);
                        var openTransitions = relatedActivities
                            .Where(i => i.FromNodeId == lastActivity.ToNodeId && i.DoDate == null);
                        var newTransition = openTransitions.Any()
                            ? openTransitions
                            : relatedActivities.Where(i => i.FromNodeId == lastActivity.ToNodeId);
                        if (newTransition == null)
                        {
                            _logger.LogWarning("No transition found from last activity. InstanceId={InstanceId}, FromNodeId={FromNodeId}", InstanceId, lastActivity.ToNodeId);
                            _unitOfWork.Rollback();
                            return OperationResult.NotFound("مسیر گردش کار به گره بعدی تعریف نشده است");
                        }
                        else
                        {
                            if (newTransition.Any())
                            {
                                try
                                {

                                    var relatedTransition = newTransition.Single();

                                    // اگر ActionId انتخاب شده با ActionId پیش‌فرض (1) متفاوت باشد، Transition را به‌روزرسانی کن
                                    if (relatedTransition.ActionId != ActionId)
                                    {
                                        _logger.LogInformation("Updating transition ActionId from {OldActionId} to {NewActionId}. InstanceId={InstanceId}, TransitionId={TransitionId}", 
                                            relatedTransition.ActionId, ActionId, InstanceId, relatedTransition.Id);
                                        
                                        relatedTransition.ActionId = ActionId;
                                        relatedTransition.ToNodeId = newActivityDef.ToNodeId;
                                    }

                                    if (IsReturnRejectAction(ActionId) || IsFullRejectAction(ActionId))
                                    {
                                        relatedTransition.IsFinalTransition = newActivityDef.IsFinalTransition
                                            || !newActivityDef.ToNodeId.HasValue;
                                    }
                                    
                                    ActivityTemplate pendingActivity;
                                    long? returnRejectLastActivityId = null;

                                    if (IsReturnToPreviousReject(newActivityDef, ActionId))
                                    {
                                        var returnResolution = ResolveReturnRejectPendingActivity(
                                            InstanceId,
                                            relatedActivities,
                                            definistion,
                                            relatedTransition,
                                            newActivityDef);
                                        pendingActivity = returnResolution.WaitingActivity;
                                        returnRejectLastActivityId = returnResolution.LastActivityId;
                                    }
                                    else
                                    {
                                        var newPending = relatedActivities.Where(i => i.FromNodeId == relatedTransition.ToNodeId);

                                        if (newPending.Any())
                                        {
                                            pendingActivity = newPending.Single();
                                        }
                                        else if (!IsForwardAction(ActionId))
                                        {
                                            // مسیرهای جایگزین (امضا و ...) هنگام ایجاد نمونه گردش کار فقط در مسیر تایید ساخته می‌شوند.
                                            long? nextToNodeId = null;
                                            if (!newActivityDef.IsFinalTransition && newActivityDef.ToNodeId.HasValue)
                                            {
                                                var approveDefFromDestination = definistion
                                                    .FirstOrDefault(d => d.FromNodeId == relatedTransition.ToNodeId && d.ActionId == (long)Enums.WorkFlowActions.Approve);
                                                nextToNodeId = approveDefFromDestination?.ToNodeId;
                                            }

                                            pendingActivity = new ActivityTemplate
                                            {
                                                WorkFlowInstanceId = InstanceId,
                                                FromNodeId = relatedTransition.ToNodeId,
                                                ToNodeId = newActivityDef.IsFinalTransition || !newActivityDef.ToNodeId.HasValue ? null : nextToNodeId,
                                                ActionId = ActionId,
                                                CreateDate = DateTime.Now,
                                                LastModifiedDate = DateTime.Now,
                                                IPAddress = _userService.GetIP(),
                                                IsDeleted = false,
                                                Pending = true,
                                                IsFinalTransition = newActivityDef.IsFinalTransition || !newActivityDef.ToNodeId.HasValue,
                                                title = newActivityDef.Id.ToString()
                                            };
                                            _db.Set<ActivityTemplate>().Add(pendingActivity);
                                            _unitOfWork.Context.SaveChanges();
                                        }
                                        else
                                        {
                                            _logger.LogWarning("Pending node sequence is empty. InstanceId={InstanceId}, ToNodeId={ToNodeId}", InstanceId, relatedTransition.ToNodeId);
                                            _unitOfWork.Rollback();
                                            return OperationResult.NotFound("گره در انتظار جدیدی یافت نشد");
                                        }

                                        if (IsTerminalReject(newActivityDef, ActionId))
                                        {
                                            ClearOpenPendingActivities(relatedActivities, pendingActivity.Id);
                                            pendingActivity.Pending = false;
                                        }
                                        else
                                        {
                                            pendingActivity.Pending = true;
                                        }

                                        _db.Update(pendingActivity);
                                        _unitOfWork.Context.SaveChanges();
                                    }

                                    relatedTransition.Pending = false;
                                    relatedTransition.DoDate = DateTime.Now;
                                    if (!string.IsNullOrEmpty(normalizedComment))
                                    {
                                        relatedTransition.Comment = normalizedComment;
                                    }
                                    if (ActionId == (long)Enums.WorkFlowActions.Sign && userSignatureId.HasValue)
                                    {
                                        relatedTransition.UserSignatureId = userSignatureId.Value;
                                    }

                                    relatedTransition.LastModifiedDate = DateTime.Now;
                                    relatedTransition.IPAddress = _userService.GetIP();
                                    if (!string.IsNullOrEmpty(normalizedComment))
                                    {
                                        _unitOfWork.Context.Entry(relatedTransition).Property(a => a.Comment).IsModified = true;
                                    }

                                    instance.LastActivityId = returnRejectLastActivityId ?? relatedTransition.Id;
                                    _unitOfWork.Context.SaveChanges();
                                    Update(instance);
                                    _unitOfWork.Context.SaveChanges();

                                    if (pendingActivity.IsFinalTransition && IsForwardAction(ActionId))
                                    {
                                        if (instance.EmployeeSettlementId.HasValue && instance.EmployeeSettlementId.Value > 0)
                                        {
                                            var approveSettlementResult = ApplyFinalApproveToPendingSettlement(instance.EmployeeSettlementId.Value);
                                            if (!approveSettlementResult.Success)
                                            {
                                                _unitOfWork.Rollback();
                                                return approveSettlementResult;
                                            }
                                        }
                                        else if (instance.InterdictOrderId.HasValue && instance.InterdictOrderId.Value > 0)
                                        {
                                            var approveResult = _orderService.ApplyFinalApproveToPendingOrder(
                                                instance.InterdictOrderId.Value,
                                                _userService.GetUserId(),
                                                _unitOfWork.Context);
                                            if (!approveResult.Success)
                                            {
                                                _unitOfWork.Rollback();
                                                return approveResult;
                                            }
                                        }

                                        _unitOfWork.Context.SaveChanges();
                                    }

                                    if (IsTerminalReject(newActivityDef, ActionId))
                                    {
                                        if (instance.EmployeeSettlementId.HasValue && instance.EmployeeSettlementId.Value > 0)
                                        {
                                            var rejectSettlementResult = ApplyRejectToPendingSettlement(instance.EmployeeSettlementId.Value);
                                            if (!rejectSettlementResult.Success)
                                            {
                                                _unitOfWork.Rollback();
                                                return rejectSettlementResult;
                                            }
                                        }
                                        else if (instance.InterdictOrderId.HasValue && instance.InterdictOrderId.Value > 0)
                                        {
                                            var interdictOrder = _db.Set<InterdictOrder>().Find(instance.InterdictOrderId.Value);
                                            var rejectResult = _orderService.ApplyRejectToPendingOrder(interdictOrder, _unitOfWork.Context);
                                            if (!rejectResult.Success)
                                            {
                                                _unitOfWork.Rollback();
                                                return rejectResult;
                                            }

                                            _db.Set<InterdictOrder>().Update(interdictOrder!);
                                        }

                                        _unitOfWork.Context.SaveChanges();
                                    }

                                    _unitOfWork.Commit();

                                    return OperationResult.Succeeded();
                                }
                                catch (Exception ex)
                                {
                                    _unitOfWork.Rollback();
                                    _logger.LogError(ex, "Error during workflow transaction. ActionId={ActionId}, InstanceId={InstanceId}", ActionId, InstanceId);
                                    return OperationResult.Failed("خطا در انجام عملیات گردش کار. لطفا مجددا تلاش کنید یا با پشتیبانی تماس بگیرید.");
                                }
                            }
                            else
                            {
                                _logger.LogWarning("Transition sequence is empty. InstanceId={InstanceId}, FromNodeId={FromNodeId}", InstanceId, lastActivity.ToNodeId);
                                _unitOfWork.Rollback();
                                return OperationResult.NotFound("مسیر گردش کار به گره بعدی تعریف نشده است");
                            }
                        }
                    }
                    else
                    {
                        _logger.LogWarning("No workflow definition found for requested action. ActionId={ActionId}, InstanceId={InstanceId}, FromNodeId={FromNodeId}", ActionId, InstanceId, lastActivity.ToNodeId);
                        _unitOfWork.Rollback();
                        return OperationResult.NotFound("تعریفی برای عملیات درخواستی یافت نشد");
                    }
                }
          
            }
            catch (Exception ex)
            {
                _unitOfWork.Rollback();
                _logger.LogError(ex, "Unexpected error in DoActionOnInstance. ActionId={ActionId}, InstanceId={InstanceId}", ActionId, InstanceId);
                return OperationResult.Failed("خطای غیرمنتظره در انجام عملیات گردش کار رخ داد");
            }
        }

        public OperationResult DoActionOnInstancesBatch(WorkFlowInstanceBatchActionRequestDto request)
        {
            if (request == null || request.InstanceIds == null || request.InstanceIds.Count == 0)
            {
                return OperationResult.Failed("لیست شناسه نمونه گردش کار خالی است");
            }

            if (request.ActionId <= 0)
            {
                return OperationResult.Failed("عملیات انتخاب‌شده معتبر نیست");
            }

            var distinctIds = request.InstanceIds.Where(id => id > 0).Distinct().ToList();
            if (distinctIds.Count == 0)
            {
                return OperationResult.Failed("شناسه‌های ارسالی معتبر نیستند");
            }

            if (distinctIds.Count > MaxBatchActionCount)
            {
                return OperationResult.Failed(
                    $"حداکثر {MaxBatchActionCount} رکورد را می‌توانید در یک درخواست عملیات گروهی ارسال کنید");
            }

            var items = new List<WorkFlowInstanceBatchActionItemResultDto>();

            foreach (var instanceId in distinctIds)
            {
                try
                {
                    var singleResult = DoActionOnInstance(
                        request.ActionId,
                        instanceId,
                        request.Comment,
                        request.UserSignatureId);

                    items.Add(new WorkFlowInstanceBatchActionItemResultDto
                    {
                        InstanceId = instanceId,
                        Success = singleResult.Success,
                        Message = string.IsNullOrWhiteSpace(singleResult.Message)
                            ? (singleResult.Success ? "عملیات با موفقیت انجام شد" : "عملیات ناموفق بود")
                            : singleResult.Message
                    });
                }
                catch (Exception ex)
                {
                    _logger.LogError(
                        ex,
                        "Batch workflow action failed for instance. ActionId={ActionId}, InstanceId={InstanceId}",
                        request.ActionId,
                        instanceId);

                    items.Add(new WorkFlowInstanceBatchActionItemResultDto
                    {
                        InstanceId = instanceId,
                        Success = false,
                        Message = "خطای غیرمنتظره در پردازش این رکورد"
                    });
                }
            }

            var summary = new WorkFlowInstanceBatchActionResultDto
            {
                TotalCount = items.Count,
                SuccessCount = items.Count(i => i.Success),
                FailedCount = items.Count(i => !i.Success),
                Items = items
            };

            var message = summary.FailedCount == 0
                ? $"عملیات گروهی برای {summary.SuccessCount} رکورد با موفقیت انجام شد."
                : $"از {summary.TotalCount} رکورد، {summary.SuccessCount} مورد موفق و {summary.FailedCount} مورد ناموفق بود.";

            return summary.FailedCount == 0
                ? OperationResult.Succeeded(message, summary)
                : OperationResult.Failed(message, summary);
        }

        #endregion

        private OperationResult ApplyFinalApproveToPendingSettlement(long settlementId)
        {
            var settlement = _workFlowDb.EmployeeSettlements.Find(settlementId);
            var result = EmployeeSettlementWorkflowStatus.ApplyFinalApprove(settlement);
            if (result.Success && settlement != null)
            {
                _workFlowDb.EmployeeSettlements.Update(settlement);
            }

            return result;
        }

        private OperationResult ApplyRejectToPendingSettlement(long settlementId)
        {
            var settlement = _workFlowDb.EmployeeSettlements.Find(settlementId);
            var result = EmployeeSettlementWorkflowStatus.ApplyReject(settlement);
            if (result.Success && settlement != null)
            {
                _workFlowDb.EmployeeSettlements.Update(settlement);
            }

            return result;
        }

        public new OperationResult GetAsKeyValuePair()
        {
            return OperationResult.Succeeded(payload: All()
                .OrderByDescending(i => i.Id)
                .Select(i => new HR.SharedKernel.Data.KeyValuePair
                {
                    key = i.Id,
                    value = $"#{i.Id} / گردش کار {i.WorkFlowId}"
                }));
        }

        public new async Task<OperationResult> CreateForAsync(WorkFlowInstanceDTO entityToCreate)
        {
            var mapped = _mapper.Map<Core.Data.WorkFlowInstance>(entityToCreate);
            if (!Validate(mapped))
            {
                return OperationResult.Failed(
                    "اطلاعات نمونه گردش کار نامعتبر است. گردش کار الزامی است، شناسه آخرین فعالیت نباید منفی باشد و دقیقاً یکی از شناسه حکم یا تسویه حساب باید مقدار داشته باشد.");
            }

            return await base.CreateForAsync(entityToCreate);
        }

        public new async Task<OperationResult> UpdateForAsync(WorkFlowInstanceDTO entityToUpdate)
        {
            var mapped = _mapper.Map<Core.Data.WorkFlowInstance>(entityToUpdate);
            if (!Validate(mapped))
            {
                return OperationResult.Failed(
                    "اطلاعات نمونه گردش کار نامعتبر است. گردش کار الزامی است، شناسه آخرین فعالیت نباید منفی باشد و دقیقاً یکی از شناسه حکم یا تسویه حساب باید مقدار داشته باشد.");
            }

            return await base.UpdateForAsync(entityToUpdate);
        }

        public bool Validate(Core.Data.WorkFlowInstance entity, object etc = null)
        {
            if (entity == null)
            {
                return false;
            }

            if (entity.WorkFlowId <= 0)
            {
                return false;
            }

            var hasOrder = entity.InterdictOrderId.HasValue && entity.InterdictOrderId.Value > 0;
            var hasSettlement = entity.EmployeeSettlementId.HasValue && entity.EmployeeSettlementId.Value > 0;
            if (hasOrder == hasSettlement)
            {
                return false;
            }

            if (entity.LastActivityId < 0)
            {
                return false;
            }

            return true;
        }
    }
}
