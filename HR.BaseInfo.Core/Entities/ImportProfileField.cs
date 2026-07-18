using HR.SharedKernel.Import;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Import_Profile_Field", Schema = "bas")]
public class ImportProfileField : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey(nameof(ImportProfile))]
    public long ImportProfileId { get; set; }
    public virtual ImportProfile? ImportProfile { get; set; }

    public int ExcelColumnOrder { get; set; }

    [StringLength(8)]
    public string? ExcelColumnLetter { get; set; }

    [StringLength(256)]
    public string? ExcelColumnHeader { get; set; }

    [StringLength(128)]
    public string TargetPropertyName { get; set; } = "";

    [StringLength(64)]
    public string? DataType { get; set; }

    public bool IsRequired { get; set; }

    public bool IsUniqueKey { get; set; }

    public FkLookupType FkLookupType { get; set; } = FkLookupType.None;

    [StringLength(256)]
    public string? FkReferenceEntity { get; set; }

    [StringLength(128)]
    public string? FkReferenceField { get; set; }

    public int DisplayOrder { get; set; }
}
