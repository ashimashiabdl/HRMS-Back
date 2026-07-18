using System.Globalization;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;

namespace HR.BaseInfo.infrastructure.Import;

public class ImportContextService(IEnumerable<IImportOrganScopedFkValidator> organFkValidators) : IScopedServices
{
    private readonly IReadOnlyList<IImportOrganScopedFkValidator> _organFkValidators = organFkValidators.ToList();

    public IReadOnlyList<ImportProfileContextField> GetActiveContextFields(ImportProfile profile) =>
        profile.ContextFields
            .Where(f => !f.IsDeleted)
            .OrderBy(f => f.DisplayOrder)
            .ThenBy(f => f.Id)
            .ToList();

    public bool RequiresOrganisationContext(ImportProfile profile)
    {
        var fields = GetActiveContextFields(profile);
        if (fields.Count == 0)
            return profile.HandlerType == ImportHandlerType.EmployeeLinked || profile.RequiresEmployeeLookup;

        return fields.Any(f =>
            f.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped or FkLookupType.ContextForm);
    }

    public async Task<OperationResult?> ValidateAndNormalizeAsync(
        ImportProfile profile,
        string? contextJson,
        long organisationChartId,
        ImportContextMode contextMode = ImportContextMode.BatchContext)
    {
        var fields = GetActiveContextFields(profile);
        if (fields.Count == 0)
            return null;

        if (RequiresOrganisationContext(profile) && organisationChartId <= 0)
            return OperationResult.Failed("سازمان پیش‌فرض مشخص نشده است.");

        if (ImportEffectiveFieldsHelper.NormalizeMode(contextMode) == ImportContextMode.RowExcelKeys)
        {
            var rowModeDict = ParseContextDictionary(contextJson, organisationChartId);
            return rowModeDict == null
                ? OperationResult.Failed("اطلاعات تکمیلی Import نامعتبر است.")
                : null;
        }

        var dict = ParseContextDictionary(contextJson, organisationChartId);
        if (dict == null)
            return OperationResult.Failed("اطلاعات تکمیلی Import نامعتبر است.");

        foreach (var field in fields)
        {
            dict.TryGetValue(field.TargetPropertyName, out var raw);
            var label = field.title ?? field.TargetPropertyName;

            if (field.IsRequired && string.IsNullOrWhiteSpace(raw))
                return OperationResult.Failed($"{label} الزامی است.");

            if (string.IsNullOrWhiteSpace(raw))
                continue;

            switch (field.ControlType)
            {
                case ImportContextControlType.Number:
                case ImportContextControlType.Combo when field.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped:
                    if (!long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal) || longVal <= 0)
                        return OperationResult.Failed($"{label} معتبر نیست.");
                    if (field.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped)
                    {
                        var fkError = await ValidateOrganScopedFkAsync(
                            field.FkReferenceEntity, field.FkReferenceSchema, longVal, organisationChartId);
                        if (fkError != null)
                            return OperationResult.Failed($"{label}: {fkError}");
                    }
                    dict[field.TargetPropertyName] = longVal.ToString(CultureInfo.InvariantCulture);
                    break;

                case ImportContextControlType.Date:
                    if (!TryParseDate(raw, out var dateVal))
                        return OperationResult.Failed($"{label} معتبر نیست.");
                    dict[field.TargetPropertyName] = dateVal.ToString("O", CultureInfo.InvariantCulture);
                    break;

                default:
                    dict[field.TargetPropertyName] = raw.Trim();
                    break;
            }
        }

        return null;
    }

    public async Task<List<string>> ValidateRowContextFieldsAsync(
        ImportProfile profile,
        Dictionary<string, string?> rowResolved,
        long organisationChartId)
    {
        var messages = new List<string>();
        foreach (var field in GetActiveContextFields(profile))
        {
            rowResolved.TryGetValue(field.TargetPropertyName, out var raw);
            var error = await ValidateSingleContextFieldAsync(field, raw, organisationChartId);
            if (!string.IsNullOrEmpty(error))
                messages.Add(error);
        }

        return messages;
    }

    private async Task<string?> ValidateSingleContextFieldAsync(
        ImportProfileContextField field,
        string? raw,
        long organisationChartId)
    {
        var label = field.title ?? field.TargetPropertyName;

        if (field.IsRequired && string.IsNullOrWhiteSpace(raw))
            return $"{label} الزامی است.";

        if (string.IsNullOrWhiteSpace(raw))
            return null;

        switch (field.ControlType)
        {
            case ImportContextControlType.Number:
            case ImportContextControlType.Combo when field.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped:
                if (!long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var longVal) || longVal <= 0)
                    return $"{label} معتبر نیست.";
                if (field.FkLookupType is FkLookupType.ComboKeyValue or FkLookupType.OrganScoped)
                {
                    var fkError = await ValidateOrganScopedFkAsync(
                        field.FkReferenceEntity, field.FkReferenceSchema, longVal, organisationChartId);
                    if (fkError != null)
                        return $"{label}: {fkError}";
                }
                break;

            case ImportContextControlType.Date:
                if (!TryParseDate(raw, out _))
                    return $"{label} معتبر نیست.";
                break;
        }

        return null;
    }

    public string? BuildNormalizedContextJson(string? contextJson, long organisationChartId, ImportProfile profile)
    {
        var dict = ParseContextDictionary(contextJson, organisationChartId);
        return dict == null || dict.Count == 0 ? null : JsonSerializer.Serialize(dict);
    }

    public Dictionary<string, string?>? ParseContextDictionary(string? contextJson, long organisationChartId)
    {
        Dictionary<string, string?> dict;
        try
        {
            if (string.IsNullOrWhiteSpace(contextJson))
                dict = new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            else
            {
                dict = JsonSerializer.Deserialize<Dictionary<string, string?>>(
                           contextJson,
                           new JsonSerializerOptions { PropertyNameCaseInsensitive = true })
                       ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);
            }
        }
        catch
        {
            return null;
        }

        if (organisationChartId > 0)
            dict["OrganisationChartId"] = organisationChartId.ToString(CultureInfo.InvariantCulture);

        return dict;
    }

    public long ReadLong(Dictionary<string, string?> dict, string key)
    {
        if (!dict.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return 0;
        return long.TryParse(raw, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value) ? value : 0;
    }

    public DateTime ReadDate(Dictionary<string, string?> dict, string key)
    {
        if (!dict.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return default;
        return TryParseDate(raw, out var date) ? date : default;
    }

    public long ReadOrganisationChartId(Dictionary<string, string?> dict) =>
        ReadLong(dict, "OrganisationChartId");

    private async Task<string?> ValidateOrganScopedFkAsync(
        string? fkReferenceEntity,
        string? fkReferenceSchema,
        long id,
        long organisationChartId)
    {
        if (string.IsNullOrWhiteSpace(fkReferenceEntity))
            return "مرجع FK تعریف نشده است.";

        var validator = _organFkValidators.FirstOrDefault(v => v.CanValidate(fkReferenceEntity, fkReferenceSchema));
        if (validator == null)
            return null;

        return await validator.ValidateAsync(fkReferenceEntity, id, organisationChartId, fkReferenceSchema);
    }

    private static bool TryParseDate(string raw, out DateTime date) =>
        ImportDateParser.TryParse(raw, out date);
}
