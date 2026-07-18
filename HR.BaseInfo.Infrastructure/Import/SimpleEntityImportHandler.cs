using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

namespace HR.BaseInfo.infrastructure.Import;

public class SimpleEntityImportHandler(
    ImportContextService contextService,
    ImportFieldResolver fieldResolver) : IImportTargetHandler, IScopedServices
{
    public bool CanHandle(string targetEntityName) => ImportEntityRegistry.IsSupported(targetEntityName);

    public async Task ValidateAndResolveRowsAsync(
        BaseInfoContext context,
        ImportProfile profile,
        List<ImportTempRow> tempRows)
    {
        var batchId = tempRows.FirstOrDefault()?.ImportBatchId ?? 0;
        var batch = batchId > 0
            ? await context.ImportBatches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == batchId)
            : null;
        var mode = batch?.ContextMode ?? ImportContextMode.BatchContext;
        var importCtx = batch == null ? null : contextService.ParseContextDictionary(batch.ContextJson, 0);
        var organId = importCtx == null ? 0 : contextService.ReadOrganisationChartId(importCtx);
        var contextFields = contextService.GetActiveContextFields(profile);

        if (mode == ImportContextMode.BatchContext && contextFields.Count > 0 && importCtx != null)
        {
            var contextError = ImportContextRowHelper.ValidateRequiredBatchContextFields(profile, importCtx);
            if (contextError != null)
            {
                MarkAllRowsError(tempRows, contextError);
                return;
            }
        }

        var entityType = ImportEntityRegistry.GetEntityType(profile.TargetEntityName)
            ?? throw new InvalidOperationException($"Entity '{profile.TargetEntityName}' is not registered for simple import.");

        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, mode);
        var profileFieldNames = profile.Fields.Where(f => !f.IsDeleted).Select(f => f.TargetPropertyName).ToHashSet(StringComparer.OrdinalIgnoreCase);
        var uniqueFields = fields.Where(f => f.IsUniqueKey).ToList();
        if (uniqueFields.Count == 0)
        {
            var titleField = fields.FirstOrDefault(f =>
                string.Equals(f.TargetPropertyName, "title", StringComparison.OrdinalIgnoreCase));
            if (titleField != null)
                uniqueFields = [titleField];
        }

        var existingUniqueValues = await LoadExistingUniqueValuesAsync(context, entityType, uniqueFields);
        var fileUniqueCounts = BuildFileUniqueCounts(tempRows, uniqueFields);

        foreach (var row in tempRows)
        {
            var parsed = DeserializeDict(row.ParsedDataJson);
            var resolved = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            var messages = new List<string>();
            var status = ImportValidationStatus.Valid;

            foreach (var field in fields)
            {
                parsed.TryGetValue(field.TargetPropertyName, out var raw);
                var (value, fieldError) = fieldResolver.NormalizeProfileField(raw, field);
                resolved[field.TargetPropertyName] = value;

                if (!string.IsNullOrEmpty(fieldError))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add(fieldError);
                }
            }

            if (status != ImportValidationStatus.Error
                && mode == ImportContextMode.RowExcelKeys
                && contextFields.Count > 0
                && organId > 0)
            {
                var contextErrors = await contextService.ValidateRowContextFieldsAsync(profile, resolved, organId);
                if (contextErrors.Count > 0)
                {
                    status = ImportValidationStatus.Error;
                    messages.AddRange(contextErrors);
                }
            }

            foreach (var uniqueField in uniqueFields)
            {
                resolved.TryGetValue(uniqueField.TargetPropertyName, out var uniqueValue);
                if (string.IsNullOrWhiteSpace(uniqueValue))
                    continue;

                var key = BuildUniqueKey(uniqueField.TargetPropertyName, uniqueValue);
                if (fileUniqueCounts.GetValueOrDefault(key) > 1)
                {
                    status = ImportValidationStatus.Error;
                    messages.Add($"{LabelFor(uniqueField)} در فایل تکراری است.");
                }

                if (existingUniqueValues.TryGetValue(uniqueField.TargetPropertyName, out var existingSet)
                    && existingSet.Contains(NormalizeUniqueCompare(uniqueValue)))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add($"{LabelFor(uniqueField)} در سیستم از قبل وجود دارد.");
                }
            }

            row.ResolvedDataJson = JsonSerializer.Serialize(resolved);
            row.ValidationStatus = status;
            row.ValidationMessagesJson = messages.Count > 0 ? JsonSerializer.Serialize(messages) : null;
            resolved.TryGetValue("title", out var title);
            row.title = title ?? $"ردیف {row.RowNumber}";
        }
    }

    public async Task<int> FinalizeAsync(BaseInfoContext context, ImportBatch batch, string? ipAddress)
    {
        var entityType = ImportEntityRegistry.GetEntityType(batch.ImportProfile?.TargetEntityName ?? "")
            ?? throw new InvalidOperationException("Entity type not found for finalize.");

        var profile = batch.ImportProfile!;
        var contextFields = contextService.GetActiveContextFields(profile);
        var profileFieldNames = profile.Fields.Where(f => !f.IsDeleted).Select(f => f.TargetPropertyName).ToHashSet(StringComparer.OrdinalIgnoreCase);

        var validRows = await context.ImportTempRows
            .Where(r => r.ImportBatchId == batch.Id
                        && !r.IsDeleted
                        && r.ValidationStatus == ImportValidationStatus.Valid
                        && r.MainRecordId == null)
            .ToListAsync();

        var inserted = 0;
        var now = DateTime.Now;

        foreach (var row in validRows)
        {
            var resolved = DeserializeDict(row.ResolvedDataJson);
            var applyValues = new Dictionary<string, string?>(resolved, StringComparer.OrdinalIgnoreCase);

            if (contextFields.Count > 0)
            {
                var rowContext = ImportContextRowHelper.MergeForRow(batch, profile, resolved, contextService);
                foreach (var ctxField in contextFields)
                {
                    if (profileFieldNames.Contains(ctxField.TargetPropertyName))
                        continue;
                    rowContext.TryGetValue(ctxField.TargetPropertyName, out var ctxValue);
                    if (!string.IsNullOrWhiteSpace(ctxValue))
                        applyValues[ctxField.TargetPropertyName] = ctxValue;
                }
            }

            var entity = Activator.CreateInstance(entityType) as BaseEntity
                ?? throw new InvalidOperationException($"Cannot create entity instance for {entityType.Name}.");

            entity.CreateDate = now;
            entity.StartDate = now;
            entity.IsDeleted = false;
            entity.IPAddress = ipAddress;

            if (!TryApplyResolvedProperties(entity, entityType, applyValues, out var applyError))
            {
                row.ValidationStatus = ImportValidationStatus.Error;
                row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { applyError });
                continue;
            }

            ImportEntityReflectionHelper.AddEntity(context, entityType, entity);
            await context.SaveChangesAsync();

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

        var entityType = ImportEntityRegistry.GetEntityType(batch.ImportProfile?.TargetEntityName ?? "");
        if (entityType == null)
            return null;

        var mainIds = await context.ImportTempRows
            .AsNoTracking()
            .Where(r => r.ImportBatchId == batch.Id && !r.IsDeleted && r.MainRecordId != null)
            .Select(r => r.MainRecordId!.Value)
            .Distinct()
            .ToListAsync();

        if (mainIds.Count == 0)
            return null;

        var entities = await ImportEntityReflectionHelper.LoadEntitiesByIdsAsync(context, entityType, mainIds);
        foreach (var entity in entities)
            entity.IsDeleted = true;

        await context.SaveChangesAsync();
        return null;
    }

    private static void MarkAllRowsError(List<ImportTempRow> tempRows, string message)
    {
        var json = JsonSerializer.Serialize(new[] { message });
        foreach (var row in tempRows)
        {
            row.ValidationStatus = ImportValidationStatus.Error;
            row.ValidationMessagesJson = json;
        }
    }

    private static async Task<Dictionary<string, HashSet<string>>> LoadExistingUniqueValuesAsync(
        BaseInfoContext context,
        Type entityType,
        List<ImportProfileField> uniqueFields)
    {
        var result = uniqueFields.ToDictionary(
            f => f.TargetPropertyName,
            _ => new HashSet<string>(StringComparer.OrdinalIgnoreCase),
            StringComparer.OrdinalIgnoreCase);

        if (uniqueFields.Count == 0)
            return result;

        var entities = await ImportEntityReflectionHelper.LoadActiveEntitiesAsync(context, entityType);

        foreach (var entity in entities)
        {
            foreach (var field in uniqueFields)
            {
                var prop = entityType.GetProperty(field.TargetPropertyName,
                    BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
                if (prop == null)
                    continue;

                var raw = prop.GetValue(entity)?.ToString();
                if (string.IsNullOrWhiteSpace(raw))
                    continue;

                result[field.TargetPropertyName].Add(NormalizeUniqueCompare(raw));
            }
        }

        return result;
    }

    private static Dictionary<string, int> BuildFileUniqueCounts(
        List<ImportTempRow> tempRows,
        List<ImportProfileField> uniqueFields)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);

        foreach (var row in tempRows)
        {
            var parsed = DeserializeDict(row.ParsedDataJson);
            foreach (var field in uniqueFields)
            {
                parsed.TryGetValue(field.TargetPropertyName, out var raw);
                var (value, _) = NormalizeFieldValue(raw, field);
                if (string.IsNullOrWhiteSpace(value))
                    continue;

                var key = BuildUniqueKey(field.TargetPropertyName, value);
                counts[key] = counts.GetValueOrDefault(key) + 1;
            }
        }

        return counts;
    }

    private static (string? Value, string? Error) NormalizeFieldValue(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var dataType = (field.DataType ?? "Text").Trim();
        if (string.Equals(dataType, "Int", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Integer", StringComparison.OrdinalIgnoreCase))
        {
            var normalized = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim();
            if (!int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out _))
                return (normalized, $"{LabelFor(field)} باید عدد صحیح باشد.");
            return (normalized, null);
        }

        if (string.Equals(dataType, "Bool", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Boolean", StringComparison.OrdinalIgnoreCase))
        {
            var v = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().ToLowerInvariant();
            var ok = v is "1" or "0" or "true" or "false" or "yes" or "no" or "بله" or "خیر";
            return ok ? (v, null) : (raw.Trim(), $"{LabelFor(field)} باید بله/خیر یا ۱/۰ باشد.");
        }

        if (ImportDateParser.IsDateDataType(dataType))
            return ImportDateParser.NormalizeToIsoDate(raw, field.IsRequired, LabelFor(field));

        return (ExcelImportParser.NormalizeText(raw), null);
    }

    private static bool TryApplyResolvedProperties(
        BaseEntity entity,
        Type entityType,
        Dictionary<string, string?> resolved,
        out string error)
    {
        error = "";

        foreach (var (propertyName, rawValue) in resolved)
        {
            if (string.IsNullOrWhiteSpace(rawValue))
                continue;

            var prop = entityType.GetProperty(propertyName,
                BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            if (prop == null || !prop.CanWrite)
                continue;

            try
            {
                object? converted = prop.PropertyType switch
                {
                    _ when prop.PropertyType == typeof(string) => rawValue.Trim(),
                    _ when prop.PropertyType == typeof(int) => int.Parse(
                        ExcelImportParser.NormalizeDigitsToEnglish(rawValue),
                        CultureInfo.InvariantCulture),
                    _ when prop.PropertyType == typeof(int?) => int.Parse(
                        ExcelImportParser.NormalizeDigitsToEnglish(rawValue),
                        CultureInfo.InvariantCulture),
                    _ when prop.PropertyType == typeof(bool) => ParseBool(rawValue),
                    _ when prop.PropertyType == typeof(bool?) => ParseBool(rawValue),
                    _ when prop.PropertyType == typeof(DateTime) => ImportDateParser.ParseRequired(rawValue),
                    _ when prop.PropertyType == typeof(DateTime?) => ImportDateParser.ParseNullable(rawValue),
                    _ => Convert.ChangeType(rawValue, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType, CultureInfo.InvariantCulture)
                };

                prop.SetValue(entity, converted);
            }
            catch
            {
                error = $"مقدار نامعتبر برای فیلد '{propertyName}'.";
                return false;
            }
        }

        if (string.IsNullOrWhiteSpace(entity.title))
        {
            error = "عنوان الزامی است.";
            return false;
        }

        return true;
    }

    private static bool ParseBool(string raw)
    {
        var v = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().ToLowerInvariant();
        return v is "1" or "true" or "yes" or "بله";
    }

    private static string LabelFor(ImportProfileField field) =>
        field.ExcelColumnHeader ?? field.TargetPropertyName;

    private static string BuildUniqueKey(string propertyName, string value) =>
        $"{propertyName}:{NormalizeUniqueCompare(value)}";

    private static string NormalizeUniqueCompare(string value) =>
        ExcelImportParser.NormalizeDigitsToEnglish(value).Trim().ToLowerInvariant();

    private static Dictionary<string, string?> DeserializeDict(string? json) =>
        string.IsNullOrEmpty(json)
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : JsonSerializer.Deserialize<Dictionary<string, string?>>(json)
              ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
}
