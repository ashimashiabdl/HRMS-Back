namespace HR.SharedKernel.Import;

public class ExcelHeaderCell
{
    public int ColumnIndex { get; set; }
    public string Header { get; set; } = "";
}

public class ImportColumnMappingItem
{
    public string TargetPropertyName { get; set; } = "";
    public int ExcelColumnOrder { get; set; }
}

public class ImportColumnMappingSuggestion
{
    public long ProfileFieldId { get; set; }
    public string TargetPropertyName { get; set; } = "";
    public string? ExpectedHeader { get; set; }
    public int? SuggestedExcelColumnOrder { get; set; }
    public string? MatchedFileHeader { get; set; }
    public int MatchScore { get; set; }
    public bool IsRequired { get; set; }
}

public class ImportDetectColumnsResult
{
    public List<ExcelHeaderCell> FileHeaders { get; set; } = new();
    public List<ImportColumnMappingSuggestion> Suggestions { get; set; } = new();
    public bool IsFullyMapped { get; set; }
}

public static class ImportColumnMapper
{
    public static List<ImportColumnMappingSuggestion> SuggestMappings(
        IReadOnlyList<ExcelHeaderCell> fileHeaders,
        IEnumerable<(long Id, string TargetPropertyName, string? ExcelColumnHeader, int ExcelColumnOrder, bool IsRequired)> profileFields)
    {
        var suggestions = new List<ImportColumnMappingSuggestion>();
        var usedColumns = new HashSet<int>();

        foreach (var field in profileFields.OrderBy(f => f.ExcelColumnOrder))
        {
            var suggestion = new ImportColumnMappingSuggestion
            {
                ProfileFieldId = field.Id,
                TargetPropertyName = field.TargetPropertyName,
                ExpectedHeader = field.ExcelColumnHeader ?? field.TargetPropertyName,
                IsRequired = field.IsRequired,
            };

            var best = FindBestHeaderMatch(fileHeaders, field, usedColumns);
            if (best != null)
            {
                suggestion.SuggestedExcelColumnOrder = best.Value.ColumnIndex;
                suggestion.MatchedFileHeader = best.Value.Header;
                suggestion.MatchScore = best.Value.Score;
                usedColumns.Add(best.Value.ColumnIndex);
            }
            else if (fileHeaders.Any(h => h.ColumnIndex == field.ExcelColumnOrder))
            {
                var fallback = fileHeaders.First(h => h.ColumnIndex == field.ExcelColumnOrder);
                suggestion.SuggestedExcelColumnOrder = fallback.ColumnIndex;
                suggestion.MatchedFileHeader = fallback.Header;
                suggestion.MatchScore = 40;
                usedColumns.Add(fallback.ColumnIndex);
            }

            suggestions.Add(suggestion);
        }

        return suggestions;
    }

    public static bool IsFullyMapped(IEnumerable<ImportColumnMappingSuggestion> suggestions) =>
        suggestions.Where(s => s.IsRequired).All(s => s.SuggestedExcelColumnOrder is > 0);

    private static (int ColumnIndex, string Header, int Score)? FindBestHeaderMatch(
        IReadOnlyList<ExcelHeaderCell> fileHeaders,
        (long Id, string TargetPropertyName, string? ExcelColumnHeader, int ExcelColumnOrder, bool IsRequired) field,
        HashSet<int> usedColumns)
    {
        (int ColumnIndex, string Header, int Score)? best = null;

        foreach (var header in fileHeaders)
        {
            if (usedColumns.Contains(header.ColumnIndex))
                continue;

            var score = ScoreHeaderMatch(header.Header, field.ExcelColumnHeader, field.TargetPropertyName);
            if (score <= 0)
                continue;

            if (best == null || score > best.Value.Score)
                best = (header.ColumnIndex, header.Header, score);
        }

        return best;
    }

    private static int ScoreHeaderMatch(string fileHeader, string? expectedHeader, string targetProperty)
    {
        var file = NormalizeHeader(fileHeader);
        if (string.IsNullOrEmpty(file))
            return 0;

        var expected = NormalizeHeader(expectedHeader ?? "");
        if (!string.IsNullOrEmpty(expected) && file == expected)
            return 100;

        var target = NormalizeHeader(targetProperty);
        if (!string.IsNullOrEmpty(target) && file == target)
            return 90;

        if (!string.IsNullOrEmpty(expected) && (file.Contains(expected) || expected.Contains(file)))
            return 75;

        return 0;
    }

    private static string NormalizeHeader(string value) =>
        ExcelImportParser.NormalizeDigitsToEnglish(value).Trim().ToLowerInvariant()
            .Replace(" ", "").Replace("_", "").Replace("-", "");
}
