using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_OrderType_WageItem", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "EmployeeTypeId", "OrderTypeId", "WageItemId", "IsDeleted", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_Lookup")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationCheckFormulaId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_OrganisationCheckFormulaId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_OrganisationFormulaId")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Organisation_EmployeeType_OrderType_WageItem_WageItemId")]
public partial class OrganisationEmployeeTypeOrderTypeWageItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrderTypeId { get; set; }

    public long WageItemId { get; set; }

    public long EnterTypeId { get; set; }

    public long? CheckingTimeId { get; set; }

    public long? OrganisationFormulaId { get; set; }

    public long? OrganisationCheckFormulaId { get; set; }

    public int? FixValue { get; set; }

    public int? Min { get; set; }

    public int? Max { get; set; }

    public bool IsEditable { get; set; }

    public bool HideInOrderPrint { get; set; }

    public bool IsDefault { get; set; }

    public bool IsBatch { get; set; }

    [StringLength(256)]
    public string? Description { get; set; }

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

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItems")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItems")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationCheckFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItemOrganisationCheckFormulas")]
    public virtual OrganisationFormula? OrganisationCheckFormula { get; set; }

    [ForeignKey("OrganisationFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItemOrganisationFormulas")]
    public virtual OrganisationFormula? OrganisationFormula { get; set; }

    [ForeignKey("WageItemId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeWageItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
