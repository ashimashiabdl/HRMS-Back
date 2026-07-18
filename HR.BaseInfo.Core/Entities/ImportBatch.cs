using HR.SharedKernel.Import;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Import_Batch", Schema = "bas")]
public class ImportBatch : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey(nameof(ImportProfile))]
    public long ImportProfileId { get; set; }
    public virtual ImportProfile? ImportProfile { get; set; }

    public long FileId { get; set; }

    [StringLength(4000)]
    public string? ContextJson { get; set; }

    public ImportContextMode ContextMode { get; set; } = ImportContextMode.BatchContext;

    public ImportBatchStatus Status { get; set; } = ImportBatchStatus.Draft;

    public int TotalRowsRead { get; set; }
    public int ValidCount { get; set; }
    public int WarningCount { get; set; }
    public int ErrorCount { get; set; }
    public int InsertedCount { get; set; }

    [StringLength(4000)]
    public string? FailedRowsJson { get; set; }

    [StringLength(256)]
    public string? UploaderUserName { get; set; }

    [StringLength(256)]
    public string? UploaderDisplayName { get; set; }

    [StringLength(2000)]
    public string? ErrorMessage { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CompletedAt { get; set; }
}
