using HR.BaseInfo.Core.Entities;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Data;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Core.Entities;

[Table("Organisation_Job", Schema = "Org")]
public class OrganizationJob : BaseEntity, IOrganisationChartId
{
    [ForeignKey("OrganisationChart")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long OrganisationChartId { get; set; }
    public virtual OrganisationChart? OrganisationChart { get; set; }
    [ForeignKey("Job")]
    [IsEffectiveInDateOverLapChecking(IsEffective = true)]
    public long? JobId { get; set; }
    [IsEffectiveInGenericSearch(IsEffective = true)]
    public virtual Job? Job { get; set; }

    public long? StateId { get; set; }
    public virtual BaseTableValue? State { get; set; }

    public long? InsuranceDesketid { get; set; }


    [ForeignKey("StaffingRule")]
    public long? StaffingRuleId { get; set; }
    public virtual StaffingRule? StaffingRule { get; set; }
    [ForeignKey("OrganisationJobSeries")]
    public long? OrganisationJobSeriesId { get; set; }
    public virtual OrganisationJobSeries? OrganisationJobSeries { get; set; }
    [ForeignKey("OrganisationJobGroup")]
    public long? OrganisationJobGroupId { get; set; }
    public virtual OrganisationJobGroup? OrganisationJobGroup { get; set; }
    [ForeignKey("OrganisationJobCategory")]
    public long? OrganisationJobCategoryId { get; set; }
    public virtual OrganisationJobCategory? OrganisationJobCategory { get; set; }

    public long? JobNatureId { get; set; }
    public virtual BaseTableValue? JobNature { get; set; }

    public long? BasQualificationGenderId { get; set; }
    public virtual BaseTableValue? BasQualificationGender { get; set; }

    public long? CoefficientOfJobTypeId { get; set; }
    public virtual BaseTableValue? CoefficientOfJobType { get; set; }

    /// <summary>
    /// حوزه فر آیندی
    /// </summary>
    public long? ProcessAreaId { get; set; }
    public virtual BaseTableValue? ProcessArea { get; set; }


    [StringLength(255)]
    public string? Code { get; set; }
    [StringLength(50)]
    public string? SystemCode { get; set; }

    /// <summary>
    /// کد تفضیل شغل
    /// </summary>
    [StringLength(50)]
    public string? JobFinancialCode { get; set; }

    [StringLength(30)]
    public string? TotalJobCode { get; set; }
    /// <summary>
    /// ��� ���
    /// </summary>
    [StringLength(8096)]
    public string? JobDescriptions { get; set; }
    public int Capacity { get; set; }
    public int Order { get; set; }
    public int JobDegree { get; set; }
    public int FilledCapacity { get; set; }
    public bool IsDifficultJob { get; set; }

    public int MinAge { get; set; }
    public int MaxAge { get; set; }

    public int ExperienceInYears { get; set; }
    public int ExperienceInMonths { get; set; }

    [ForeignKey("TaminInsuranceJobList")]
    public long? TaminInsuranceJobListId { get; set; }
    public virtual TaminInsuranceJobList? TaminInsuranceJobList { get; set; }

    [ForeignKey("TaxOccupation")]
    public long? TaxOccupationId { get; set; }
    public virtual HR.BaseInfo.Core.Entities.TaxOccupation? TaxOccupation { get; set; }

    [ForeignKey("JobActivityType")]
    public long? JobActivityTypeId { get; set; }
    public virtual HR.BaseInfo.Core.Entities.JobActivityType? JobActivityType { get; set; }

    [ForeignKey("JobLevel")]
    public long? JobLevelId { get; set; }
    public virtual HR.BaseInfo.Core.Entities.JobLevel? JobLevel { get; set; }

    /// <summary>
    /// حداکثر مقطع تحصیلی
    /// </summary>
    [ForeignKey("MaxEducationGrade")]
    public long? MaxEducationGradeId { get; set; }
    public virtual EducationGrade? MaxEducationGrade { get; set; }


    public int JobMatchingBaseNumber { get; set; }

    [NotMapped]
    private new string title { get; set; }
}
