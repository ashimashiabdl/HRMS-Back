using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeStatus", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusGroupId", Name = "IX_Organisation_EmployeeStatus_EmployeeStatusGroupId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", Name = "IX_Organisation_EmployeeStatus_EmployeeStatusId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", "OrganisationChartId", Name = "IX_Organisation_EmployeeStatus_Lookup")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeStatus_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeStatusId", "OrganisationChartId", "IsEmployed", Name = "IX_Organisation_EmployeeStatus_Status_Organ_IsEmployed")]
public partial class OrganisationEmployeeStatus
{
    [Key]
    public long Id { get; set; }

    public long EmployeeStatusId { get; set; }

    public long EmployeeStatusGroupId { get; set; }

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

    public bool? IsEmployed { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeStatusId")]
    [InverseProperty("OrganisationEmployeeStatuses")]
    public virtual EmployeeStatus EmployeeStatus { get; set; } = null!;

    [ForeignKey("EmployeeStatusGroupId")]
    [InverseProperty("OrganisationEmployeeStatuses")]
    public virtual EmployeeStatusGroup EmployeeStatusGroup { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeStatuses")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
