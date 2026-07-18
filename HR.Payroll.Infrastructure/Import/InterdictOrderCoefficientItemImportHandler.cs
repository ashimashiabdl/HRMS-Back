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
/// Order.Interdict_Order_CoefficientItem — Upsert by InterdictOrderId + CoefficientId.
/// </summary>
public class InterdictOrderCoefficientItemImportHandler(
    IUnitOfWork<OrderContext> orderUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(InterdictOrderCoefficientItem);

    private static readonly HashSet<string> ReservedKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName",
        "InterdictOrderCode", "InterdictOrderId", "Serial"
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
            throw new InvalidOperationException("ContextJson برای InterdictOrderCoefficientItem نامعتبر است.");

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
        var entityType = typeof(InterdictOrderCoefficientItem);
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

            var parent = await OrderImportShared.FindInterdictOrderAsync(orderCtx, resolved, employeeId, payLocationId);
            if (parent == null)
            {
                OrderImportShared.MarkRowError(row, "حکم کارگزینی (InterdictOrder) یافت نشد. کد حکم یا سریال را بررسی کنید.");
                continue;
            }

            var coefficientId = OrderImportShared.ReadNullableLong(rowContext, resolved, "CoefficientId");
            if (coefficientId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "ضریب (CoefficientId) الزامی است.");
                continue;
            }

            if (!TryReadOutPutFactValue(resolved, rowContext, out var outValue, out var valueError))
            {
                OrderImportShared.MarkRowError(row, valueError);
                continue;
            }

            var existing = await orderCtx.InterdictOrderCoefficientItems
                .Where(c => !c.IsDeleted && c.InterdictOrderId == parent.Id && c.CoefficientId == coefficientId.Value)
                .OrderByDescending(c => c.Id)
                .FirstOrDefaultAsync();

            var isUpdate = existing != null;
            var entity = existing ?? new InterdictOrderCoefficientItem
            {
                CreateDate = now,
                IsDeleted = false,
                InterdictOrderId = parent.Id,
                CoefficientId = coefficientId.Value,
            };

            entity.IPAddress = ipAddress ?? entity.IPAddress ?? string.Empty;
            entity.InterdictOrderId = parent.Id;
            entity.CoefficientId = coefficientId.Value;
            entity.OutPutFactValue = outValue;
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

            entity.InterdictOrderId = parent.Id;
            entity.CoefficientId = coefficientId.Value;
            entity.OutPutFactValue = outValue;

            if (!isUpdate)
                orderCtx.InterdictOrderCoefficientItems.Add(entity);

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
        var entities = await orderCtx.InterdictOrderCoefficientItems.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await orderCtx.SaveChangesAsync();
        return null;
    }

    private static bool TryReadOutPutFactValue(
        Dictionary<string, string?> resolved,
        Dictionary<string, string?> rowContext,
        out double outValue,
        out string error)
    {
        outValue = 0;
        error = "";
        string? raw = null;
        if (resolved.TryGetValue("OutPutFactValue", out var fromRow) && !string.IsNullOrWhiteSpace(fromRow))
            raw = fromRow;
        else if (rowContext.TryGetValue("OutPutFactValue", out var fromCtx) && !string.IsNullOrWhiteSpace(fromCtx))
            raw = fromCtx;

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = "مقدار ضریب الزامی است.";
            return false;
        }

        if (!double.TryParse(
                ExcelImportParser.NormalizeDigitsToEnglish(raw).Replace(",", "."),
                NumberStyles.Float,
                CultureInfo.InvariantCulture,
                out outValue))
        {
            error = "مقدار ضریب نامعتبر است.";
            return false;
        }

        return true;
    }
}
