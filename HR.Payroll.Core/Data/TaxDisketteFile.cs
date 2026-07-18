using HR.SharedKernel.Data;
using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;
using HR.BaseInfo.Core.Entities;

namespace HR.Payroll.Core.Data;

[Table("Tax_Diskette_File", Schema = "Payroll")]
public class TaxDisketteFile : BaseEntity
{ 
    [ForeignKey("TaxDiskette")]
    public long TaxDisketteId { get; set; }
    public virtual TaxDiskette? TaxDiskette { get; set; }
    public virtual BaseTableValue? FileType { get; set; }
    public long FileTypeId { get; set; }
    public string? Content { get; set; } = null!;
    [NotMapped]
    public string? MimeType { get; set; }
    [StringLength(64)]
    public string? Extension { get; set; }
    [StringLength(64)]
    public string? FileName { get; set; }
    [NotMapped]
    private new string title { get; set; }
}
