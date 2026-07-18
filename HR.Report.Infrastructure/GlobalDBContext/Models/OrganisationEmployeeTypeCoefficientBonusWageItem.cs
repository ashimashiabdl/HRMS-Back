using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_Coefficient_Bonus_WageItem", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("CoefficientId", Name = "IX_Organisation_EmployeeType_Coefficient_Bonus_WageItem_CoefficientId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_Coefficient_Bonus_WageItem_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_Coefficient_Bonus_WageItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Organisation_EmployeeType_Coefficient_Bonus_WageItem_WageItemId")]
public partial class OrganisationEmployeeTypeCoefficientBonusWageItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long WageItemId { get; set; }

    public long CoefficientId { get; set; }

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

    [ForeignKey("CoefficientId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficientBonusWageItems")]
    public virtual Coefficient Coefficient { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficientBonusWageItems")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficientBonusWageItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficientBonusWageItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
