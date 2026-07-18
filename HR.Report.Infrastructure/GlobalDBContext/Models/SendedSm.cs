using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("SendedSMS", Schema = "Log")]
public partial class SendedSm
{
    [Key]
    public long Id { get; set; }

    public long? UserId { get; set; }

    [StringLength(11)]
    public string? MobileNumber { get; set; }

    [Column("SMSBody")]
    [StringLength(512)]
    public string? Smsbody { get; set; }

    public string? Provider1Response { get; set; }

    public string? Provider2Response { get; set; }

    [StringLength(512)]
    public string Provider1Name { get; set; } = null!;

    [StringLength(512)]
    public string Provider2Name { get; set; } = null!;

    public bool IsSuccess { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Provider1SendDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Provider1ResponseDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Provider2SendDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime Provider2ResponseDateTime { get; set; }

    [StringLength(32)]
    public string? ValidationCode { get; set; }

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
}
