using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Required_Character_Qualification", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("CharacterTypeId", Name = "IX_OrganizationJob_Required_Character_Qualification_CharacterTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Required_Character_Qualification_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("RequiredLevelId", Name = "IX_OrganizationJob_Required_Character_Qualification_RequiredLevelId")]
public partial class OrganizationJobRequiredCharacterQualification
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long CharacterTypeId { get; set; }

    public long RequiredLevelId { get; set; }

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
    [InverseProperty("OrganizationJobRequiredCharacterQualifications")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
