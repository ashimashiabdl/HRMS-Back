using System.Globalization;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.Payroll.Core.Data;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

public class EmployeeFundImportHandler(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(EmployeeFund);

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
            var batchError = ImportContextRowHelper.ValidateBatchLongKeys(
                importCtx,
                contextService,
                ("FundTypeId", "نوع صندوق در context الزامی است."),
                ("StartDeductPaymentPeriodId", "دوره پرداخت در context الزامی است."));
            if (batchError != null)
            {
                EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, batchError);
                return;
            }
        }

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);

        var payroll = payrollUnitOfWork.Context;

        foreach (var row in tempRows.Where(r => r.ValidationStatus == ImportValidationStatus.Valid))
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
                continue;

            var fundTypeId = ImportContextRowHelper.ReadLong(mode, importCtx, resolved, "FundTypeId");
            var startPeriodId = ImportContextRowHelper.ReadLong(mode, importCtx, resolved, "StartDeductPaymentPeriodId");
            if (fundTypeId <= 0 || startPeriodId <= 0)
            {
                MarkRowError(row, "نوع صندوق و دوره پرداخت الزامی است.");
                continue;
            }

            var exists = await payroll.EmployeeFunds.AsNoTracking().AnyAsync(f =>
                !f.IsDeleted
                && f.OrganisationChartId == organId
                && f.EmployeeId == employeeId
                && f.FundTypeId == fundTypeId
                && f.StartDeductPaymentPeriodId == startPeriodId);

            if (exists)
            {
                MarkRowError(row, "این کارمند قبلاً برای همین صندوق و دوره ثبت شده است.");
            }
        }
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null)
            throw new InvalidOperationException("ContextJson برای EmployeeFund نامعتبر است.");

        var organisationChartId = contextService.ReadOrganisationChartId(importCtx);
        if (organisationChartId <= 0)
            throw new InvalidOperationException("اطلاعات تکمیلی Import ناقص است.");

        var validRows = await context.ImportTempRows
            .Where(r => r.ImportBatchId == batch.Id
                        && !r.IsDeleted
                        && r.ValidationStatus == ImportValidationStatus.Valid
                        && r.MainRecordId == null)
            .ToListAsync();

        if (validRows.Count == 0)
            return 0;

        var payroll = payrollUnitOfWork.Context;
        var now = DateTime.Now;
        var inserted = 0;

        foreach (var row in validRows)
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
            {
                MarkRowError(row, "شناسه کارمند نامعتبر است.");
                continue;
            }

            var fundTypeId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "FundTypeId");
            var startPeriodId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "StartDeductPaymentPeriodId");
            if (fundTypeId <= 0 || startPeriodId <= 0)
            {
                MarkRowError(row, "نوع صندوق و دوره پرداخت الزامی است.");
                continue;
            }

            var isActive = EmployeeLinkedImportHelper.TryParseBool(resolved.GetValueOrDefault("IsActive"), defaultValue: true);
            var entity = new EmployeeFund
            {
                title = "",
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                FundTypeId = fundTypeId,
                OrganisationChartId = organisationChartId,
                StartDeductPaymentPeriodId = startPeriodId,
                IsActive = isActive
            };

            payroll.EmployeeFunds.Add(entity);
            await payroll.SaveChangesAsync();

            row.MainRecordId = entity.Id;
            row.FinalizedAt = now;
            inserted++;
        }

        await context.SaveChangesAsync();
        return inserted;
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

        var payroll = payrollUnitOfWork.Context;
        var entities = await payroll.EmployeeFunds.Where(f => mainIds.Contains(f.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await payroll.SaveChangesAsync();
        return null;
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = System.Text.Json.JsonSerializer.Serialize(new[] { message });
    }
}
