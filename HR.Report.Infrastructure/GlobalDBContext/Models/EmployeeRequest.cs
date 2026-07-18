using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Employee_Request", Schema = "emp")]
[Microsoft.EntityFrameworkCore.Index("EmployeeId", Name = "IX_Employee_Request_EmployeeId")]
[Microsoft.EntityFrameworkCore.Index("EmployeeRequestStatusId", Name = "IX_Employee_Request_EmployeeRequestStatusId")]
[Microsoft.EntityFrameworkCore.Index("RequestDocumentRequirementId", Name = "IX_Employee_Request_RequestDocumentRequirementId")]
public partial class EmployeeRequest
{
    [Key]
    public long Id { get; set; }

    public long? OrganisationChartId { get; set; }

    public long EmployeeId { get; set; }

    public long RequestDocumentRequirementId { get; set; }

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

    public long EmployeeRequestStatusId { get; set; }

    [ForeignKey("EmployeeId")]
    [InverseProperty("EmployeeRequests")]
    public virtual Employee Employee { get; set; } = null!;

    [InverseProperty("EmployeeRequest")]
    public virtual ICollection<EmployeeRequestDetail> EmployeeRequestDetails { get; set; } = new List<EmployeeRequestDetail>();

    [ForeignKey("EmployeeRequestStatusId")]
    [InverseProperty("EmployeeRequests")]
    public virtual EmployeeRequestStatus EmployeeRequestStatus { get; set; } = null!;

    [ForeignKey("RequestDocumentRequirementId")]
    [InverseProperty("EmployeeRequests")]
    public virtual RequestDocumentRequirement RequestDocumentRequirement { get; set; } = null!;
}
