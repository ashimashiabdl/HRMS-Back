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

public class EmployeeLeaveEntitlementImportHandler(
    IUnitOfWork<PayrollContext> payrollUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(EmployeeLeaveEntitlement);

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
                ("LeaveTypeId", "نوع مرخصی در context الزامی است."),
                ("Year", "سال در context الزامی است."));
            if (batchError != null)
            {
                EmployeeLinkedImportHelper.MarkAllRowsError(tempRows, batchError);
                return;
            }
        }

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);

        var payroll = payrollUnitOfWork.Context;
        var fileEmployeeKeys = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in tempRows.Where(r => r.ValidationStatus == ImportValidationStatus.Valid))
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
                continue;

            var leaveTypeId = ImportContextRowHelper.ReadLong(mode, importCtx, resolved, "LeaveTypeId");
            var year = (int)ImportContextRowHelper.ReadLong(mode, importCtx, resolved, "Year");
            if (leaveTypeId <= 0 || year <= 0)
            {
                MarkRowError(row, "نوع مرخصی و سال الزامی است.");
                continue;
            }

            var exists = await payroll.EmployeeLeaveEntitlements.AsNoTracking().AnyAsync(e =>
                !e.IsDeleted
                && e.OrganisationChartId == organId
                && e.EmployeeId == employeeId
                && e.LeaveTypeId == leaveTypeId
                && e.Year == year);

            if (exists)
            {
                MarkRowError(row, "سهمیه مرخصی این کارمند برای همین نوع و سال از قبل ثبت شده است.");
                continue;
            }

            var fileKey = $"{employeeId}:{leaveTypeId}:{year}";
            fileEmployeeKeys[fileKey] = fileEmployeeKeys.GetValueOrDefault(fileKey) + 1;
            if (fileEmployeeKeys[fileKey] > 1)
            {
                MarkRowError(row, "کارمند در فایل تکراری است.");
            }
        }
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null)
            throw new InvalidOperationException("ContextJson برای EmployeeLeaveEntitlement نامعتبر است.");

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

            var leaveTypeId = ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "LeaveTypeId");
            var year = (int)ImportContextRowHelper.ReadLong(batch.ContextMode, importCtx, resolved, "Year");
            if (leaveTypeId <= 0 || year <= 0)
            {
                MarkRowError(row, "نوع مرخصی و سال الزامی است.");
                continue;
            }

            if (!decimal.TryParse(resolved.GetValueOrDefault("LeaveAmount"), NumberStyles.Number, CultureInfo.InvariantCulture, out var leaveAmount)
                || leaveAmount < 0)
            {
                MarkRowError(row, "سهمیه مرخصی نامعتبر است.");
                continue;
            }

            var entity = new EmployeeLeaveEntitlement
            {
                title = "",
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = ipAddress,
                EmployeeId = employeeId,
                OrganisationChartId = organisationChartId,
                LeaveTypeId = leaveTypeId,
                Year = year,
                LeaveAmount = leaveAmount
            };

            payroll.EmployeeLeaveEntitlements.Add(entity);
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
        var entities = await payroll.EmployeeLeaveEntitlements.Where(e => mainIds.Contains(e.Id)).ToListAsync();
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
