using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("VersionChangeLog", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("VersionId", Name = "IX_VersionChangeLog_VersionId")]
public partial class VersionChangeLog
{
    [Key]
    public long Id { get; set; }

    public long VersionId { get; set; }

    [StringLength(20)]
    public string ChangeType { get; set; } = null!;

    [StringLength(1000)]
    public string Description { get; set; } = null!;

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

    [ForeignKey("VersionId")]
    [InverseProperty("VersionChangeLogs")]
    public virtual Version Version { get; set; } = null!;
}
