using System.Globalization;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.Order.Core.Data;
using HR.Order.Infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// Order.Interdict_Order import — Upsert by Code، یا RecruitOrderId + Serial، یا RecruitOrderId + OrderTypeId + StartDate.
/// پیش‌نیاز: RecruitOrder برای کارمند + محل پرداخت.
/// </summary>
public class InterdictOrderImportHandler(
    IUnitOfWork<OrderContext> orderUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(InterdictOrder);

    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName", "InterdictOrderCode", "InterdictOrderId", "RecruitOrderId"
    };

    public bool CanHandle(string targetEntityName) =>
        string.Equals(targetEntityName, TargetEntity, StringComparison.OrdinalIgnoreCase);

    public async Task ValidateAndResolveRowsAsync(BaseInfoContext context, ImportProfile profile, List<ImportTempRow> tempRows)
    {
        var (mode, importCtx, organId) = await EmployeeLinkedImportHelper.LoadBatchImportContextAsync(context, contextService, tempRows);
        if (importCtx == null || organId <= 0)
        {
            EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, "اطلاعات تکمیلی Import نامعتبر است.");
            return;
        }

        if (mode == ImportContextMode.BatchContext)
        {
            var contextError = ImportContextRowHelper.ValidateRequiredBatchContextFields(profile, importCtx);
            if (contextError != null)
            {
                EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, contextError);
                return;
            }
        }

        await OrderImportShared.ResolveEmployeeRowsAllowingDuplicateNationalNoAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException("ContextJson برای InterdictOrder نامعتبر است.");

        var organisationChartId = contextService.ReadOrganisationChartId(importCtx);
        var profile = batch.ImportProfile ?? throw new InvalidOperationException("Import profile not loaded.");
        var contextFields = contextService.GetActiveContextFields(profile);
        var profileFieldNames = profile.Fields
            .Where(f => !f.IsDeleted)
            .Select(f => f.TargetPropertyName)
            .ToHashSet(StringComparer.OrdinalIgnoreCase);

        var validRows = await context.ImportTempRows
            .Where(r => r.ImportBatchId == batch.Id
                        && !r.IsDeleted
                        && r.ValidationStatus == ImportValidationStatus.Valid
                        && r.MainRecordId == null)
            .ToListAsync();

        if (validRows.Count == 0)
            return 0;

        var orderCtx = orderUnitOfWork.Context;
        var entityType = typeof(InterdictOrder);
        var now = DateTime.Now;
        var upserted = 0;

        foreach (var row in validRows)
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
            {
                OrderImportShared.MarkRowError(row, "شناسه کارمند نامعتبر است.");
                continue;
            }

            var rowContext = ImportContextRowHelper.MergeForRow(batch, profile, resolved, contextService);
            var payLocationId = OrderImportShared.ResolvePayLocationId(rowContext, organisationChartId);

            var recruit = await OrderImportShared.FindRecruitOrderAsync(orderCtx, employeeId, payLocationId);
            if (recruit == null)
            {
                OrderImportShared.MarkRowError(row, "حکم استخدامی (RecruitOrder) برای این کارمند و محل پرداخت یافت نشد. ابتدا RecruitOrder را Import کنید.");
                continue;
            }

            var orderTypeId = OrderImportShared.ReadNullableLong(rowContext, resolved, "OrderTypeId");
            var statusId = OrderImportShared.ReadNullableLong(rowContext, resolved, "StatusId");
            var issueTypeId = OrderImportShared.ReadNullableLong(rowContext, resolved, "IssueTypeId");
            var insuranceTypeId = OrderImportShared.ReadNullableLong(rowContext, resolved, "InsuranceTypeId");

            if (orderTypeId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "نوع حکم الزامی است.");
                continue;
            }
            if (statusId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "وضعیت حکم الزامی است.");
                continue;
            }
            if (issueTypeId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "نوع صدور حکم الزامی است.");
                continue;
            }
            if (insuranceTypeId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "نوع بیمه الزامی است.");
                continue;
            }

            DateTime? startDate = null;
            if (rowContext.TryGetValue("StartDate", out var startRaw) && !string.IsNullOrWhiteSpace(startRaw))
                startDate = ImportDateParser.ParseNullable(startRaw);
            if (startDate == null && resolved.TryGetValue("StartDate", out var startRow) && !string.IsNullOrWhiteSpace(startRow))
                startDate = ImportDateParser.ParseNullable(startRow);
            if (startDate == null)
            {
                OrderImportShared.MarkRowError(row, "تاریخ اجرای حکم (StartDate) الزامی است.");
                continue;
            }

            var existing = await FindExistingAsync(orderCtx, recruit.Id, resolved, rowContext, orderTypeId.Value, startDate.Value);
            var isUpdate = existing != null;
            var entity = existing ?? new InterdictOrder
            {
                CreateDate = now,
                IsDeleted = false,
                RecruitOrderId = recruit.Id,
            };

            entity.IPAddress = ipAddress ?? entity.IPAddress ?? string.Empty;
            entity.RecruitOrderId = recruit.Id;
            if (isUpdate)
                entity.LastModifiedDate = now;

            var hadError = false;
            OrderImportShared.ApplyMappedProperties(
                entityType, entity, profile, contextFields, rowContext, resolved,
                ReservedKeys, profileFieldNames,
                err =>
                {
                    OrderImportShared.MarkRowError(row, err);
                    hadError = true;
                });
            if (hadError)
                continue;

            entity.RecruitOrderId = recruit.Id;
            entity.OrderTypeId = orderTypeId.Value;
            entity.StatusId = statusId.Value;
            entity.IssueTypeId = issueTypeId.Value;
            entity.InsuranceTypeId = insuranceTypeId.Value;
            entity.StartDate = startDate.Value.Date;

            if (string.IsNullOrWhiteSpace(entity.NationalNo))
                entity.NationalNo = ImportEmployeeLookupService.NormalizeNationalNo(resolved.GetValueOrDefault("NationalNo"));

            if (!isUpdate)
            {
                entity.UniqueId ??= Guid.NewGuid();
                orderCtx.InterdictOrders.Add(entity);
            }

            await orderCtx.SaveChangesAsync();

            row.MainRecordId = entity.Id;
            row.FinalizedAt = now;
            upserted++;
        }

        await context.SaveChangesAsync();
        return upserted;
    }

    public async Task<OperationResult?> RollbackFinalizedBatchAsync(BaseInfoContext context, ImportBatch batch)
    {
        if (batch.Status != ImportBatchStatus.Completed)
            return null;

        var mainIds = await context.ImportTempRows.AsNoTracking()
            .Where(r => r.ImportBatchId == batch.Id && !r.IsDeleted && r.MainRecordId != null)
            .Select(r => r.MainRecordId!.Value)
            .Distinct()
            .ToListAsync();

        if (mainIds.Count == 0)
            return null;

        var orderCtx = orderUnitOfWork.Context;
        var entities = await orderCtx.InterdictOrders.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await orderCtx.SaveChangesAsync();
        return null;
    }

    private static async Task<InterdictOrder?> FindExistingAsync(
        OrderContext orderCtx,
        long recruitOrderId,
        Dictionary<string, string?> resolved,
        Dictionary<string, string?> rowContext,
        long orderTypeId,
        DateTime startDate)
    {
        resolved.TryGetValue("Code", out var code);
        if (string.IsNullOrWhiteSpace(code))
            rowContext.TryGetValue("Code", out code);
        code = ExcelImportParser.NormalizeText(code);

        if (!string.IsNullOrWhiteSpace(code))
        {
            return await orderCtx.InterdictOrders
                .Where(i => !i.IsDeleted && i.Code == code)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
        }

        var serial = OrderImportShared.ReadShort(resolved, "Serial")
                     ?? OrderImportShared.ReadShort(rowContext, "Serial");
        if (serial is > 0)
        {
            return await orderCtx.InterdictOrders
                .Where(i => !i.IsDeleted && i.RecruitOrderId == recruitOrderId && i.Serial == serial)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
        }

        var start = startDate.Date;
        return await orderCtx.InterdictOrders
            .Where(i => !i.IsDeleted
                        && i.RecruitOrderId == recruitOrderId
                        && i.OrderTypeId == orderTypeId
                        && i.StartDate != null
                        && i.StartDate.Value.Date == start)
            .OrderByDescending(i => i.Id)
            .FirstOrDefaultAsync();
    }
}
