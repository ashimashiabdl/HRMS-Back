using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Group_Punishment_Encourage_File", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("GroupPunishmentEncourageId", Name = "IX_Group_Punishment_Encourage_File_GroupPunishmentEncourageId")]
[Microsoft.EntityFrameworkCore.Index("TempFileId", Name = "IX_Group_Punishment_Encourage_File_TempFileId")]
public partial class GroupPunishmentEncourageFile
{
    [Key]
    public long Id { get; set; }

    public long? GroupPunishmentEncourageId { get; set; }

    public string? Extension { get; set; }

    public Guid? UniqueId { get; set; }

    public long Size { get; set; }

    public byte[] Content { get; set; } = null!;

    public string? MimeType { get; set; }

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

    public long? TempFileId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("GroupPunishmentEncourageId")]
    [InverseProperty("GroupPunishmentEncourageFiles")]
    public virtual GroupPunishmentEncourage? GroupPunishmentEncourage { get; set; }

    [ForeignKey("TempFileId")]
    [InverseProperty("GroupPunishmentEncourageFiles")]
    public virtual File1? TempFile { get; set; }
}
