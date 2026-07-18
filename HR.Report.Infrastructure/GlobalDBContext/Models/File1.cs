using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("File", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_File_OrganisationChartId")]
public partial class File1
{
    [Key]
    public long Id { get; set; }

    public byte[] Content { get; set; } = null!;

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

    [StringLength(512)]
    public string? MimeType { get; set; }

    public long OrganisationChartId { get; set; }

    public long Size { get; set; }

    public Guid? UniqueId { get; set; }

    [StringLength(30)]
    public string? Extension { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [InverseProperty("File")]
    public virtual ICollection<EmployeeFile> EmployeeFiles { get; set; } = new List<EmployeeFile>();

    [InverseProperty("File")]
    public virtual ICollection<EmployeeRequestDetail> EmployeeRequestDetails { get; set; } = new List<EmployeeRequestDetail>();

    [InverseProperty("TempFile")]
    public virtual ICollection<GroupPunishmentEncourageFile> GroupPunishmentEncourageFiles { get; set; } = new List<GroupPunishmentEncourageFile>();

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("File1s")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("TempFile")]
    public virtual ICollection<TempPunishmentEncourage> TempPunishmentEncourages { get; set; } = new List<TempPunishmentEncourage>();
}
