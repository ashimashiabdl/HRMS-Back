using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities;
/// <summary>
/// جهت استفاده در فرمول ها
/// </summary>
[Table("Organisation_Job_Skill_Year_Setting", Schema = "Org")]
public class OrganisationJobSkillYearSetting : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }


    [ForeignKey("OrganizationJob")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganizationJobId { get; set; }
    public virtual OrganizationJob? OrganizationJob { get; set; }


    [ForeignKey("SkillLevel")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long SkillLevelId { get; set; }
    public virtual SkillLevel? SkillLevel { get; set; }

    public int Year { get; set; }

    public long? Value { get; set; }

}
