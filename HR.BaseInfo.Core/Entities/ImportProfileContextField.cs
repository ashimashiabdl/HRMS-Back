using HR.SharedKernel.Import;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace HR.BaseInfo.Core.Entities;

[Table("Import_Profile_Context_Field", Schema = "bas")]
public class ImportProfileContextField : HR.SharedKernel.Data.BaseEntity
{
    [ForeignKey(nameof(ImportProfile))]
    public long ImportProfileId { get; set; }
    public virtual ImportProfile? ImportProfile { get; set; }

    [StringLength(128)]
    public string TargetPropertyName { get; set; } = "";

    [StringLength(64)]
    public string? DataType { get; set; }

    public ImportContextControlType ControlType { get; set; } = ImportContextControlType.Text;

    public bool IsRequired { get; set; }

    public FkLookupType FkLookupType { get; set; } = FkLookupType.None;

    [StringLength(256)]
    public string? FkReferenceEntity { get; set; }

    [StringLength(64)]
    public string? FkReferenceSchema { get; set; }

    public int DisplayOrder { get; set; }
}
