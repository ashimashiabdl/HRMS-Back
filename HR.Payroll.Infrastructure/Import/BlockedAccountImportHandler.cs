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

public class BlockedAccountImportHandler(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(BlockedAccount);

    public bool CanHandle(string targetEntityName) =>
        string.Equals(targetEntityName, TargetEntity, StringComparison.OrdinalIgnoreCase);

    public async Task ValidateAndResolveRowsAsync(BaseInfoContext context, ImportProfile profile, List<ImportTempRow> tempRows)
    {
        var (mode, importCtx, organId) = await EmployeeLinkedImportHelper.LoadBatchImportContextAsync(context, contextService, tempRows);
        if (organId <= 0)
        {
            EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, "سازمان پیش‌فرض مشخص نشده است.");
            return;
        }

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        var organisationChartId = importCtx == null ? 0 : contextService.ReadOrganisationChartId(importCtx);
        if (organisationChartId <= 0)
            throw new InvalidOperationException("سازمان پیش‌فرض برای BlockedAccount نامعتبر است.");

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

            var accountNo = resolved.GetValueOrDefault("AccountNo")?.Trim();
            if (string.IsNullOrWhiteSpace(accountNo))
            {
                MarkRowError(row, "شماره حساب الزامی است.");
                continue;
            }

            var entity = new BlockedAccount
            {
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                OrganisationChartId = organisationChartId,
                AccountNo = accountNo,
                Comment = resolved.GetValueOrDefault("Comment")?.Trim()
            };

            payroll.BlockedAccounts.Add(entity);
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
        var entities = await payroll.BlockedAccounts.Where(b => mainIds.Contains(b.Id)).ToListAsync();
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
