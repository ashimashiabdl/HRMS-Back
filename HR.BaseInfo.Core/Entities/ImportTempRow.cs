using HR.SharedKernel.Import;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Import_Temp_Row", Schema = "bas")]
public class ImportTempRow : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey(nameof(ImportBatch))]
    public long ImportBatchId { get; set; }
    public virtual ImportBatch? ImportBatch { get; set; }

    public int RowNumber { get; set; }

    [StringLength(4000)]
    public string? RawDataJson { get; set; }

    [StringLength(4000)]
    public string? ParsedDataJson { get; set; }

    [StringLength(4000)]
    public string? ResolvedDataJson { get; set; }

    public ImportValidationStatus ValidationStatus { get; set; } = ImportValidationStatus.Valid;

    [StringLength(2000)]
    public string? ValidationMessagesJson { get; set; }

    public long? MainRecordId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? FinalizedAt { get; set; }
}
