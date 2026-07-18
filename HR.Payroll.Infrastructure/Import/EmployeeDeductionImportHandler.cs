using System.Globalization;
using System.Text.Json;
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

public class EmployeeDeductionImportHandler(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(EmployeeDeduction);

    public bool CanHandle(string targetEntityName) =>
        string.Equals(targetEntityName, TargetEntity, StringComparison.OrdinalIgnoreCase);

    public async Task ValidateAndResolveRowsAsync(
        BaseInfoContext context,
        ImportProfile profile,
        List<ImportTempRow> tempRows)
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
                ("DeductionTypeId", "نوع کسر در context الزامی است."),
                ("StartDeductPaymentPeriodId", "دوره شروع کسر در context الزامی است."));
            batchError ??= ImportContextRowHelper.ValidateBatchDateKey(
                importCtx, contextService, "PaymentDate", "تاریخ پرداخت در context الزامی است.");
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
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException("ContextJson برای EmployeeDeduction نامعتبر است.");

        var organisationChartId = contextService.ReadOrganisationChartId(importCtx);

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
            if (!TryReadResolvedRow(resolved, out var employeeId, out var installmentAmount, out var error))
            {
                MarkRowError(row, error);
                continue;
            }

            var deductionTypeId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "DeductionTypeId");
            var startPeriodId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "StartDeductPaymentPeriodId");
            var paymentDate = ImportContextRowHelper.ReadDate(
                contextService, batch.ContextMode, importCtx, resolved, "PaymentDate");

            if (deductionTypeId <= 0 || startPeriodId <= 0 || paymentDate == default)
            {
                MarkRowError(row, "اطلاعات تکمیلی Import ناقص است.");
                continue;
            }

            var entity = new EmployeeDeduction
            {
                title = "",
                CreateDate = now,
                IsDeleted = false,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                DeductionTypeId = deductionTypeId,
                OrganisationChartId = organisationChartId,
                StartDeductPaymentPeriodId = startPeriodId,
                PaymentDate = paymentDate,
                LoanPaymentDocDesc = "Import اکسل",
                AllAmount = installmentAmount,
                InstallmentAmount = installmentAmount,
                RemainingCrumbsAtFirst = false,
                IsActive = true,
                FileId = batch.FileId
            };

            payroll.EmployeeDeductions.Add(entity);
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

        var mainIds = await context.ImportTempRows
            .AsNoTracking()
            .Where(r => r.ImportBatchId == batch.Id && !r.IsDeleted && r.MainRecordId != null)
            .Select(r => r.MainRecordId!.Value)
            .Distinct()
            .ToListAsync();

        if (mainIds.Count == 0)
            return null;

        var payroll = payrollUnitOfWork.Context;

        var usedInFiche = await payroll.FicheItems
            .AsNoTracking()
            .AnyAsync(f => f.EmployeeDeductionId != null && mainIds.Contains(f.EmployeeDeductionId.Value));
        if (usedInFiche)
        {
            return OperationResult.Failed(
                "برخی رکوردهای این دسته Import در محاسبات فیش لحاظ شده‌اند. پس از حذف فیش مربوطه دوباره تلاش کنید.");
        }

        await using var transaction = await payroll.Database.BeginTransactionAsync();
        try
        {
            var payments = await payroll.EmployeeDeductionPayments
                .Where(p => mainIds.Contains(p.EmployeeDeductionId))
                .ToListAsync();
            if (payments.Count > 0)
                payroll.EmployeeDeductionPayments.RemoveRange(payments);

            var deductions = await payroll.EmployeeDeductions
                .Where(ed => mainIds.Contains(ed.Id))
                .ToListAsync();
            if (deductions.Count > 0)
                payroll.EmployeeDeductions.RemoveRange(deductions);

            await payroll.SaveChangesAsync();
            await transaction.CommitAsync();
            return null;
        }
        catch (Exception ex)
        {
            await transaction.RollbackAsync();
            return OperationResult.Failed($"خطا در بازگردانی رکوردهای Import: {ex.Message}");
        }
    }

    private static bool TryReadResolvedRow(
        Dictionary<string, string?> resolved,
        out long employeeId,
        out long installmentAmount,
        out string error)
    {
        employeeId = 0;
        installmentAmount = 0;
        error = "";

        resolved.TryGetValue("EmployeeId", out var empRaw);
        if (!long.TryParse(empRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out employeeId) || employeeId <= 0)
        {
            error = "شناسه کارمند نامعتبر است.";
            return false;
        }

        resolved.TryGetValue("InstallmentAmount", out var amountRaw);
        if (!long.TryParse(amountRaw, NumberStyles.Integer, CultureInfo.InvariantCulture, out installmentAmount) || installmentAmount <= 0)
        {
            error = "مبلغ نامعتبر است.";
            return false;
        }

        return true;
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
