using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Batch_Request", Schema = "Order")]
[Microsoft.EntityFrameworkCore.Index("ArchiveStateId", Name = "IX_Batch_Request_ArchiveStateId")]
[Microsoft.EntityFrameworkCore.Index("AspNetUsersId", Name = "IX_Batch_Request_AspNetUsersId")]
[Microsoft.EntityFrameworkCore.Index("OrderTypeId", Name = "IX_Batch_Request_OrderTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Batch_Request_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("RequestStateId", Name = "IX_Batch_Request_RequestStateId")]
[Microsoft.EntityFrameworkCore.Index("RequestTypeId", Name = "IX_Batch_Request_RequestTypeId")]
public partial class BatchRequest
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long RequestStateId { get; set; }

    public long RequestTypeId { get; set; }

    [StringLength(128)]
    public string? IssuerUser { get; set; }

    public string? RequsetDescription { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? LastPoolingTime { get; set; }

    public bool IsDone { get; set; }

    public int EmployeeCount { get; set; }

    public int PoolingEmployeeId { get; set; }

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

    [Column(TypeName = "datetime")]
    public DateTime? FinishDateTime { get; set; }

    public bool IncludeManager { get; set; }

    public bool NeedBatchPrint { get; set; }

    public long OrderTypeId { get; set; }

    public bool SendToCartable { get; set; }

    public bool IgnoreEqualToInputes { get; set; }

    public bool KeepOrderCopies { get; set; }

    public bool KeepPromissories { get; set; }

    public int SuccessCount { get; set; }

    public bool ForceRecruitIssue { get; set; }

    public long? ArchiveStateId { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ArchiveFinishDateTime { get; set; }

    [Column(TypeName = "datetime")]
    public DateTime? ArchiveLastPoolingTime { get; set; }

    public int ArchivePoolingEmployeeId { get; set; }

    public string? OverriddenOrderDescription { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    public long AspNetUsersId { get; set; }

    [ForeignKey("AspNetUsersId")]
    [InverseProperty("BatchRequests")]
    public virtual AspNetUser AspNetUsers { get; set; } = null!;

    [InverseProperty("BatchRequest")]
    public virtual ICollection<BatchRequestDetail> BatchRequestDetails { get; set; } = new List<BatchRequestDetail>();

    [InverseProperty("BatchRequest")]
    public virtual ICollection<BatchRequestFile> BatchRequestFiles { get; set; } = new List<BatchRequestFile>();

    [ForeignKey("OrderTypeId")]
    [InverseProperty("BatchRequests")]
    public virtual OrderType OrderType { get; set; } = null!;

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("BatchRequests")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;
}
