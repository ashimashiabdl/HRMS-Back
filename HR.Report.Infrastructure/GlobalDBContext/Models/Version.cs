using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Version", Schema = "bas")]
public partial class Version
{
    [Key]
    public long Id { get; set; }

    [StringLength(50)]
    public string VersionNumber { get; set; } = null!;

    [StringLength(256)]
    public string? VersionName { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime ReleaseDate { get; set; }

    [StringLength(20)]
    public string ReleaseType { get; set; } = null!;

    [StringLength(20)]
    public string Status { get; set; } = null!;

    [StringLength(20)]
    public string Environment { get; set; } = null!;

    public bool IsBreakingChange { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime CreatedAt { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [InverseProperty("Version")]
    public virtual ICollection<VersionChangeLog> VersionChangeLogs { get; set; } = new List<VersionChangeLog>();
}
