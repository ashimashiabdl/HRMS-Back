using System.Globalization;
using System.Text;
using System.Text.RegularExpressions;
using OfficeOpenXml;

namespace HR.SharedKernel.Import;

public static class ExcelImportParser
{
    static ExcelImportParser()
    {
        ExcelPackage.License.SetNonCommercialOrganization("HRMS");
    }

    public static List<ParsedExcelRow> Parse(byte[] fileContent, string extension, bool hasHeaderRow)
    {
        var ext = (extension ?? "").Trim().ToLowerInvariant();
        if (ext == ".csv")
            return ParseCsv(fileContent, hasHeaderRow);
        if (ext is ".xlsx" or ".xls")
            return ParseXlsx(fileContent, hasHeaderRow);
        throw new InvalidOperationException($"فرمت فایل پشتیبانی نمی‌شود: {extension}");
    }

    public static List<ExcelHeaderCell> ReadHeaders(byte[] fileContent, string extension)
    {
        var ext = (extension ?? "").Trim().ToLowerInvariant();
        if (ext == ".csv")
            return ReadCsvHeaders(fileContent);
        if (ext is ".xlsx" or ".xls")
            return ReadXlsxHeaders(fileContent);
        throw new InvalidOperationException($"فرمت فایل پشتیبانی نمی‌شود: {extension}");
    }

    private static List<ExcelHeaderCell> ReadCsvHeaders(byte[] fileContent)
    {
        using var reader = new StreamReader(new MemoryStream(fileContent), Encoding.UTF8);
        var line = reader.ReadLine();
        if (string.IsNullOrWhiteSpace(line))
            return [];

        var parts = SplitCsvLine(line);
        var headers = new List<ExcelHeaderCell>();
        for (var i = 0; i < parts.Count; i++)
        {
            var header = parts[i].Trim();
            if (!string.IsNullOrEmpty(header))
                headers.Add(new ExcelHeaderCell { ColumnIndex = i + 1, Header = header });
        }
        return headers;
    }

    private static List<ExcelHeaderCell> ReadXlsxHeaders(byte[] fileContent)
    {
        using var stream = new MemoryStream(fileContent);
        using var package = new ExcelPackage(stream);
        var sheet = package.Workbook.Worksheets.FirstOrDefault()
            ?? throw new InvalidOperationException("فایل اکسل فاقد برگه است.");

        if (sheet.Dimension == null)
            return [];

        var headers = new List<ExcelHeaderCell>();
        for (var c = 1; c <= sheet.Dimension.End.Column; c++)
        {
            var value = sheet.Cells[1, c].Text?.Trim() ?? "";
            if (!string.IsNullOrEmpty(value))
                headers.Add(new ExcelHeaderCell { ColumnIndex = c, Header = value });
        }
        return headers;
    }

    private static List<ParsedExcelRow> ParseCsv(byte[] fileContent, bool hasHeaderRow)
    {
        var rows = new List<ParsedExcelRow>();
        using var reader = new StreamReader(new MemoryStream(fileContent), Encoding.UTF8);
        string? line;
        var lineNo = 0;
        while ((line = reader.ReadLine()) != null)
        {
            lineNo++;
            if (string.IsNullOrWhiteSpace(line))
                continue;
            if (hasHeaderRow && lineNo == 1)
                continue;

            var parts = SplitCsvLine(line);
            var cells = new Dictionary<int, string>();
            for (var i = 0; i < parts.Count; i++)
                cells[i + 1] = parts[i].Trim();

            if (cells.Values.All(string.IsNullOrWhiteSpace))
                continue;

            rows.Add(new ParsedExcelRow { RowNumber = lineNo, Cells = cells });
        }
        return rows;
    }

    private static List<string> SplitCsvLine(string line)
    {
        var result = new List<string>();
        var current = new StringBuilder();
        var inQuotes = false;
        for (var i = 0; i < line.Length; i++)
        {
            var c = line[i];
            if (c == '"')
            {
                inQuotes = !inQuotes;
                continue;
            }
            if (c == ',' && !inQuotes)
            {
                result.Add(current.ToString());
                current.Clear();
                continue;
            }
            current.Append(c);
        }
        result.Add(current.ToString());
        return result;
    }

    private static List<ParsedExcelRow> ParseXlsx(byte[] fileContent, bool hasHeaderRow)
    {
        var rows = new List<ParsedExcelRow>();
        using var stream = new MemoryStream(fileContent);
        using var package = new ExcelPackage(stream);
        var sheet = package.Workbook.Worksheets.FirstOrDefault()
            ?? throw new InvalidOperationException("فایل اکسل فاقد برگه است.");

        if (sheet.Dimension == null)
            return rows;

        var startRow = hasHeaderRow ? 2 : 1;
        for (var r = startRow; r <= sheet.Dimension.End.Row; r++)
        {
            var cells = new Dictionary<int, string>();
            for (var c = 1; c <= sheet.Dimension.End.Column; c++)
            {
                var value = sheet.Cells[r, c].Text?.Trim() ?? "";
                if (!string.IsNullOrEmpty(value))
                    cells[c] = value;
            }

            if (cells.Count == 0)
                continue;

            rows.Add(new ParsedExcelRow { RowNumber = r, Cells = cells });
        }
        return rows;
    }

    public static string NormalizeDigitsToEnglish(string? input)
    {
        if (string.IsNullOrWhiteSpace(input)) return "";
        var sb = new StringBuilder(input.Length);
        foreach (var c in input)
        {
            if (c >= '\u06F0' && c <= '\u06F9') sb.Append((char)('0' + (c - '\u06F0')));
            else if (c >= '\u0660' && c <= '\u0669') sb.Append((char)('0' + (c - '\u0660')));
            else sb.Append(c);
        }
        return sb.ToString();
    }

    public static string? NormalizeText(string? raw)
    {
        if (raw == null) return null;
        var normalized = NormalizeDigitsToEnglish(raw).Trim();
        return string.IsNullOrWhiteSpace(normalized) ? null : normalized;
    }

    public static string ColumnIndexToLetter(int columnIndex)
    {
        if (columnIndex <= 0)
            return "";
        var result = "";
        var n = columnIndex;
        while (n > 0)
        {
            n--;
            result = (char)('A' + (n % 26)) + result;
            n /= 26;
        }
        return result;
    }
}
