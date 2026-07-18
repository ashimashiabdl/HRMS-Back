using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_EmployeeType_Leave", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("EmployeeTypeId", Name = "IX_Organisation_EmployeeType_Leave_EmployeeTypeId")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Organisation_EmployeeType_Leave_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_EmployeeType_Leave_OrganisationChartId")]
public partial class OrganisationEmployeeTypeLeave
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long LeaveTypeId { get; set; }

    public long EmployeeTypeId { get; set; }

    public bool IsPaid { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? DefaultAnnualQuota { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? AnnualQuotaDays { get; set; }

    [Column(TypeName = "decimal(18, 2)")]
    public decimal? CarryForwardLimit { get; set; }

    public bool Encashable { get; set; }

    public bool IsActive { get; set; }

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

    public bool IsDailyOrHourMinute { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("EmployeeTypeId")]
    [InverseProperty("OrganisationEmployeeTypeLeaves")]
    public virtual EmployeeType EmployeeType { get; set; } = null!;

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("OrganisationEmployeeTypeLeaves")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationEmployeeTypeLeaves")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
