using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_File_Upload", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("FileId", Name = "IX_User_File_Upload_FileId")]
public partial class UserFileUpload
{
    [Key]
    public long Id { get; set; }

    public long FileId { get; set; }

    [StringLength(2000)]
    public string Description { get; set; } = null!;

    [StringLength(500)]
    public string? OrganizationName { get; set; }

    public long? UploadedByUserId { get; set; }

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

    [ForeignKey("FileId")]
    [InverseProperty("UserFileUploads")]
    public virtual File File { get; set; } = null!;
}
