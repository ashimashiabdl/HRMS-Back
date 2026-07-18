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

public class TaxNonCashPaymentImportHandler(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(TaxNonCashPayment);

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
                ("PaymentPeriodId", "دوره پرداخت در context الزامی است."),
                ("ItemTypeId", "نوع آیتم در context الزامی است."));
            if (batchError != null)
            {
                EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, batchError);
                return;
            }
        }

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null)
            throw new InvalidOperationException("ContextJson برای TaxNonCashPayment نامعتبر است.");

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

            var paymentPeriodId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "PaymentPeriodId");
            var itemTypeId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "ItemTypeId");
            if (paymentPeriodId <= 0 || itemTypeId <= 0)
            {
                MarkRowError(row, "دوره پرداخت و نوع آیتم الزامی است.");
                continue;
            }

            var continuous = ImportContextRowHelper.ReadBool(
                batch.ContextMode, importCtx, resolved, "Continuous", defaultValue: false);

            if (!double.TryParse(resolved.GetValueOrDefault("Value"), NumberStyles.Number, CultureInfo.InvariantCulture, out var value)
                || value < 0)
            {
                MarkRowError(row, "مبلغ نامعتبر است.");
                continue;
            }

            var entity = new TaxNonCashPayment
            {
                title = "",
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                OrganisationChartId = organisationChartId,
                PaymentPeriodId = paymentPeriodId,
                ItemTypeId = itemTypeId,
                Value = value,
                Continuous = continuous
            };

            payroll.TaxNonCashPayments.Add(entity);
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
        var entities = await payroll.TaxNonCashPayments.Where(t => mainIds.Contains(t.Id)).ToListAsync();
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
