using System.Globalization;
using System.Reflection;
using System.Text.Json;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Import;
using HR.Order.Core.Data;
using HR.Order.Infrastructure.Data;
using HR.SharedKernel.Import;
using Microsoft.EntityFrameworkCore;

namespace HR.Payroll.Infrastructure.Import;

/// <summary>
/// Shared helpers for Order.* import handlers (employee resolve without unique NationalNo-in-file constraint).
/// </summary>
internal static class OrderImportShared
{
    public static readonly HashSet<string> ReservedEmployeeKeys = new(StringComparer.OrdinalIgnoreCase)
    {
        "NationalNo", "EmployeeId", "EmployeeFullName",
        "InterdictOrderCode", "InterdictOrderId", "RecruitOrderId",
    };

    public static async Task ResolveEmployeeRowsAllowingDuplicateNationalNoAsync(
        ImportProfile profile,
        List<ImportTempRow> tempRows,
        ImportFieldResolver fieldResolver,
        ImportContextService? contextService = null,
        ImportContextMode contextMode = ImportContextMode.BatchContext,
        long organisationChartId = 0)
    {
        var fields = ImportEffectiveFieldsHelper.GetEffectiveRowFields(profile, contextMode);
        var contextFields = profile.ContextFields.Where(f => !f.IsDeleted).ToList();

        foreach (var row in tempRows)
        {
            var parsed = EmployeeLinkedImportHelper.DeserializeDict(row.ParsedDataJson);
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
            row.ValidationMessagesJson = messages.Count == 0 ? null : JsonSerializer.Serialize(messages);
        }
    }

    public static long ResolvePayLocationId(Dictionary<string, string?> rowContext, long organisationChartId)
    {
        var fromCtx = ReadLong(rowContext, "PayLocationId");
        if (fromCtx > 0)
            return fromCtx;
        return organisationChartId;
    }

    public static async Task<RecruitOrder?> FindRecruitOrderAsync(
        OrderContext orderCtx,
        long employeeId,
        long payLocationId)
    {
        return await orderCtx.RecruitOrders
            .Where(r => !r.IsDeleted && r.EmployeeId == employeeId && r.PayLocationId == payLocationId)
            .OrderByDescending(r => r.Id)
            .FirstOrDefaultAsync();
    }

    public static async Task<InterdictOrder?> FindInterdictOrderAsync(
        OrderContext orderCtx,
        Dictionary<string, string?> resolved,
        long employeeId,
        long payLocationId)
    {
        var byId = ReadLong(resolved, "InterdictOrderId");
        if (byId > 0)
        {
            return await orderCtx.InterdictOrders
                .Where(i => !i.IsDeleted && i.Id == byId)
                .FirstOrDefaultAsync();
        }

        resolved.TryGetValue("InterdictOrderCode", out var code);
        code = ExcelImportParser.NormalizeText(code);
        if (!string.IsNullOrWhiteSpace(code))
        {
            return await orderCtx.InterdictOrders
                .Where(i => !i.IsDeleted && i.Code == code)
                .OrderByDescending(i => i.Id)
                .FirstOrDefaultAsync();
        }

        var serial = ReadShort(resolved, "Serial");
        var recruit = await FindRecruitOrderAsync(orderCtx, employeeId, payLocationId);
        if (recruit == null)
            return null;

        var query = orderCtx.InterdictOrders
            .Where(i => !i.IsDeleted && i.RecruitOrderId == recruit.Id);

        if (serial is > 0)
            query = query.Where(i => i.Serial == serial);

        return await query.OrderByDescending(i => i.Id).FirstOrDefaultAsync();
    }

    public static long? ReadNullableLong(
        Dictionary<string, string?> rowContext,
        Dictionary<string, string?> resolved,
        string key)
    {
        var fromCtx = ReadLong(rowContext, key);
        if (fromCtx > 0)
            return fromCtx;
        var fromRow = ReadLong(resolved, key);
        return fromRow > 0 ? fromRow : null;
    }

