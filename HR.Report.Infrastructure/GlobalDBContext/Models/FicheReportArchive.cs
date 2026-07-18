using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Fiche_Report_Archive", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Fiche_Report_Archive_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("FicheId", Name = "IX_Fiche_Report_Archive_FicheId")]
[Microsoft.EntityFrameworkCore.Index("PaymentPeriodId", Name = "IX_Fiche_Report_Archive_PaymentPeriodId")]
public partial class FicheReportArchive
{
    [Key]
    public long Id { get; set; }

    public long FicheId { get; set; }

    public long PaymentPeriodId { get; set; }

    public long EmployeeId { get; set; }

    public string? FicheImageBase64 { get; set; }

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
    [InverseProperty("FicheReportArchives")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("FicheReportArchives")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;
}
