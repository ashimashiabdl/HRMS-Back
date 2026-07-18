using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_Coefficient", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("CoefficientId", Name = "IX_Organisation_EmployeeType_Coefficient_CoefficientId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_Coefficient_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_Coefficient_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "EmployeeTypeId", "IsDeleted", Name = "IX_Organisation_EmployeeType_Coefficient_PayLoc_EmpType_Active")]
public partial class OrganisationEmployeeTypeCoefficient
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long EmployeeTypeId { get; set; }

    public long CoefficientId { get; set; }

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

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("CoefficientId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficients")]
    public virtual Coefficient Coefficient { get; set; } = null!;

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficients")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeCoefficients")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
