using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Status", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_Employee_Status_title", IsUnique = true)]
public partial class EmployeeStatus
{
    [Key]
    public long Id { get; set; }

    [StringLength(450)]
    public string? Description { get; set; }

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

    [Column(TypeName = "datetime")]
    public DateTime? EndDate { get; set; }

    public bool IsDeleted { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? StartDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("EmployeeStatus")]
    public virtual ICollection<OrganisationEmployeeStatus> OrganisationEmployeeStatuses { get; set; } = new List<OrganisationEmployeeStatus>();

    [InverseProperty("DefaultEmpStatus")]
    public virtual ICollection<OrganisationEmployeeTypeOrderTypeCanChange> OrganisationEmployeeTypeOrderTypeCanChanges { get; set; } = new List<OrganisationEmployeeTypeOrderTypeCanChange>();

    [InverseProperty("EmployeeStatus")]
    public virtual ICollection<RecruitOrder> RecruitOrders { get; set; } = new List<RecruitOrder>();

    [InverseProperty("EmployeeStatus")]
    public virtual ICollection<StatusListItem> StatusListItems { get; set; } = new List<StatusListItem>();

    [InverseProperty("EmployeeStatus")]
    public virtual ICollection<Work> Works { get; set; } = new List<Work>();
}
