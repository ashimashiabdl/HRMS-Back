using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Leave_Entitlement", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Leave_Entitlement_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Employee_Leave_Entitlement_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Employee_Leave_Entitlement_OrganisationChartId")]
public partial class EmployeeLeaveEntitlement
{
    [Key]
    public long Id { get; set; }

    public long EmployeeId { get; set; }

    public long OrganisationChartId { get; set; }

    public long LeaveTypeId { get; set; }

    public int Year { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal LeaveAmount { get; set; }

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
    [InverseProperty("EmployeeLeaveEntitlements")]
    public virtual Employee Employee { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("EmployeeLeaveEntitlements")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("EmployeeLeaveEntitlements")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
