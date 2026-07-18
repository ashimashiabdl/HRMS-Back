using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Periodic_Task_Description", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Periodic_Task_Description_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("TaskPeriodId", Name = "IX_OrganizationJob_Periodic_Task_Description_TaskPeriodId")]
public partial class OrganizationJobPeriodicTaskDescription
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long? TaskPeriodId { get; set; }

    public string? TaskDescription { get; set; }

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

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganizationJobPeriodicTaskDescriptions")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
