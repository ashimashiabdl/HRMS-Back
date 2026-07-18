using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

namespace HR.Report.Infrastructure.GlobalDBContext.Models;

[Table("Organisation_Job", Schema = "Org")]
[Microsoft.EntityFrameworkCore.Index("BasQualificationGenderId", Name = "IX_Organisation_Job_BasQualificationGenderId")]
[Microsoft.EntityFrameworkCore.Index("CoefficientOfJobTypeId", Name = "IX_Organisation_Job_CoefficientOfJobTypeId")]
[Microsoft.EntityFrameworkCore.Index("JobActivityTypeId", Name = "IX_Organisation_Job_JobActivityTypeId")]
[Microsoft.EntityFrameworkCore.Index("JobId", Name = "IX_Organisation_Job_JobId")]
[Microsoft.EntityFrameworkCore.Index("JobLevelId", Name = "IX_Organisation_Job_JobLevelId")]
[Microsoft.EntityFrameworkCore.Index("JobNatureId", Name = "IX_Organisation_Job_JobNatureId")]
[Microsoft.EntityFrameworkCore.Index("MaxEducationGradeId", Name = "IX_Organisation_Job_MaxEducationGradeId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationChartId", Name = "IX_Organisation_Job_OrganisationChartId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobCategoryId", Name = "IX_Organisation_Job_OrganisationJobCategoryId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobGroupId", Name = "IX_Organisation_Job_OrganisationJobGroupId")]
[Microsoft.EntityFrameworkCore.Index("OrganisationJobSeriesId", Name = "IX_Organisation_Job_OrganisationJobSeriesId")]
[Microsoft.EntityFrameworkCore.Index("ProcessAreaId", Name = "IX_Organisation_Job_ProcessAreaId")]
[Microsoft.EntityFrameworkCore.Index("StaffingRuleId", Name = "IX_Organisation_Job_StaffingRuleId")]
[Microsoft.EntityFrameworkCore.Index("StateId", Name = "IX_Organisation_Job_StateId")]
[Microsoft.EntityFrameworkCore.Index("TaminInsuranceJobListId", Name = "IX_Organisation_Job_TaminInsuranceJobListId")]
[Microsoft.EntityFrameworkCore.Index("TaxOccupationId", Name = "IX_Organisation_Job_TaxOccupationId")]
public partial class OrganisationJob
{
    [Key]
    public long Id { get; set; }

    public long OrganisationChartId { get; set; }

    public long? StateId { get; set; }

    public long? StaffingRuleId { get; set; }

    public long? OrganisationJobSeriesId { get; set; }

    public long? OrganisationJobGroupId { get; set; }

    public long? OrganisationJobCategoryId { get; set; }

    public long? JobNatureId { get; set; }

    [StringLength(255)]
    public string? Code { get; set; }

    [StringLength(50)]
    public string? SystemCode { get; set; }

    [StringLength(50)]
    public string? JobFinancialCode { get; set; }

    public string? JobDescriptions { get; set; }

    public int Capacity { get; set; }

    public int Order { get; set; }

    public int JobDegree { get; set; }

    public int FilledCapacity { get; set; }

    public bool IsDifficultJob { get; set; }

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

    public long? JobId { get; set; }

    public long? InsuranceDesketid { get; set; }

    [StringLength(30)]
    public string? TotalJobCode { get; set; }

    public long? CoefficientOfJobTypeId { get; set; }

    public int ExperienceInMonths { get; set; }

    public int ExperienceInYears { get; set; }

    public int MaxAge { get; set; }

    public int MinAge { get; set; }

    public long? BasQualificationGenderId { get; set; }

    public long? TaminInsuranceJobListId { get; set; }

    public long? TaxOccupationId { get; set; }

    public int JobMatchingBaseNumber { get; set; }

    public long? MaxEducationGradeId { get; set; }

    public long? ProcessAreaId { get; set; }

    public long? JobActivityTypeId { get; set; }

    public long? JobLevelId { get; set; }

    [StringLength(256)]
    public string? CreatedBy { get; set; }

    [StringLength(256)]
    public string? LastModifiedBy { get; set; }

    [ForeignKey("JobId")]
    [InverseProperty("OrganisationJobs")]
    public virtual Job? Job { get; set; }

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<JobAbundanceJobScoringFactorQuestion> JobAbundanceJobScoringFactorQuestions { get; set; } = new List<JobAbundanceJobScoringFactorQuestion>();

