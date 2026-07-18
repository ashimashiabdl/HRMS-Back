using System.Globalization;
using System.Text.Json;
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
/// Order.Recruit_Order import — Upsert by EmployeeId + PayLocationId.
/// PayLocationId از Context؛ در صورت خالی بودن از OrganisationChartId بچ.
/// </summary>
public class RecruitOrderImportHandler(
    IUnitOfWork<OrderContext> orderUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(RecruitOrder);

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
            throw new InvalidOperationException("ContextJson برای RecruitOrder نامعتبر است.");

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
        var entityType = typeof(RecruitOrder);
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
            if (payLocationId <= 0)
            {
                OrderImportShared.MarkRowError(row, "محل پرداخت (PayLocationId) الزامی است.");
                continue;
            }

            var employeeTypeId = OrderImportShared.ReadNullableLong(rowContext, resolved, "EmployeeTypeId");
            var employeeStatusId = OrderImportShared.ReadNullableLong(rowContext, resolved, "EmployeeStatusId");
            var costCenterId = OrderImportShared.ReadNullableLong(rowContext, resolved, "CostCenterId");

            if (employeeTypeId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "نوع استخدام الزامی است.");
                continue;
            }
            if (employeeStatusId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "وضعیت استخدامی الزامی است.");
                continue;
            }
            if (costCenterId is null or <= 0)
            {
                OrderImportShared.MarkRowError(row, "مرکز هزینه الزامی است.");
                continue;
            }

            var existing = await OrderImportShared.FindRecruitOrderAsync(orderCtx, employeeId, payLocationId);
            var isUpdate = existing != null;
            var entity = existing ?? new RecruitOrder
            {
                CreateDate = now,
                IsDeleted = false,
                EmployeeId = employeeId,
                PayLocationId = payLocationId,
            };

            entity.IPAddress = ipAddress ?? entity.IPAddress ?? string.Empty;
            entity.EmployeeId = employeeId;
            entity.PayLocationId = payLocationId;
            if (isUpdate)
                entity.LastModifiedDate = now;

            var hadError = false;
            OrderImportShared.ApplyMappedProperties(
                entityType, entity, profile, contextFields, rowContext, resolved,
                OrderImportShared.ReservedEmployeeKeys, profileFieldNames,
                err =>
                {
                    OrderImportShared.MarkRowError(row, err);
                    hadError = true;
                });
            if (hadError)
                continue;

            entity.EmployeeId = employeeId;
            entity.PayLocationId = payLocationId;
            entity.EmployeeTypeId = employeeTypeId.Value;
            entity.EmployeeStatusId = employeeStatusId.Value;
            entity.CostCenterId = costCenterId.Value;

            if (!isUpdate)
                orderCtx.RecruitOrders.Add(entity);

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
        var entities = await orderCtx.RecruitOrders.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await orderCtx.SaveChangesAsync();
        return null;
    }
}
