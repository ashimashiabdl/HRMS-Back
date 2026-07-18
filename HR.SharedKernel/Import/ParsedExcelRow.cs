namespace HR.SharedKernel.Import;

public sealed class ParsedExcelRow
{
    public int RowNumber { get; init; }
    public Dictionary<int, string> Cells { get; init; } = new();
}
