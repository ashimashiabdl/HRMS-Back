using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_Request_File", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("BatchRequestId", Name = "IX_Batch_Request_File_BatchRequestId")]
[Microsoft.EntityFrameworkCore.Index("FileTypeId", Name = "IX_Batch_Request_File_FileTypeId")]
public partial class BatchRequestFile
{
    [Key]
    public long Id { get; set; }

    public long BatchRequestId { get; set; }

    public byte[] Content { get; set; } = null!;

    [StringLength(2048)]
    public string? Description { get; set; }

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

    public long FileTypeId { get; set; }

    [StringLength(2048)]
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

    [ForeignKey("BatchRequestId")]
    [InverseProperty("BatchRequestFiles")]
    public virtual BatchRequest BatchRequest { get; set; } = null!;
}
