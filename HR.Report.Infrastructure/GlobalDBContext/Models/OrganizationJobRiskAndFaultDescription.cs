using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Risk_And_Fault_Description", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Risk_And_Fault_Description_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("RiskOrFaultTypeId", Name = "IX_OrganizationJob_Risk_And_Fault_Description_RiskOrFaultTypeId")]
public partial class OrganizationJobRiskAndFaultDescription
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public bool Has { get; set; }

    public long? RiskOrFaultTypeId { get; set; }

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

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganizationJobRiskAndFaultDescriptions")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
