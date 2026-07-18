using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Software", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Software_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("MasteryLevelTypeId", Name = "IX_Employee_Software_MasteryLevelTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Software_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("SoftwareId", Name = "IX_Employee_Software_SoftwareId")]
[Microsoft.EntityFrameworkCore.Index("SoftwareTypeId", Name = "IX_Employee_Software_SoftwareTypeId")]
public partial class EmployeeSoftware
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long SoftwareId { get; set; }

    public long SoftwareTypeId { get; set; }

    public long MasteryLevelTypeId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? CreateDate { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastModifiedDate { get; set; }

    [Column("IPAddress")]
    [StringLength(128)]
    public string Ipaddress { get; set; } = null!;

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
    [InverseProperty("EmployeeSoftwares")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeSoftwares")]
    public virtual OrganisationChart? OrganisationChart { get; set; }
}