    [ForeignKey("JobActivityTypeId")]
    [InverseProperty("OrganisationJobs")]
    public virtual JobActivityType? JobActivityType { get; set; }

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<JobComplexityJobScoringFactorQuestion> JobComplexityJobScoringFactorQuestions { get; set; } = new List<JobComplexityJobScoringFactorQuestion>();

    [ForeignKey("JobLevelId")]
    [InverseProperty("OrganisationJobs")]
    public virtual JobLevel? JobLevel { get; set; }

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<JobScoreAbundanceComplexity> JobScoreAbundanceComplexities { get; set; } = new List<JobScoreAbundanceComplexity>();

    [ForeignKey("MaxEducationGradeId")]
    [InverseProperty("OrganisationJobs")]
    public virtual EducationGrade? MaxEducationGrade { get; set; }

    [ForeignKey("OrganisationChartId")]
    [InverseProperty("OrganisationJobs")]
    public virtual OrganisationChart OrganisationChart { get; set; } = null!;

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganisationJobSkillYearSetting> OrganisationJobSkillYearSettings { get; set; } = new List<OrganisationJobSkillYearSetting>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganisationPositionJob> OrganisationPositionJobs { get; set; } = new List<OrganisationPositionJob>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobAbilityQualification> OrganizationJobAbilityQualifications { get; set; } = new List<OrganizationJobAbilityQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobCompetencyQualification> OrganizationJobCompetencyQualifications { get; set; } = new List<OrganizationJobCompetencyQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobEducationFieldQualification> OrganizationJobEducationFieldQualifications { get; set; } = new List<OrganizationJobEducationFieldQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobEducationGradeQualification> OrganizationJobEducationGradeQualifications { get; set; } = new List<OrganizationJobEducationGradeQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobForeignLanguageQualification> OrganizationJobForeignLanguageQualifications { get; set; } = new List<OrganizationJobForeignLanguageQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobInitialCourseQualification> OrganizationJobInitialCourseQualifications { get; set; } = new List<OrganizationJobInitialCourseQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobPerformanceEvaluationCriteriaDescription> OrganizationJobPerformanceEvaluationCriteriaDescriptions { get; set; } = new List<OrganizationJobPerformanceEvaluationCriteriaDescription>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobPeriodicTaskDescription> OrganizationJobPeriodicTaskDescriptions { get; set; } = new List<OrganizationJobPeriodicTaskDescription>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobRequiredCharacterQualification> OrganizationJobRequiredCharacterQualifications { get; set; } = new List<OrganizationJobRequiredCharacterQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobRequiredSoftwaresQualification> OrganizationJobRequiredSoftwaresQualifications { get; set; } = new List<OrganizationJobRequiredSoftwaresQualification>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobRiskAndFaultDescription> OrganizationJobRiskAndFaultDescriptions { get; set; } = new List<OrganizationJobRiskAndFaultDescription>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<OrganizationJobTaskDescription> OrganizationJobTaskDescriptions { get; set; } = new List<OrganizationJobTaskDescription>();

    [ForeignKey("ProcessAreaId")]
    [InverseProperty("OrganisationJobs")]
    public virtual BaseTableValue? ProcessArea { get; set; }

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<RecruitOrder> RecruitOrders { get; set; } = new List<RecruitOrder>();

    [InverseProperty("OrganizationJob")]
    public virtual ICollection<RelatedOrganizationJobDescription> RelatedOrganizationJobDescriptionOrganizationJobs { get; set; } = new List<RelatedOrganizationJobDescription>();

    [InverseProperty("OrganizationRelatedJob")]
    public virtual ICollection<RelatedOrganizationJobDescription> RelatedOrganizationJobDescriptionOrganizationRelatedJobs { get; set; } = new List<RelatedOrganizationJobDescription>();

    [ForeignKey("StaffingRuleId")]
    [InverseProperty("OrganisationJobs")]
    public virtual StaffingRule? StaffingRule { get; set; }

    [ForeignKey("TaminInsuranceJobListId")]
    [InverseProperty("OrganisationJobs")]
    public virtual TaminInsuranceJobList? TaminInsuranceJobList { get; set; }

    [ForeignKey("TaxOccupationId")]
    [InverseProperty("OrganisationJobs")]
    public virtual TaxOccupation? TaxOccupation { get; set; }
}
