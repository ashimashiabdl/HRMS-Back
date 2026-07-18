using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Role_Reportable_Entity", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("RoleId", Name = "IX_Role_Reportable_Entity_RoleId")]
public partial class RoleReportableEntity
{
    [Key]
    public long Id { get; set; }

    public long RoleId { get; set; }

    public long ReportableEntityId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

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

    [ForeignKey("RoleId")]
    [InverseProperty("RoleReportableEntities")]
    public virtual AspNetRole Role { get; set; } = null!;
}
