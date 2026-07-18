using System.Globalization;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.SharedKernel.Import;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

internal static class EmployeeLinkedImportHelper
{
    public static async Task<(ImportContextMode Mode, Dictionary<string, string?>? Context, long OrganisationChartId)> LoadBatchImportContextAsync(
        BaseInfoContext context,
        ImportContextService contextService,
        List<ImportTempRow> tempRows)
    {
        var batchId = tempRows.FirstOrDefault()?.ImportBatchId ?? 0;
        if (batchId <= 0)
            return (ImportContextMode.BatchContext, null, 0);

        var batch = await context.ImportBatches.AsNoTracking().FirstOrDefaultAsync(b => b.Id == batchId);
        var importCtx = batch == null ? null : contextService.ParseContextDictionary(batch.ContextJson, 0);
        if (importCtx == null)
            return (ImportContextMode.BatchContext, null, 0);

        var organId = contextService.ReadOrganisationChartId(importCtx);
        var mode = batch?.ContextMode ?? ImportContextMode.BatchContext;
        return organId <= 0 ? (mode, null, 0) : (mode, importCtx, organId);
    }

    public static async Task<(Dictionary<string, string?>? Context, long OrganisationChartId)> TryLoadBatchContextAsync(
        BaseInfoContext context,
        ImportContextService contextService,
        List<ImportTempRow> tempRows)
    {
        var (mode, importCtx, organId) = await LoadBatchImportContextAsync(context, contextService, tempRows);
        return (importCtx, organId);
    }

    public static async Task ResolveEmployeeLinkedRowsAsync(
        ImportProfile profile,
        List<ImportTempRow> tempRows,
        ImportFieldResolver fieldResolver,
        ImportContextService? contextService = null,
        ImportContextMode contextMode = ImportContextMode.BatchContext,
        long organisationChartId = 0)
    {
        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, contextMode);
        var fileNationalCounts = BuildFileNationalCounts(tempRows);
        var contextFields = profile.ContextFields.Where(f => !f.IsDeleted).ToList();

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
                && contextMode == ImportContextMode.RowExcelKeys
                && contextService != null
                && organisationChartId > 0
                && contextFields.Count > 0)
            {
                var contextErrors = await contextService.ValidateRowContextFieldsAsync(profile, resolved, organisationChartId);
                if (contextErrors.Count > 0)
                {
                    status = ImportValidationStatus.Error;
                    messages.AddRange(contextErrors);
                }
            }

            if (status != ImportValidationStatus.Error)
            {
                resolved.TryGetValue("NationalNo", out var nationalNo);
                if (!string.IsNullOrWhiteSpace(nationalNo) && fileNationalCounts.GetValueOrDefault(nationalNo) > 1)
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کد ملی در فایل تکراری است.");
                }
            }

            if (status != ImportValidationStatus.Error)
            {
                resolved.TryGetValue("NationalNo", out var nationalNo);
                if (string.IsNullOrWhiteSpace(nationalNo))
                {
                    status = ImportValidationStatus.Error;
                    messages.Add("کد ملی الزامی است.");
                }
                else
                {
                    var employeeId = fieldResolver.ResolveEmployeeId(nationalNo);
                    if (employeeId <= 0)
                    {
                        status = ImportValidationStatus.Error;
                        messages.Add("کاربری با این کد ملی یافت نشد.");
                    }
                    else
                    {
                        resolved["EmployeeId"] = employeeId.ToString(CultureInfo.InvariantCulture);
                        resolved["EmployeeFullName"] = fieldResolver.ResolveEmployeeFullName(employeeId);
                    }
                }
            }

            row.ResolvedDataJson = JsonSerializer.Serialize(resolved);
            row.ValidationStatus = status;
            row.ValidationMessagesJson = messages.Count > 0 ? JsonSerializer.Serialize(messages) : null;
            resolved.TryGetValue("EmployeeFullName", out var fn);
            resolved.TryGetValue("NationalNo", out var nn);
            row.title = !string.IsNullOrWhiteSpace(fn) ? fn : nn ?? $"ردیف {row.RowNumber}";
        }
    }

    public static Task ResolveEmployeeLinkedRowsAsync(
        ImportProfile profile,
        List<ImportTempRow> tempRows,
        ImportFieldResolver fieldResolver) =>
        ResolveEmployeeLinkedRowsAsync(profile, tempRows, fieldResolver, null, ImportContextMode.BatchContext, 0);

    public static void MarkAllRowsError(List<ImportTempRow> tempRows, string message)
    {
        var json = JsonSerializer.Serialize(new[] { message });
        foreach (var row in tempRows)
        {
            row.ValidationStatus = ImportValidationStatus.Error;
            row.ValidationMessagesJson = json;
        }
    }

    public static Dictionary<string, int> BuildFileNationalCounts(List<ImportTempRow> tempRows)
    {
        var counts = new Dictionary<string, int>(StringComparer.OrdinalIgnoreCase);
        foreach (var row in tempRows)
        {
            var parsed = DeserializeDict(row.ParsedDataJson);
            parsed.TryGetValue("NationalNo", out var raw);
            var nationalNo = ImportEmployeeLookupService.NormalizeNationalNo(raw);
            if (string.IsNullOrWhiteSpace(nationalNo))
                continue;
            counts[nationalNo] = counts.GetValueOrDefault(nationalNo) + 1;
        }

        return counts;
    }

    public static Dictionary<string, string?> DeserializeDict(string? json)
    {
        if (string.IsNullOrEmpty(json))
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        var parsed = JsonSerializer.Deserialize<Dictionary<string, string?>>(json);
        if (parsed == null || parsed.Count == 0)
            return new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

        return new Dictionary<string, string?>(parsed, StringComparer.OrdinalIgnoreCase);
    }

    public static bool TryParseBool(string? raw, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return defaultValue;
        var v = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().ToLowerInvariant();
        return v is "1" or "true" or "yes" or "بله";
    }
}
