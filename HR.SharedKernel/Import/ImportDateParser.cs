using System.Globalization;
using System.Text.RegularExpressions;

namespace HR.SharedKernel.Import;

/// <summary>
/// Parses Import Excel/context date values: Gregorian as-is, Persian (Jalali) converted to Gregorian.
/// </summary>
public static class ImportDateParser
{
    private static readonly Regex StructuredDateRegex = new(
        @"^(?<y>\d{3,4})\s*[/\-.\s]\s*(?<m>\d{1,2})\s*[/\-.\s]\s*(?<d>\d{1,2})(?:\s+\d{1,2}:\d{1,2}(?::\d{1,2})?)?$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex CompactDateRegex = new(
        @"^(?<y>\d{4})(?<m>\d{2})(?<d>\d{2})$",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    /// <summary>
    /// Tries to parse a date string. Persian years 1300–1500 convert via PersianCalendar;
    /// Gregorian years (and Excel serial / standard formats) stay Gregorian.
    /// </summary>
    public static bool TryParse(string? raw, out DateTime date)
    {
        date = default;
        if (string.IsNullOrWhiteSpace(raw))
            return false;

        var s = ExcelImportParser.NormalizeDigitsToEnglish(raw).Trim();
        if (string.IsNullOrWhiteSpace(s))
            return false;

        // Excel OLE Automation date serial (e.g. 45321.0)
        if (double.TryParse(s, NumberStyles.Float, CultureInfo.InvariantCulture, out var oa)
            && oa is >= 20000 and < 80000
            && !s.Contains('/') && !s.Contains('-') && !s.Contains('.')
            && s.Length <= 7)
        {
            try
            {
                date = DateTime.FromOADate(oa).Date;
                return true;
            }
            catch
            {
                // fall through
            }
        }

        if (TryParseYearMonthDay(s, out var year, out var month, out var day))
        {
            if (IsPersianYear(year))
                return TryFromPersian(year, month, day, out date);

            if (IsGregorianYear(year))
                return TryFromGregorian(year, month, day, out date);
        }

        if (DateTime.TryParse(s, CultureInfo.InvariantCulture, DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.AssumeLocal, out date))
        {
            date = date.Date;
            return true;
        }

        if (DateTime.TryParse(s, CultureInfo.CurrentCulture, DateTimeStyles.AllowWhiteSpaces, out date))
        {
            date = date.Date;
            return true;
        }

        return false;
    }

    public static DateTime ParseRequired(string raw)
    {
        if (!TryParse(raw, out var date))
            throw new FormatException($"Invalid import date: '{raw}'");
        return date;
    }

    public static DateTime? ParseNullable(string? raw) =>
        string.IsNullOrWhiteSpace(raw) ? null : ParseRequired(raw);

    /// <summary>
    /// Normalizes a date field to ISO Gregorian <c>yyyy-MM-dd</c> for ResolvedDataJson / finalize.
    /// </summary>
    public static (string? Value, string? Error) NormalizeToIsoDate(string? raw, bool isRequired, string label)
    {
        if (string.IsNullOrWhiteSpace(raw))
        {
            if (isRequired)
                return (null, $"{label} الزامی است.");
            return (null, null);
        }

        if (!TryParse(raw, out var date))
            return (null, $"{label} تاریخ معتبر نیست (میلادی یا شمسی).");

        return (date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture), null);
    }

    public static bool IsDateDataType(string? dataType)
    {
        var t = (dataType ?? "").Trim();
        return string.Equals(t, "Date", StringComparison.OrdinalIgnoreCase)
               || string.Equals(t, "DateTime", StringComparison.OrdinalIgnoreCase)
               || string.Equals(t, "DateOnly", StringComparison.OrdinalIgnoreCase);
    }

    private static bool TryParseYearMonthDay(string s, out int year, out int month, out int day)
    {
        year = month = day = 0;

        var match = StructuredDateRegex.Match(s);
        if (!match.Success)
            match = CompactDateRegex.Match(s);
        if (!match.Success)
            return false;

        if (!int.TryParse(match.Groups["y"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out year)
            || !int.TryParse(match.Groups["m"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out month)
            || !int.TryParse(match.Groups["d"].Value, NumberStyles.None, CultureInfo.InvariantCulture, out day))
            return false;

        return month is >= 1 and <= 12 && day is >= 1 and <= 31;
    }

    private static bool IsPersianYear(int year) => year is >= 1300 and <= 1500;

    private static bool IsGregorianYear(int year) => year is >= 1900 and <= 2200;

    private static bool TryFromPersian(int year, int month, int day, out DateTime date)
    {
        date = default;
        try
        {
            var pc = new PersianCalendar();
            date = pc.ToDateTime(year, month, day, 0, 0, 0, 0).Date;
            return true;
        }
        catch
        {
            return false;
        }
    }

    private static bool TryFromGregorian(int year, int month, int day, out DateTime date)
    {
        date = default;
        try
        {
            date = new DateTime(year, month, day);
            return true;
        }
        catch
        {
            return false;
        }
    }
}
