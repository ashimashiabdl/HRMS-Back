using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("User_Issue_Report", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("FileId", Name = "IX_User_Issue_Report_FileId")]
public partial class UserIssueReport
{
    [Key]
    public long Id { get; set; }

    [StringLength(2000)]
    public string? Description { get; set; }

    [StringLength(20)]
    public string? RelatedPersonNationalCode { get; set; }

    public long? FileId { get; set; }

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

    public long? CreatedByUserId { get; set; }

    public bool IsSubmitted { get; set; }

    [StringLength(2000)]
    public string? Response { get; set; }

    public long? ResponseByUserId { get; set; }

    public DateTime? ResponseDate { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("FileId")]
    [InverseProperty("UserIssueReports")]
    public virtual File? File { get; set; }
}
