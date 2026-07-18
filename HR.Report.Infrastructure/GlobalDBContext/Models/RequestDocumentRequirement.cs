using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("RequestDocumentRequirement", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("Title", Name = "IX_RequestDocumentRequirement_title", IsUnique = true)]
public partial class RequestDocumentRequirement
{
    [Key]
    public long Id { get; set; }

    public bool IsActive { get; set; }

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

    [StringLength(1000)]
    public string? Description { get; set; }

    [InverseProperty("RequestDocumentRequirement")]
    public virtual ICollection<EmployeeRequest> EmployeeRequests { get; set; } = new List<EmployeeRequest>();

    [InverseProperty("RequestDocumentRequirement")]
    public virtual ICollection<RequestDocumentRequirementDetail> RequestDocumentRequirementDetails { get; set; } = new List<RequestDocumentRequirementDetail>();
}
