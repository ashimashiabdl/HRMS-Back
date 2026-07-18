using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("MessageAttachment", Schema = "Identity")]
[Microsoft.EntityFrameworkCore.Index("MessageId", Name = "IX_MessageAttachment_MessageId")]
public partial class MessageAttachment
{
    [Key]
    public long Id { get; set; }

    public long MessageId { get; set; }

    [StringLength(512)]
    public string FileName { get; set; } = null!;

    [StringLength(100)]
    public string? Extension { get; set; }

    [StringLength(512)]
    public string? MimeType { get; set; }

    public long Size { get; set; }

    public byte[] Content { get; set; } = null!;

    public Guid? UniqueId { get; set; }

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

    [ForeignKey("MessageId")]
    [InverseProperty("MessageAttachments")]
    public virtual Message Message { get; set; } = null!;
}
