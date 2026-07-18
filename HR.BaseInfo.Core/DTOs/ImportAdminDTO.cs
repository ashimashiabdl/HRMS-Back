using HR.SharedKernel.Data;
using HR.SharedKernel.Import;

namespace HR.BaseInfo.Core.DTOs;

public class ImportProfileCrudDTO : BaseDTO
{
    public string? TargetEntityName { get; set; }
    public string? TargetSchema { get; set; }
    public string? ModuleKey { get; set; }
    public ImportHandlerType HandlerType { get; set; } = ImportHandlerType.Simple;
    public string? CustomHandlerKey { get; set; }
    public bool RequiresEmployeeLookup { get; set; }
    public string? AllowedExtensions { get; set; } = ".xlsx,.csv";
    public int MaxRowCount { get; set; } = 10000;
    public bool HasHeaderRow { get; set; } = true;
    public string? PermissionKey { get; set; }
    public bool IsActive { get; set; } = true;
    public string? Description { get; set; }
}

public class ImportProfileFieldCrudDTO : BaseDTO
{
    public long ImportProfileId { get; set; }
    public string? ImportProfileTitle { get; set; }
    public int ExcelColumnOrder { get; set; }
    public string? ExcelColumnLetter { get; set; }
    public string? ExcelColumnHeader { get; set; }
    public string? TargetPropertyName { get; set; }
    public string? DataType { get; set; }
    public bool IsRequired { get; set; }
    public bool IsUniqueKey { get; set; }
    public FkLookupType FkLookupType { get; set; } = FkLookupType.None;
    public string? FkReferenceEntity { get; set; }
    public string? FkReferenceField { get; set; }
    public int DisplayOrder { get; set; }
}

public class ImportProfileContextFieldCrudDTO : BaseDTO
{
    public long ImportProfileId { get; set; }
    public string? ImportProfileTitle { get; set; }
    public string? TargetPropertyName { get; set; }
    public string? DataType { get; set; }
    public ImportContextControlType ControlType { get; set; } = ImportContextControlType.Text;
    public bool IsRequired { get; set; }
    public FkLookupType FkLookupType { get; set; } = FkLookupType.None;
    public string? FkReferenceEntity { get; set; }
    public string? FkReferenceSchema { get; set; }
    public int DisplayOrder { get; set; }
    public string? ControlTypeTitle { get; set; }
    public string? FkLookupTypeTitle { get; set; }
}

public class ImportProfileContextFieldDTO
{
    public long Id { get; set; }
    public string? title { get; set; }
    public string? TargetPropertyName { get; set; }
    public string? DataType { get; set; }
    public ImportContextControlType ControlType { get; set; }
    public bool IsRequired { get; set; }
    public FkLookupType FkLookupType { get; set; }
    public string? FkReferenceEntity { get; set; }
    public string? FkReferenceSchema { get; set; }
    public int DisplayOrder { get; set; }
}

public class ImportBatchListDTO
{
    public long Id { get; set; }
    public string? title { get; set; }
    public DateTime? CreateDate { get; set; }
    public long ImportProfileId { get; set; }
    public string? ImportProfileTitle { get; set; }
    public string? TargetEntityName { get; set; }
    public long FileId { get; set; }
    public string? FileTitle { get; set; }
    public string? FileExtension { get; set; }
    public ImportBatchStatus Status { get; set; }
    public string? StatusTitle { get; set; }
    public int TotalRowsRead { get; set; }
    public int ValidCount { get; set; }
    public int WarningCount { get; set; }
    public int ErrorCount { get; set; }
    public int InsertedCount { get; set; }
    public string? UploaderDisplayName { get; set; }
    public string? UploaderUserName { get; set; }
    public DateTime? CompletedAt { get; set; }
    public string? ErrorMessage { get; set; }
}

public class ImportBatchDetailDTO : ImportBatchListDTO
{
    public string? ContextJson { get; set; }
    public string? FailedRowsJson { get; set; }
}

public class ImportFileDownloadDTO
{
    public long Id { get; set; }
    public string? FileName { get; set; }
    public byte[]? Content { get; set; }
    public string? MimeType { get; set; }
    public string? Extension { get; set; }
    public long Size { get; set; }
}

public class ImportTempRowListDTO
{
    public long Id { get; set; }
    public int RowNumber { get; set; }
    public string? title { get; set; }
    public string? Description { get; set; }
    public ImportValidationStatus ValidationStatus { get; set; }
    public string? ValidationStatusTitle { get; set; }
    public string? ValidationMessage { get; set; }
    public string? RawDataJson { get; set; }
    public string? ResolvedDataJson { get; set; }
    public long? MainRecordId { get; set; }
    public DateTime? FinalizedAt { get; set; }
}
