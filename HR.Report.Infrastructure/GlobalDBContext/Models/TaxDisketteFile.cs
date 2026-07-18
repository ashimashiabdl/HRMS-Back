using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_Diskette_File", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("FileTypeId", Name = "IX_Tax_Diskette_File_FileTypeId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteId", Name = "IX_Tax_Diskette_File_TaxDisketteId")]
public partial class TaxDisketteFile
{
    [Key]
    public long Id { get; set; }

    public long TaxDisketteId { get; set; }

    public long FileTypeId { get; set; }

    [StringLength(64)]
    public string? Extension { get; set; }

    [StringLength(64)]
    public string? FileName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string? Ipaddress { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public string? Content { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("TaxDisketteId")]
    [InverseProperty("TaxDisketteFiles")]
    public virtual TaxDiskette TaxDiskette { get; set; } = null!;
}
