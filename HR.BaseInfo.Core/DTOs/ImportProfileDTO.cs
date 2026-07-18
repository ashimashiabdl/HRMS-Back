using HR.SharedKernel.Import;

namespace HR.BaseInfo.Core.DTOs;

public class ImportProfileListItemDTO
{
    public long Id { get; set; }
    public string? title { get; set; }
    public string? TargetEntityName { get; set; }
    public string? TargetSchema { get; set; }
    public string? ModuleKey { get; set; }
    public ImportHandlerType HandlerType { get; set; }
    public bool RequiresEmployeeLookup { get; set; }
    public string? AllowedExtensions { get; set; }
    public string? Description { get; set; }
}

public class ImportProfileFieldDTO
{
    public long Id { get; set; }
    public int ExcelColumnOrder { get; set; }
    public string? ExcelColumnHeader { get; set; }
    public string? TargetPropertyName { get; set; }
    public string? DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUniqueKey { get; set; }
    public int DisplayOrder { get; set; }
}

public class ImportProfileDetailDTO : ImportProfileListItemDTO
{
    public bool HasHeaderRow { get; set; }
    public int MaxRowCount { get; set; }
    public List<ImportProfileFieldDTO> Fields { get; set; } = new();
    public List<ImportProfileContextFieldDTO> ContextFields { get; set; } = new();
}
