using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Deduction_Type", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Deduction_Type_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SettlementItemId", Name = "IX_Deduction_Type_SettlementItemId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Deduction_Type_WageItemId")]
public partial class DeductionType
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    [StringLength(2)]
    public string? TaxCode { get; set; }

    public bool IsActive { get; set; }

    [StringLength(50)]
    public string? EnglishName { get; set; }

    [StringLength(128)]
    public string? Code { get; set; }

    [StringLength(512)]
    public string? Comment { get; set; }

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

    public long WageItemId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    public long? SettlementItemId { get; set; }

    [InverseProperty("DeductionType")]
    public virtual ICollection<EmployeeDeduction> EmployeeDeductions { get; set; } = new List<EmployeeDeduction>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("DeductionTypes")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("SettlementItemId")]
    [InverseProperty("DeductionTypes")]
    public virtual SettlementItem? SettlementItem { get; set; }

    [InverseProperty("DeductionType")]
    public virtual ICollection<TempEmployeeDeduction> TempEmployeeDeductions { get; set; } = new List<TempEmployeeDeduction>();

    [ForeignKey("WageItemId")]
    [InverseProperty("DeductionTypes")]
    public virtual WageItem WageItem { get; set; } = null!;
}
