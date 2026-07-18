using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Job_Skill_Year_Setting", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Job_Skill_Year_Setting_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganizationJobId", Name = "IX_Organisation_Job_Skill_Year_Setting_OrganizationJobId")]
[Microsoft.EntityFrameworkCore.Index("SkillLevelId", Name = "IX_Organisation_Job_Skill_Year_Setting_SkillLevelId")]
public partial class OrganisationJobSkillYearSetting
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long OrganizationJobId { get; set; }

    public long SkillLevelId { get; set; }

    public int Year { get; set; }

    public long? Value { get; set; }

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

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationJobSkillYearSettings")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [ForeignKey("OrganizationJobId")]
    [InverseProperty("OrganisationJobSkillYearSettings")]
    public virtual OrganisationJob OrganizationJob { get; set; } = null!;

    [ForeignKey("SkillLevelId")]
    [InverseProperty("OrganisationJobSkillYearSettings")]
    public virtual SkillLevel SkillLevel { get; set; } = null!;
}
