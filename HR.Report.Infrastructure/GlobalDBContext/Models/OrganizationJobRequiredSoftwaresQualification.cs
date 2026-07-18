using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Required_Softwares_Qualification", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("MasteryLevelTypeId", Name = "IX_OrganizationJob_Required_Softwares_Qualification_MasteryLevelTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Required_Softwares_Qualification_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("SoftwareId", Name = "IX_OrganizationJob_Required_Softwares_Qualification_SoftwareId")]
[Microsoft.EntityFrameworkCore.Index("SoftwareTypeId", Name = "IX_OrganizationJob_Required_Softwares_Qualification_SoftwareTypeId")]
public partial class OrganizationJobRequiredSoftwaresQualification
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long SoftwareTypeId { get; set; }

    public long MasteryLevelTypeId { get; set; }

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

    public long SoftwareId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganizationJobRequiredSoftwaresQualifications")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
