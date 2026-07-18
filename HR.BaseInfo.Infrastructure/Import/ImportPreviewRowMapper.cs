using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Import;

namespace HR.BaseInfo.infrastructure.Import;

public static class ImportPreviewRowMapper
{
    public static Dictionary<string, object?> MapRow(
        ImportTempRow row,
        IReadOnlyList<ImportProfileField> profileFields,
        bool includeAdminFields = false)
    {
        var resolved = DeserializeDict(row.ResolvedDataJson);
        var parsed = DeserializeDict(row.ParsedDataJson);
        var msgs = DeserializeMessages(row.ValidationMessagesJson);

        var rowDict = new Dictionary<string, object?>(StringComparer.OrdinalIgnoreCase)
        {
            ["id"] = row.Id,
            ["rowNumber"] = row.RowNumber,
            ["validationStatus"] = row.ValidationStatus,
            ["validationMessage"] = msgs.Count > 0 ? string.Join(" | ", msgs) : null,
        };

        foreach (var field in profileFields)
        {
            var key = ToCamelCase(field.TargetPropertyName);
            resolved.TryGetValue(field.TargetPropertyName, out var value);
            if (string.IsNullOrWhiteSpace(value))
                parsed.TryGetValue(field.TargetPropertyName, out value);
            rowDict[key] = value;
        }

        if (resolved.TryGetValue("EmployeeFullName", out var employeeFullName) && !string.IsNullOrWhiteSpace(employeeFullName))
            rowDict["employeeFullName"] = employeeFullName;

        if (!rowDict.ContainsKey("title") || rowDict["title"] == null)
            rowDict["title"] = row.title;

        if (includeAdminFields)
        {
            rowDict["validationStatusTitle"] = ImportDisplayHelper.GetValidationStatusTitle(row.ValidationStatus);
            rowDict["mainRecordId"] = row.MainRecordId;
        }

        return rowDict;
    }

    public static string ToCamelCase(string propertyName)
    {
        if (string.IsNullOrEmpty(propertyName))
            return propertyName;
        if (propertyName.Length == 1)
            return propertyName.ToLowerInvariant();
        return char.ToLowerInvariant(propertyName[0]) + propertyName[1..];
    }

    private static Dictionary<string, string?> DeserializeDict(string? json) =>
        string.IsNullOrEmpty(json)
            ? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase)
            : JsonSerializer.Deserialize<Dictionary<string, string?>>(json)
              ?? new Dictionary<string, string?>(StringComparer.OrdinalIgnoreCase);

    private static List<string> DeserializeMessages(string? json)
    {
        if (string.IsNullOrWhiteSpace(json))
            return [];
        try
        {
            return JsonSerializer.Deserialize<List<string>>(json) ?? [];
        }
        catch
        {
            return [json];
        }
    }
}
