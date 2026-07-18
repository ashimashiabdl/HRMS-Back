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
/// emp.Education import with upsert by EmployeeId + EducationGradeId + EducationFieldId + EducationOrientationId.
/// </summary>
public class EducationImportHandler(
    IUnitOfWork<EmployeeContext> employeeUnitOfWork,
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    private const string TargetEntity = nameof(Education);

    private static readonly HashSet<string> ReservedRowKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName"
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
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var importCtx = contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null || contextService.ReadOrganisationChartId(importCtx) <= 0)
            throw new InvalidOperationException("ContextJson برای Education نامعتبر است.");

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
        var entityType = typeof(Education);
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

            var rowContext = ImportContextRowHelper.MergeForRow(batch, profile, resolved, contextService);

            var gradeId = ReadNullableLong(rowContext, resolved, "EducationGradeId");
            if (gradeId is null or <= 0)
            {
                MarkRowError(row, "مقطع تحصیلی الزامی است.");
                continue;
            }

            var fieldId = ReadNullableLong(rowContext, resolved, "EducationFieldId");
            var orientationId = ReadNullableLong(rowContext, resolved, "EducationOrientationId");

            var existing = await FindExistingAsync(employeeCtx, employeeId, gradeId.Value, fieldId, orientationId);
            var isUpdate = existing != null;
            var entity = existing ?? new Education
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

            // Ensure upsert key fields from context/row are applied even if blanked earlier
            entity.EducationGradeId = gradeId;
            entity.EducationFieldId = fieldId is > 0 ? fieldId : entity.EducationFieldId;
            entity.EducationOrientationId = orientationId is > 0 ? orientationId : entity.EducationOrientationId;

            if (entity.IsDefaultEducation == true)
            {
                var others = await employeeCtx.Educations
                    .Where(e => e.EmployeeId == employeeId && !e.IsDeleted && e.IsDefaultEducation == true
                                && (existing == null || e.Id != existing.Id))
                    .ToListAsync();
                foreach (var other in others)
                    other.IsDefaultEducation = false;
            }

            if (!isUpdate)
                employeeCtx.Educations.Add(entity);

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
        var entities = await employeeCtx.Educations.Where(e => mainIds.Contains(e.Id)).ToListAsync();
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await employeeCtx.SaveChangesAsync();
        return null;
    }

    private static async Task<Education?> FindExistingAsync(
        EmployeeContext ctx,
        long employeeId,
        long educationGradeId,
        long? educationFieldId,
        long? educationOrientationId)
    {
        var query = ctx.Educations.Where(e =>
            !e.IsDeleted
            && e.EmployeeId == employeeId
            && e.EducationGradeId == educationGradeId);

        if (educationFieldId is > 0)
            query = query.Where(e => e.EducationFieldId == educationFieldId);
        else
            query = query.Where(e => e.EducationFieldId == null || e.EducationFieldId == 0);

        if (educationOrientationId is > 0)
            query = query.Where(e => e.EducationOrientationId == educationOrientationId);
        else
            query = query.Where(e => e.EducationOrientationId == null || e.EducationOrientationId == 0);

        return await query.OrderByDescending(e => e.Id).FirstOrDefaultAsync();
    }

    private static long? ReadNullableLong(
        Dictionary<string, string?> rowContext,
        Dictionary<string, string?> resolved,
        string key)
    {
        if (rowContext.TryGetValue(key, out var fromCtx) && !string.IsNullOrWhiteSpace(fromCtx)
            && long.TryParse(ExcelImportParser.NormalizeDigitsToEnglish(fromCtx), NumberStyles.Integer, CultureInfo.InvariantCulture, out var ctxVal)
            && ctxVal > 0)
            return ctxVal;

        if (resolved.TryGetValue(key, out var fromRow) && !string.IsNullOrWhiteSpace(fromRow)
            && long.TryParse(ExcelImportParser.NormalizeDigitsToEnglish(fromRow), NumberStyles.Integer, CultureInfo.InvariantCulture, out var rowVal)
            && rowVal > 0)
            return rowVal;

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

        // EducationAverage is string on entity but often typed Decimal in profile
        if (underlying == typeof(string))
            return ExcelImportParser.NormalizeText(rawValue);

        if (underlying == typeof(int) || string.Equals(typeHint, "Int", StringComparison.OrdinalIgnoreCase))
            return int.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(long) || string.Equals(typeHint, "Long", StringComparison.OrdinalIgnoreCase))
            return long.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(decimal) || string.Equals(typeHint, "Decimal", StringComparison.OrdinalIgnoreCase))
            return decimal.Parse(normalized.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture);

        return Convert.ChangeType(normalized, underlying, CultureInfo.InvariantCulture);
    }

    private static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
