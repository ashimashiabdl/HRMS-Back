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
/// Order.Interdict_Order_WageItem — Upsert by InterdictOrderId + WageItemId.
/// والد از کد حکم / سریال + کد ملی کارمند.
/// </summary>
public class InterdictOrderWageItemImportHandler(
    IUnitOfWork<OrderContext> orderUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(InterdictOrderWageItem);

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
            throw new InvalidOperationException("ContextJson برای InterdictOrderWageItem نامعتبر است.");

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
        var entityType = typeof(InterdictOrderWageItem);
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

            var wageItemId = OrderImportShared.ReadNullableLong(rowContext, resolved, "WageItemId");
            if (wageItemId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "عامل حقوقی (WageItemId) الزامی است.");
                continue;
            }

            if (!TryReadValue(resolved, rowContext, out var value, out var valueError))
            {
                OrderImportShared.MarkRowError(row, valueError);
                continue;
            }

            var existing = await orderCtx.InterdictOrderWageItems
                .Where(w => !w.IsDeleted && w.InterdictOrderId == parent.Id && w.WageItemId == wageItemId.Value)
                .OrderByDescending(w => w.Id)
                .FirstOrDefaultAsync();

            var isUpdate = existing != null;
            var entity = existing ?? new InterdictOrderWageItem
            {
                CreateDate = now,
                IsDeleted = false,
                InterdictOrderId = parent.Id,
                WageItemId = wageItemId.Value,
            };

            entity.IPAddress = ipAddress ?? entity.IPAddress ?? string.Empty;
            entity.InterdictOrderId = parent.Id;
            entity.WageItemId = wageItemId.Value;
            entity.Value = value;
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
            entity.WageItemId = wageItemId.Value;
            entity.Value = value;

            if (!isUpdate)
                orderCtx.InterdictOrderWageItems.Add(entity);

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
        var entities = await orderCtx.InterdictOrderWageItems.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await orderCtx.SaveChangesAsync();
        return null;
    }

    private static bool TryReadValue(
        Dictionary<string, string?> resolved,
        Dictionary<string, string?> rowContext,
        out int value,
        out string error)
    {
        value = 0;
        error = "";
        string? raw = null;
        if (resolved.TryGetValue("Value", out var fromRow) && !string.IsNullOrWhiteSpace(fromRow))
            raw = fromRow;
        else if (rowContext.TryGetValue("Value", out var fromCtx) && !string.IsNullOrWhiteSpace(fromCtx))
            raw = fromCtx;

        if (string.IsNullOrWhiteSpace(raw))
        {
            error = "مقدار عامل حقوقی الزامی است.";
            return false;
        }

        if (!int.TryParse(
                ExcelImportParser.NormalizeDigitsToEnglish(raw),
                NumberStyles.Integer,
                CultureInfo.InvariantCulture,
                out value))
        {
            error = "مقدار عامل حقوقی نامعتبر است.";
            return false;
        }

        return true;
    }
}
