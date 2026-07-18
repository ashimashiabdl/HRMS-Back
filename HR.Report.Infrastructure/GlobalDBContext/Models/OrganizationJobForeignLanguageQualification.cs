using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("OrganizationJob_Foreign_Language_Qualification", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("LanguageLevelTypeId", Name = "IX_OrganizationJob_Foreign_Language_Qualification_LanguageLevelTypeId")]
[Microsoft.EntityFrameworkCore.Index("LanguageSkillTypeId", Name = "IX_OrganizationJob_Foreign_Language_Qualification_LanguageSkillTypeId")]
[Microsoft.EntityFrameworkCore.Index("LanguageTypeId", Name = "IX_OrganizationJob_Foreign_Language_Qualification_LanguageTypeId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_OrganizationJob_Foreign_Language_Qualification_OrganizationJobId")]
public partial class OrganizationJobForeignLanguageQualification
{
    [Key]
    public long Id { get; set; }

    public long OrganizationJobId { get; set; }

    public long LanguageTypeId { get; set; }

    public long LanguageLevelTypeId { get; set; }

    public long LanguageSkillTypeId { get; set; }

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
    [InverseProperty("OrganizationJobForeignLanguageQualifications")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;
}
