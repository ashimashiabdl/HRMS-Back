using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_MRT", Schema = "Setting")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_MRT_OrganisationChartId")]
public partial class OrganisationMrt
{
    [Key]
    public long Id { get; set; }

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

    [StringLength(1024)]
    public string? Description { get; set; }

    public byte[] Content { get; set; } = null!;

    [StringLength(512)]
    public string? MimeType { get; set; }

    public long Size { get; set; }

    public Guid? UniqueId { get; set; }

    [Column("title")]
    [StringLength(256)]
    public string Title { get; set; } = null!;

    [StringLength(30)]
    public string? Extension { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("OrganisationMrt")]
    public virtual ICollection<DynamicReport> DynamicReports { get; set; } = new List<DynamicReport>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationMrts")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganisationMrt")]
    public virtual ICollection<OrganisationEmployeeTypeMrt> OrganisationEmployeeTypeMrts { get; set; } = new List<OrganisationEmployeeTypeMrt>();
}
