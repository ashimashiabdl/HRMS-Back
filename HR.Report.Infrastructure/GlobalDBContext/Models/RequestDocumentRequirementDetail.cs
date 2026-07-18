using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("RequestDocumentRequirementDetail", Schema = "bas")]
[Microsoft.EntityFrameworkCore.Index("RequestDocumentRequirementId", Name = "IX_RequestDocumentRequirementDetail_RequestDocumentRequirementId")]
[Microsoft.EntityFrameworkCore.Index("RequestDocumentRequirementId", "Title", Name = "IX_RequestDocumentRequirementDetail_RequestDocumentRequirementId_title", IsUnique = true)]
public partial class RequestDocumentRequirementDetail
{
    [Key]
    public long Id { get; set; }

    public long RequestDocumentRequirementId { get; set; }

    public bool IsRequired { get; set; }

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

    [InverseProperty("RequestDocumentRequirementDetail")]
    public virtual ICollection<EmployeeRequestDetail> EmployeeRequestDetails { get; set; } = new List<EmployeeRequestDetail>();

    [ForeignKey("RequestDocumentRequirementId")]
    [InverseProperty("RequestDocumentRequirementDetails")]
    public virtual RequestDocumentRequirement RequestDocumentRequirement { get; set; } = null!;
}
