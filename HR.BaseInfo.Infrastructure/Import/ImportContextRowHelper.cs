using System.Globalization;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Import;

namespace HR.BaseInfo.infrastructure.Import;

/// <summary>
/// Reads Import context values from batch form (mode 1) or Excel row columns (mode 2).
/// </summary>
public static class ImportContextRowHelper
{
    public static Dictionary<string, string?> MergeForRow(
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        ImportProfile profile,
        Dictionary<string, string?> rowResolved,
        ImportContextService contextService) =>
        ImportEffectiveFieldsHelper.MergeRowContext(
            mode,
            batchContext,
            rowResolved,
            contextService.GetActiveContextFields(profile));

    public static Dictionary<string, string?> MergeForRow(
        ImportBatch batch,
        ImportProfile profile,
        Dictionary<string, string?> rowResolved,
        ImportContextService contextService)
    {
        var batchCtx = contextService.ParseContextDictionary(batch.ContextJson, 0)
            ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        return MergeForRow(batch.ContextMode, batchCtx, profile, rowResolved, contextService);
    }

    public static long ReadLong(
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        Dictionary<string, string?> rowResolved,
        string key) =>
        ImportEffectiveFieldsHelper.ReadContextLong(
            mode,
            key,
            batchContext ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase),
            rowResolved);

    public static long ReadLong(
        ImportContextService contextService,
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        Dictionary<string, string?> rowResolved,
        string key)
    {
        var merged = mode == ImportContextMode.RowExcelKeys
            ? rowResolved
            : batchContext ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
        return contextService.ReadLong(merged, key);
    }

    public static DateTime ReadDate(
        ImportContextService contextService,
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        Dictionary<string, string?> rowResolved,
        string key)
    {
        if (ImportEffectiveFieldsHelper.NormalizeMode(mode) == ImportContextMode.RowExcelKeys
            && rowResolved.TryGetValue(key, out var rowRaw)
            && !string.IsNullOrWhiteSpace(rowRaw))
        {
            if (ImportDateParser.TryParse(rowRaw, out var date))
                return date;
        }

        return batchContext == null ? default : contextService.ReadDate(batchContext, key);
    }

    public static bool ReadBool(
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        Dictionary<string, string?> rowResolved,
        string key,
        bool defaultValue = false)
    {
        if (ImportEffectiveFieldsHelper.NormalizeMode(mode) == ImportContextMode.RowExcelKeys
            && rowResolved.TryGetValue(key, out var rowRaw))
            return TryParseBool(rowRaw, defaultValue);

        var batchRaw = batchContext?.GetValueOrDefault(key);
        return TryParseBool(batchRaw, defaultValue);
    }

    public static bool TryParseBool(string? raw, bool defaultValue = false)
    {
        if (string.IsNullOrWhiteSpace(raw))
            return defaultValue;
        var v = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().ToLowerInvariant();
        return v is "1" or "true" or "yes" or "بله";
    }

    public static string? ValidateBatchLongKeys(
        Dictionary<string, string?> batchContext,
        ImportContextService contextService,
        params (string Key, string Message)[] keys)
    {
        foreach (var (key, message) in keys)
        {
            if (contextService.ReadLong(batchContext, key) <= 0)
                return message;
        }

        return null;
    }

    public static string? ValidateBatchDateKey(
        Dictionary<string, string?> batchContext,
        ImportContextService contextService,
        string key,
        string message) =>
        contextService.ReadDate(batchContext, key) == default ? message : null;

    public static string? ValidateRequiredBatchContextFields(
        ImportProfile profile,
        Dictionary<string, string?> importCtx)
    {
        foreach (var field in profile.ContextFields.Where(f => !f.IsDeleted && f.IsRequired))
        {
            importCtx.TryGetValue(field.TargetPropertyName, out var raw);
            if (string.IsNullOrWhiteSpace(raw))
                return $"{field.title ?? field.TargetPropertyName} در context الزامی است.";

            if (field.ControlType is ImportContextControlType.Number or ImportContextControlType.Combo
                && field.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped or FkLookupType.None)
            {
                if (!long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) || value <= 0)
                    return $"{field.title ?? field.TargetPropertyName} معتبر نیست.";
            }
        }

        return null;
    }
}
