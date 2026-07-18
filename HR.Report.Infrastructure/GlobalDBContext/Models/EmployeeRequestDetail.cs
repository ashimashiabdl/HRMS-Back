using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Request_Detail", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeRequestId", Name = "IX_Employee_Request_Detail_EmployeeRequestId")]
[Microsoft.EntityFrameworkCore.Index("FileId", Name = "IX_Employee_Request_Detail_FileId")]
[Microsoft.EntityFrameworkCore.Index("RequestDocumentRequirementDetailId", Name = "IX_Employee_Request_Detail_RequestDocumentRequirementDetailId")]
public partial class EmployeeRequestDetail
{
    [Key]
    public long Id { get; set; }

    public long EmployeeRequestId { get; set; }

    public long RequestDocumentRequirementDetailId { get; set; }

    public long? FileId { get; set; }

    [StringLength(1000)]
    public string? Description { get; set; }

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
    public string? LastModifiedBy { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [ForeignKey("EmployeeRequestId")]
    [InverseProperty("EmployeeRequestDetails")]
    public virtual EmployeeRequest EmployeeRequest { get; set; } = null!;

    [ForeignKey("FileId")]
    [InverseProperty("EmployeeRequestDetails")]
    public virtual File1? File { get; set; }

    [ForeignKey("RequestDocumentRequirementDetailId")]
    [InverseProperty("EmployeeRequestDetails")]
    public virtual RequestDocumentRequirementDetail RequestDocumentRequirementDetail { get; set; } = null!;
}
