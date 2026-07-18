using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("File", Schema = "bas")]
public partial class File
{
    [Key]
    public long Id { get; set; }

    public string? Extension { get; set; }

    public Guid? UniqueId { get; set; }

    public long Size { get; set; }

    public byte[]? Content { get; set; }

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

    [InverseProperty("File")]
    public virtual ICollection<UserFileUpload> UserFileUploads { get; set; } = new List<UserFileUpload>();

    [InverseProperty("File")]
    public virtual ICollection<UserIssueReport> UserIssueReports { get; set; } = new List<UserIssueReport>();

    [InverseProperty("SignatureImage")]
    public virtual ICollection<UserSignature> UserSignatures { get; set; } = new List<UserSignature>();
}
