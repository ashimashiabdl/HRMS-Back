using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_WageItem", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_WageItem_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_WageItem_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "EmployeeTypeId", "IsDeleted", Name = "IX_Organisation_EmployeeType_WageItem_PayLoc_EmpType_Active")]
[Microsoft.EntityFrameworkCore.Index("WageItemId", Name = "IX_Organisation_EmployeeType_WageItem_WageItemId")]
public partial class OrganisationEmployeeTypeWageItem
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long WageItemId { get; set; }

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

    public int? Priority { get; set; }

    public bool? HideInOrder { get; set; }

    public bool IsDaily { get; set; }

    public bool IsDailyAndWage { get; set; }

    [Column("IsSanavatINC")]
    public bool IsSanavatInc { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeWageItems")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeWageItems")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("WageItemId")]
    [InverseProperty("OrganisationEmployeeTypeWageItems")]
    public virtual WageItem WageItem { get; set; } = null!;
}
