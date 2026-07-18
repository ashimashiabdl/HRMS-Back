using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.Employee.Core.Entities;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// emp.Family import with upsert by EmployeeId + member NationalNo.
/// Excel column MemberNationalNo maps to Family.NationalNo.
/// </summary>
public class FamilyImportHandler(
    IUnitOfWork<EmployeeContext> employeeUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(Family);

    private static readonly HashSet<string> ReservedRowKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName", "MemberNationalNo"
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

        await EmployeeLinkedImportHelper.ResolveEmployeeLinkedRowsAsync(
            profile, tempRows, fieldResolver, contextService, mode, organId);

        foreach (var row in tempRows)
        {
            if (row.ValidationStatus == ImportValidationStatus.Error)
                continue;

            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            resolved.TryGetValue("MemberNationalNo", out var memberNational);
            var normalizedMember = ImportEmployeeLookupService.NormalizeNationalNo(memberNational);
            if (string.IsNullOrWhiteSpace(normalizedMember))
            {
                MarkRowError(row, "کد ملی عضو خانواده الزامی است.");
                continue;
            }

            if (normalizedMember.Length != 10)
            {
                MarkRowError(row, "کد ملی عضو خانواده باید ۱۰ رقم باشد.");
                continue;
            }

            resolved["MemberNationalNo"] = normalizedMember;
            row.ResolvedDataJson = JsonSerializer.Serialize(resolved);
        }
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException("ContextJson برای Family نامعتبر است.");

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

        var employeeCtx = employeeUnitOfWork.Context;
        var entityType = typeof(Family);
        var now = DateTime.Now;
        var upserted = 0;

        foreach (var row in validRows)
        {
            var resolved = EmployeeLinkedImportHelper.DeserializeDict(row.ResolvedDataJson);
            if (!long.TryParse(resolved.GetValueOrDefault("EmployeeId"), NumberStyles.Integer, CultureInfo.InvariantCulture, out var employeeId)
                || employeeId <= 0)
            {
                MarkRowError(row, "شناسه کارمند نامعتبر است.");
                continue;
            }

            var memberNationalNo = ImportEmployeeLookupService.NormalizeNationalNo(
                resolved.GetValueOrDefault("MemberNationalNo"));
            if (string.IsNullOrWhiteSpace(memberNationalNo) || memberNationalNo.Length != 10)
            {
                MarkRowError(row, "کد ملی عضو خانواده معتبر نیست.");
                continue;
            }

            var rowContext = ImportContextRowHelper.MergeForRow(batch, profile, resolved, contextService);
            var existing = await employeeCtx.Families
                .Where(f => !f.IsDeleted && f.EmployeeId == employeeId && f.NationalNo == memberNationalNo)
                .OrderByDescending(f => f.Id)
                .FirstOrDefaultAsync();

            var isUpdate = existing != null;
            var entity = existing ?? new Family
            {
                title = "",
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                EmployeeId = employeeId,
            };

            entity.IPAddress = ipAddress ?? entity.IPAddress ?? string.Empty;
            entity.OrganisationChartId = organisationChartId;
            entity.EmployeeId = employeeId;
            entity.NationalNo = memberNationalNo;
            if (isUpdate)
                entity.LastModifiedDate = now;

            foreach (var ctxField in contextFields)
            {
                rowContext.TryGetValue(ctxField.TargetPropertyName, out var raw);
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                if (!TrySetPropertyFromString(entityType, entity, ctxField.TargetPropertyName, raw, ctxField.DataType, out var ctxError))
                {
                    MarkRowError(row, ctxError);
                    goto NextRow;
                }
            }

            foreach (var (propertyName, rawValue) in resolved)
            {
                if (ReservedRowKeys.Contains(propertyName) || string.IsNullOrWhiteSpace(rawValue))
                    continue;
                if (profileFieldNames.Count > 0 && !profileFieldNames.Contains(propertyName))
                    continue;

                var profileField = profile.Fields.FirstOrDefault(f =>
                    !f.IsDeleted && string.Equals(f.TargetPropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

                if (!TrySetPropertyFromString(entityType, entity, propertyName, rawValue, profileField?.DataType, out var rowError))
                {
                    MarkRowError(row, rowError);
                    goto NextRow;
                }
            }

            // Keep member national no (not employee NationalNo from reserved key)
            entity.NationalNo = memberNationalNo;

            if (!isUpdate)
                employeeCtx.Families.Add(entity);

            await employeeCtx.SaveChangesAsync();

            row.MainRecordId = entity.Id;
            row.FinalizedAt = now;
            upserted++;

            NextRow: ;
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

        var employeeCtx = employeeUnitOfWork.Context;
        var entities = await employeeCtx.Families.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await employeeCtx.SaveChangesAsync();
        return null;
    }

    private static bool TrySetPropertyFromString(
        Type entityType,
        object entity,
        string propertyName,
        string rawValue,
        string? dataType,
        out string error)
    {
        error = "";
        var prop = entityType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
        if (prop == null || !prop.CanWrite)
            return true;

        try
        {
            object? converted = prop.PropertyType switch
            {
                _ when prop.PropertyType == typeof(string) => ExcelImportParser.NormalizeText(rawValue),
                _ when prop.PropertyType == typeof(bool) => ImportContextRowHelper.TryParseBool(rawValue),
                _ when prop.PropertyType == typeof(bool?) => ImportContextRowHelper.TryParseBool(rawValue),
                _ when prop.PropertyType == typeof(DateTime) => ImportDateParser.ParseRequired(rawValue),
                _ when prop.PropertyType == typeof(DateTime?) => ImportDateParser.ParseNullable(rawValue),
                _ => ParseNumericOrString(prop.PropertyType, rawValue, dataType)
            };

            prop.SetValue(entity, converted);
            return true;
        }
        catch
        {
            error = $"مقدار نامعتبر برای '{propertyName}'.";
            return false;
        }
    }

    private static object? ParseNumericOrString(Type propertyType, string rawValue, string? dataType)
    {
        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(rawValue).Trim();
        var typeHint = (dataType ?? "").Trim();
        var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlying == typeof(string))
            return ExcelImportParser.NormalizeText(rawValue);

        if (underlying == typeof(int) || string.Equals(typeHint, "Int", StringComparison.OrdinalIgnoreCase))
            return int.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(long) || string.Equals(typeHint, "Long", StringComparison.OrdinalIgnoreCase))
            return long.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(decimal) || string.Equals(typeHint, "Decimal", StringComparison.OrdinalIgnoreCase))
            return decimal.Parse(normalized.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture);

        if (underlying == typeof(float) || string.Equals(typeHint, "Float", StringComparison.OrdinalIgnoreCase))
            return float.Parse(normalized.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

        return Convert.ChangeType(normalized, underlying, CultureInfo.InvariantCulture);
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
