using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Import;

namespace HR.BaseInfo.infrastructure.Import;

/// <summary>
/// Builds effective Excel field list and per-row context for batch vs row-level Import modes.
/// </summary>
public static class ImportEffectiveFieldsHelper
{
    public static ImportContextMode NormalizeMode(ImportContextMode mode) =>
        mode == ImportContextMode.RowExcelKeys ? ImportContextMode.RowExcelKeys : ImportContextMode.BatchContext;

    public static List<ImportProfileField> GetEffectiveRowFields(ImportProfile profile, ImportContextMode mode)
    {
        var rows = profile.Fields.Where(f => !f.IsDeleted).OrderBy(f => f.DisplayOrder).ThenBy(f => f.ExcelColumnOrder).ToList();
        if (NormalizeMode(mode) != ImportContextMode.RowExcelKeys)
            return rows;

        var nextOrder = rows.Count > 0 ? rows.Max(f => f.ExcelColumnOrder) + 1 : 1;
        foreach (var cf in profile.ContextFields.Where(f => !f.IsDeleted).OrderBy(f => f.DisplayOrder).ThenBy(f => f.Id))
        {
            rows.Add(new ImportProfileField
            {
                ImportProfileId = profile.Id,
                TargetPropertyName = cf.TargetPropertyName,
                ExcelColumnHeader = BuildRowModeHeader(cf),
                ExcelColumnLetter = ExcelImportParser.ColumnIndexToLetter(nextOrder),
                ExcelColumnOrder = nextOrder,
                DisplayOrder = cf.DisplayOrder + 1000,
                DataType = cf.DataType ?? "Long",
                IsRequired = cf.IsRequired,
                IsUniqueKey = false,
                FkLookupType = cf.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped
                    ? cf.FkLookupType
                    : cf.ControlType == ImportContextControlType.Combo ? FkLookupType.ComboKeyValue : FkLookupType.None,
                FkReferenceEntity = cf.FkReferenceEntity,
                FkReferenceField = cf.FkReferenceSchema,
                title = cf.title ?? cf.TargetPropertyName,
                IsDeleted = false,
            });
            nextOrder++;
        }

        return rows;
    }

    public static Dictionary<string, string?> MergeRowContext(
        ImportContextMode mode,
        Dictionary<string, string?>? batchContext,
        Dictionary<string, string?> rowResolved,
        IEnumerable<ImportProfileContextField> contextFields)
    {
        var merged = batchContext == null
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : new Dictionary<string, string?>(batchContext, StringComparer.OrdinalIgnoreCase);

        if (NormalizeMode(mode) != ImportContextMode.RowExcelKeys)
            return merged;

        foreach (var field in contextFields.Where(f => !f.IsDeleted))
        {
            rowResolved.TryGetValue(field.TargetPropertyName, out var value);
            merged[field.TargetPropertyName] = value;
        }

        return merged;
    }

    public static long ReadContextLong(
        ImportContextMode mode,
        string propertyName,
        Dictionary<string, string?> batchContext,
        Dictionary<string, string?> rowResolved)
    {
        if (NormalizeMode(mode) == ImportContextMode.RowExcelKeys
            && rowResolved.TryGetValue(propertyName, out var rowRaw)
            && long.TryParse(rowRaw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var rowVal))
            return rowVal;

        if (batchContext.TryGetValue(propertyName, out var batchRaw)
            && long.TryParse(batchRaw, System.Globalization.NumberStyles.Integer, System.Globalization.CultureInfo.InvariantCulture, out var batchVal))
            return batchVal;

        return 0;
    }

    private static string BuildRowModeHeader(ImportProfileContextField field)
    {
        var label = field.title ?? field.TargetPropertyName;
        return $"{label} (Id)";
    }
}
