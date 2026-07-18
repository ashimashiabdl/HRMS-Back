using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Tax_Diskette", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("BatchPayRollRequestId", Name = "IX_Tax_Diskette_BatchPayRollRequestId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Tax_Diskette_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("TaxDisketteStatusId", Name = "IX_Tax_Diskette_TaxDisketteStatusId")]
public partial class TaxDiskette
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

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

    public long PaymentPeriodId { get; set; }

    public long TaxDisketteStatusId { get; set; }

    public long? BatchPayRollRequestId { get; set; }

    /// <summary>
    /// محاسبه دیسکت برای تمامی مراکز هزینه که در دوره جاری فیش دارند انجام شود
    /// </summary>
    public bool CalculateAllFichesInCurrentPeriod { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("BatchPayRollRequestId")]
    [InverseProperty("TaxDiskettes")]
    public virtual BatchPayRollRequest? BatchPayRollRequest { get; set; }

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<BatchPayRollRequest> BatchPayRollRequests { get; set; } = new List<BatchPayRollRequest>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("TaxDiskettes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("PaymentPeriodId")]
    [InverseProperty("TaxDiskettes")]
    public virtual PaymentPeriod PaymentPeriod { get; set; } = null!;

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<TaxDisketteCostCenter> TaxDisketteCostCenters { get; set; } = new List<TaxDisketteCostCenter>();

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<TaxDisketteFile> TaxDisketteFiles { get; set; } = new List<TaxDisketteFile>();

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<TaxDisketteWh> TaxDisketteWhs { get; set; } = new List<TaxDisketteWh>();

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<TaxDisketteWk> TaxDisketteWks { get; set; } = new List<TaxDisketteWk>();

    [InverseProperty("TaxDiskette")]
    public virtual ICollection<TaxDisketteWp> TaxDisketteWps { get; set; } = new List<TaxDisketteWp>();
}
