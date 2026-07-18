using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Leave", Schema = "Payroll")]
[Microsoft.EntityFrameworkCore.Index("LeaveTypeId", Name = "IX_Organisation_Leave_LeaveTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", "LeaveTypeId", "IsDeleted", Name = "IX_Organisation_Leave_OrganisationChartId_LeaveTypeId_IsDeleted", IsUnique = true)]
public partial class OrganisationLeave
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long LeaveTypeId { get; set; }

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

    [ForeignKey("LeaveTypeId")]
    [InverseProperty("OrganisationLeaves")]
    public virtual LeaveType LeaveType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationLeaves")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
