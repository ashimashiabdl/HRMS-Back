using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_OrderType_Checks", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("CheckTypeId", Name = "IX_Organisation_EmployeeType_OrderType_Checks_CheckTypeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_OrderType_Checks_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "EmployeeTypeId", "OrderTypeId", "IsDeleted", Name = "IX_Organisation_EmployeeType_OrderType_Checks_Lookup")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Organisation_EmployeeType_OrderType_Checks_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_OrderType_Checks_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationFormulaId", Name = "IX_Organisation_EmployeeType_OrderType_Checks_OrganisationFormulaId")]
public partial class OrganisationEmployeeTypeOrderTypeCheck
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long OrderTypeId { get; set; }

    public long CheckTypeId { get; set; }

    public long OrganisationFormulaId { get; set; }

    [StringLength(256)]
    public string? FailMessage { get; set; }

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

    [ForeignKey("CheckTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeChecks")]
    public virtual BaseTableValue CheckType { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeChecks")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrderTypeId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeChecks")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeChecks")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganisationFormulaId")]
    [InverseProperty("OrganisationEmployeeTypeOrderTypeChecks")]
    public virtual OrganisationFormula OrganisationFormula { get; set; } = null!;
}
