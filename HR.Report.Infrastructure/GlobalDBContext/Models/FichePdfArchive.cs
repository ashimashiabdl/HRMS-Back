using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Fiche_PDF_archive", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Fiche_PDF_archive_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Fiche_PDF_archive_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Fiche_PDF_archive_PaymentPeriodId")]
public partial class FichePdfArchive
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public byte[]? PdfbyteArray { get; set; }

    public long PaymentPeriodId { get; set; }

    public long EmployeeId { get; set; }

    public byte[]? FichebinaryWithEmployer { get; set; }

    public int FicheTypeId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("FichePdfArchives")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("FichePdfArchives")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;
}
