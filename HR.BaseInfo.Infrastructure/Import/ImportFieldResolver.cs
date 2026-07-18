using System.Globalization;
using System.Text.Json;
using System.Text.RegularExpressions;
using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Import;
using HR.SharedKernel.Service;

namespace HR.BaseInfo.infrastructure.Import;

public class ImportFieldResolver(ImportEmployeeLookupService employeeLookup) : IScopedServices
{
    public (string? Value, string? Error) ResolveNationalNo(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var nationalNo = ImportEmployeeLookupService.NormalizeNationalNo(raw);
        if (string.IsNullOrEmpty(nationalNo) || nationalNo.Length != 10)
            return (null, "کد ملی معتبر نیست (باید ۱۰ رقم باشد).");

        return (nationalNo, null);
    }

    public (string? Value, string? Error) NormalizeLongAmount(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(raw);
        var digitsOnly = Regex.Replace(normalized, @"[^\d]", "");
        if (string.IsNullOrEmpty(digitsOnly))
            return (null, $"{LabelFor(field)} باید عدد باشد.");

        if (!long.TryParse(digitsOnly, NumberStyles.None, CultureInfo.InvariantCulture, out var value))
            return (null, $"{LabelFor(field)} خارج از محدوده مجاز است.");

        if (value <= 0)
            return (null, $"{LabelFor(field)} باید بزرگ‌تر از صفر باشد.");

        return (value.ToString(CultureInfo.InvariantCulture), null);
    }

    public (string? Value, string? Error) NormalizeInteger(string? raw, ImportProfileField field, bool allowZero = false)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim();
        if (!int.TryParse(normalized, NumberStyles.Integer, CultureInfo.InvariantCulture, out var value))
            return (null, $"{LabelFor(field)} باید عدد صحیح باشد.");

        if (!allowZero && value <= 0)
            return (null, $"{LabelFor(field)} باید بزرگ‌تر از صفر باشد.");

        if (allowZero && value < 0)
            return (null, $"{LabelFor(field)} نمی‌تواند منفی باشد.");

        return (value.ToString(CultureInfo.InvariantCulture), null);
    }

    public (string? Value, string? Error) NormalizeDecimal(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var normalized = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().Replace(",", ".");
        if (!decimal.TryParse(normalized, NumberStyles.Number, CultureInfo.InvariantCulture, out var value))
            return (null, $"{LabelFor(field)} باید عدد باشد.");

        if (value < 0)
            return (null, $"{LabelFor(field)} نمی‌تواند منفی باشد.");

        return (value.ToString(CultureInfo.InvariantCulture), null);
    }

    public (string? Value, string? Error) NormalizeText(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        return (ExcelImportParser.NormalizeText(raw), null);
    }

    public (string? Value, string? Error) NormalizeBool(string? raw, ImportProfileField field)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (field.IsRequired)
                return (null, $"{LabelFor(field)} الزامی است.");
            return (null, null);
        }

        var v = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim().ToLowerInvariant();
        var ok = v is "1" or "0" or "true" or "false" or "yes" or "no" or "بله" or "خیر";
        return ok ? (v, null) : (raw.Trim(), $"{LabelFor(field)} باید بله/خیر یا ۱/۰ باشد.");
    }

    public (string? Value, string? Error) NormalizeProfileField(string? raw, ImportProfileField field)
    {
        if (field.FkLookupType == FkLookupType.NationalNo)
            return ResolveNationalNo(raw, field);

        var dataType = (field.DataType ?? "Text").Trim();
        if (string.Equals(dataType, "Long", StringComparison.OrdinalIgnoreCase))
            return NormalizeLongAmount(raw, field);
        if (string.Equals(dataType, "Int", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Integer", StringComparison.OrdinalIgnoreCase))
            return NormalizeInteger(raw, field, allowZero: false);
        if (string.Equals(dataType, "Year", StringComparison.OrdinalIgnoreCase))
            return NormalizeInteger(raw, field, allowZero: false);
        if (string.Equals(dataType, "Decimal", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Double", StringComparison.OrdinalIgnoreCase))
            return NormalizeDecimal(raw, field);
        if (string.Equals(dataType, "Bool", StringComparison.OrdinalIgnoreCase)
            || string.Equals(dataType, "Boolean", StringComparison.OrdinalIgnoreCase))
            return NormalizeBool(raw, field);
        if (ImportDateParser.IsDateDataType(dataType))
            return ImportDateParser.NormalizeToIsoDate(
                raw,
                field.IsRequired,
                LabelFor(field));

        return NormalizeText(raw, field);
    }

    public long ResolveEmployeeId(string? nationalNo) =>
        string.IsNullOrWhiteSpace(nationalNo) ? 0 : employeeLookup.GetEmployeeIdFromNationalNo(nationalNo);

    public string? ResolveEmployeeFullName(long employeeId) =>
        employeeId > 0 ? employeeLookup.GetEmployeeFullName(employeeId) : null;

    private static string LabelFor(ImportProfileField field) =>
        field.ExcelColumnHeader ?? field.title ?? field.TargetPropertyName;
}