    public static long ReadLong(Dictionary<string, string?> dict, string key)
    {
        if (!dict.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return 0;
        return long.TryParse(
            ExcelImportParser.NormalizeDigitsToEnglish(raw),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var val)
            ? val
            : 0;
    }

    public static short? ReadShort(Dictionary<string, string?> dict, string key)
    {
        if (!dict.TryGetValue(key, out var raw) || string.IsNullOrWhiteSpace(raw))
            return null;
        return short.TryParse(
            ExcelImportParser.NormalizeDigitsToEnglish(raw),
            NumberStyles.Integer,
            CultureInfo.InvariantCulture,
            out var val)
            ? val
            : null;
    }

    public static bool TrySetPropertyFromString(
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
                _ when prop.PropertyType == typeof(Guid) => Guid.Parse(ExcelImportParser.NormalizeText(rawValue)!),
                _ when prop.PropertyType == typeof(Guid?) => Guid.Parse(ExcelImportParser.NormalizeText(rawValue)!),
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

    public static object? ParseNumericOrString(Type propertyType, string rawValue, string? dataType)
    {
        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(rawValue).Trim();
        var typeHint = (dataType ?? "").Trim();
        var underlying = Nullable.GetUnderlyingType(propertyType) ?? propertyType;

        if (underlying == typeof(string))
            return ExcelImportParser.NormalizeText(rawValue);

        if (underlying == typeof(byte) || string.Equals(typeHint, "Byte", StringComparison.OrdinalIgnoreCase))
            return byte.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(short) || string.Equals(typeHint, "Short", StringComparison.OrdinalIgnoreCase))
            return short.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(int) || string.Equals(typeHint, "Int", StringComparison.OrdinalIgnoreCase))
            return int.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(long) || string.Equals(typeHint, "Long", StringComparison.OrdinalIgnoreCase))
            return long.Parse(normalized, CultureInfo.InvariantCulture);

        if (underlying == typeof(decimal) || string.Equals(typeHint, "Decimal", StringComparison.OrdinalIgnoreCase))
            return decimal.Parse(normalized.Replace(",", "."), NumberStyles.Number, CultureInfo.InvariantCulture);

        if (underlying == typeof(float) || string.Equals(typeHint, "Float", StringComparison.OrdinalIgnoreCase))
            return float.Parse(normalized.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

        if (underlying == typeof(double) || string.Equals(typeHint, "Double", StringComparison.OrdinalIgnoreCase))
            return double.Parse(normalized.Replace(",", "."), NumberStyles.Float, CultureInfo.InvariantCulture);

        return Convert.ChangeType(normalized, underlying, CultureInfo.InvariantCulture);
    }

    public static void ApplyMappedProperties(
        Type entityType,
        object entity,
        ImportProfile profile,
        IEnumerable<ImportProfileContextField> contextFields,
        Dictionary<string, string?> rowContext,
        Dictionary<string, string?> resolved,
        HashSet<string> reservedKeys,
        HashSet<string> profileFieldNames,
        Action<string> onError)
    {
        foreach (var ctxField in contextFields)
        {
            rowContext.TryGetValue(ctxField.TargetPropertyName, out var raw);
            if (string.IsNullOrWhiteSpace(raw))
                continue;

            if (!TrySetPropertyFromString(entityType, entity, ctxField.TargetPropertyName, raw, ctxField.DataType, out var ctxError))
            {
                onError(ctxError);
                return;
            }
        }

        foreach (var (propertyName, rawValue) in resolved)
        {
            if (reservedKeys.Contains(propertyName) || string.IsNullOrWhiteSpace(rawValue))
                continue;
            if (profileFieldNames.Count > 0 && !profileFieldNames.Contains(propertyName))
                continue;

            var profileField = profile.Fields.FirstOrDefault(f =>
                !f.IsDeleted && string.Equals(f.TargetPropertyName, propertyName, StringComparison.OrdinalIgnoreCase));

            if (!TrySetPropertyFromString(entityType, entity, propertyName, rawValue, profileField?.DataType, out var rowError))
            {
                onError(rowError);
                return;
            }
        }
    }

    public static void MarkRowError(ImportTempRow row, string message)
    {
        row.ValidationStatus = ImportValidationStatus.Error;
        row.ValidationMessagesJson = JsonSerializer.Serialize(new[] { message });
    }
}
