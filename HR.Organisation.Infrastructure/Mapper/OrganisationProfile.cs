using AutoMapper;
using HR.Organisation.Core.DTOs;
using HR.Organisation.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Infrastructure.Mapper
{
    public class OrganisationProfile : Profile
    {
        public OrganisationProfile()
        {
            CreateMap<OrganisationChart, OrganisationChartDTO>()
                 .ForMember(dest => dest.OrganizationType, opt => opt.MapFrom(src => src.OrganizationType == null ? "" : src.OrganizationType.title))
                 .ForMember(dest => dest.ParentOrganisationChart, opt => opt.MapFrom(src => src.ParentOrganisationChart == null ? "" : src.ParentOrganisationChart.title))
                 .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place == null ? "" : src.Place.title))
            ;
            CreateMap<OrganisationChartDTO, OrganisationChart>()
                 .ForMember(dest => dest.Place, opt => opt.Ignore())
                 .ForMember(dest => dest.OrganizationType, opt => opt.Ignore())
                 .ForMember(dest => dest.ParentOrganisationChart, opt => opt.Ignore())
            ;



            CreateMap<JobComplexityJobScoringFactorQuestion, JobComplexityJobScoringFactorQuestionDTO>()
           .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
           .ForMember(dest => dest.JobScoringFactor, opt => opt.MapFrom(src => src.JobScoringFactor == null ? "" : src.JobScoringFactor.title))
           .ForMember(dest => dest.JobScoringFactorGroup, opt => opt.MapFrom(src => src.JobScoringFactor == null ? "" : (src.JobScoringFactor.Group == null ? "" : src.JobScoringFactor.Group.title)))
           .ForMember(dest => dest.Complexity, opt => opt.MapFrom(src => src.Complexity == null ? "" : src.Complexity.title))
            ;
            CreateMap<JobComplexityJobScoringFactorQuestionDTO, JobComplexityJobScoringFactorQuestion>();



            CreateMap<JobAbundanceJobScoringFactorQuestion, JobAbundanceJobScoringFactorQuestionDTO>()
           .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
           .ForMember(dest => dest.JobScoringFactor, opt => opt.MapFrom(src => src.JobScoringFactor == null ? "" : src.JobScoringFactor.title))
           .ForMember(dest => dest.Abundance, opt => opt.MapFrom(src => src.Abundance == null ? "" : src.Abundance.title))
            ;
            CreateMap<JobAbundanceJobScoringFactorQuestionDTO, JobAbundanceJobScoringFactorQuestion>();


            CreateMap<OrganisationChartImageDTO, OrganisationChartImage>().ReverseMap();


            CreateMap<OrganisationJobCategory, OrganisationJobCategoryDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.JobCategory, opt => opt.MapFrom(src => src.JobCategory == null ? "" : src.JobCategory.title));
            CreateMap<OrganisationJobCategoryDTO, OrganisationJobCategory>();

            CreateMap<OrganisationPositionJob, OrganisationPositionJobDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
             .ForMember(dest => dest.OrganisationPosition, opt => opt.MapFrom(src => src.OrganisationPosition == null ? "" : (src.OrganisationPosition.Position == null ? "" : src.OrganisationPosition.Position.title)));
            CreateMap<OrganisationPositionJobDTO, OrganisationPositionJob>();

            CreateMap<OrganisationPositionSuggested, OrganisationPositionSuggestedDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.OrganisationPosition, opt => opt.MapFrom(src => src.OrganisationPosition == null ? "" : (src.OrganisationPosition.Position == null ? "" : src.OrganisationPosition.Position.title + " ( " + (src.OrganisationPosition.PositionCode ?? "") + " ) ")));
            CreateMap<OrganisationPositionSuggestedDTO, OrganisationPositionSuggested>();

            CreateMap<OrganisationPositionSupervisor, OrganisationPositionSupervisorDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.OrganisationPosition, opt => opt.MapFrom(src => src.OrganisationPosition == null ? "" : (src.OrganisationPosition.Position == null ? "" : src.OrganisationPosition.Position.title + " ( " + (src.OrganisationPosition.PositionCode ?? "") + " ) ")));
            CreateMap<OrganisationPositionSupervisorDTO, OrganisationPositionSupervisor>();

            CreateMap<OrganisationProject, OrganisationProjectDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.Project, opt => opt.MapFrom(src => src.Project == null ? "" : src.Project.title));
            CreateMap<OrganisationProjectDTO, OrganisationProject>();


            CreateMap<OrganisationPositionOccuptionMoreThanOneCach, OrganisationPositionOccuptionMoreThanOneCachDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title)
             );
            CreateMap<OrganisationPositionOccuptionMoreThanOneCachDTO, OrganisationPositionOccuptionMoreThanOneCach>();

            CreateMap<OrganisationPosition, OrganisationPositionDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.PositionType, opt => opt.MapFrom(src => src.PositionType == null ? "" : src.PositionType.title))
             .ForMember(dest => dest.Position, opt => opt.MapFrom(src => src.Position == null ? "" : src.Position.title))
             .ForMember(dest => dest.InsurancePosition, opt => opt.MapFrom(src => src.InsurancePosition == null ? "" : src.InsurancePosition.title))
             .ForMember(dest => dest.Rank, opt => opt.MapFrom(src => src.Rank == null ? "" : src.Rank.title))
             .ForMember(dest => dest.PositionManagementLevel, opt => opt.MapFrom(src => src.PositionManagementLevel == null ? "" : src.PositionManagementLevel.title))
             .ForMember(dest => dest.PositionState, opt => opt.MapFrom(src => src.PositionState == null ? "" : src.PositionState.title))
             .ForMember(dest => dest.RelatedNode, opt => opt.MapFrom(src => src.RelatedNode == null ? "" : src.RelatedNode.title));
            CreateMap<OrganisationPositionDTO, OrganisationPosition>();


            CreateMap<OrganisationJobGroup, OrganisationJobGroupDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State == null ? "" : src.State.title))
             .ForMember(dest => dest.OrganisationJobCategory, opt => opt.MapFrom(src => src.OrganisationJobCategory == null ? "" : (src.OrganisationJobCategory.JobCategory == null ? "" : src.OrganisationJobCategory.JobCategory.title)))
             .ForMember(dest => dest.JobGroup, opt => opt.MapFrom(src => src.JobGroup == null ? "" : src.JobGroup.title));
            CreateMap<OrganisationJobGroupDTO, OrganisationJobGroup>();




            CreateMap<OrganisationJobSeries, OrganisationJobSeriesDTO>()
             .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             .ForMember(dest => dest.JobSeries, opt => opt.MapFrom(src => src.JobSeries == null ? "" : src.JobSeries.title))
             .ForMember(dest => dest.OrganisationJobCategory, opt => opt.MapFrom(src => src.OrganisationJobCategory == null ? "" : (src.OrganisationJobCategory.JobCategory == null ? "" : src.OrganisationJobCategory.JobCategory.title)))
             .ForMember(dest => dest.OrganisationJobGroup, opt => opt.MapFrom(src => src.OrganisationJobGroup == null ? "" : (src.OrganisationJobGroup.JobGroup == null ? "" : src.OrganisationJobGroup.JobGroup.title)));
            CreateMap<OrganisationJobSeriesDTO, OrganisationJobSeries>();



            CreateMap<OrganizationJob, OrganizationJobDTO>()
           .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
           .ForMember(dest => dest.JobNature, opt => opt.MapFrom(src => src.JobNature == null ? "" : src.JobNature.title))
           .ForMember(dest => dest.Job, opt => opt.MapFrom(src => src.Job == null ? "" : src.Job.title + " ( " + (string.IsNullOrEmpty(src.Code) ? "" : src.Code) + " ) "))
           .ForMember(dest => dest.StaffingRule, opt => opt.MapFrom(src => src.StaffingRule == null ? "" : src.StaffingRule.title))
           .ForMember(dest => dest.State, opt => opt.MapFrom(src => src.State == null ? "" : src.State.title))
           .ForMember(dest => dest.TaminInsuranceJobList, opt => opt.MapFrom(src => src.TaminInsuranceJobList == null ? "" : src.TaminInsuranceJobList.title))
           .ForMember(dest => dest.CoefficientOfJobType, opt => opt.MapFrom(src => src.CoefficientOfJobType == null ? "" : src.CoefficientOfJobType.title))
           .ForMember(dest => dest.OrganisationJobCategory, opt => opt.MapFrom(src => src.OrganisationJobCategory == null ? "" : (src.OrganisationJobCategory.JobCategory == null ? "" : src.OrganisationJobCategory.JobCategory.title)))
           .ForMember(dest => dest.OrganisationJobGroup, opt => opt.MapFrom(src => src.OrganisationJobGroup == null ? "" : (src.OrganisationJobGroup.JobGroup == null ? "" : src.OrganisationJobGroup.JobGroup.title)))
           .ForMember(dest => dest.OrganisationJobSeries, opt => opt.MapFrom(src => src.OrganisationJobSeries == null ? "" : (src.OrganisationJobSeries.JobSeries == null ? "" : src.OrganisationJobSeries.JobSeries.title)))
           .ForMember(dest => dest.TaxOccupation, opt => opt.MapFrom(src => src.TaxOccupation == null ? "" : src.TaxOccupation.title))
           .ForMember(dest => dest.JobActivityType, opt => opt.MapFrom(src => src.JobActivityType == null ? "" : src.JobActivityType.title))
           .ForMember(dest => dest.JobLevel, opt => opt.MapFrom(src => src.JobLevel == null ? "" : src.JobLevel.title))
           .ForMember(dest => dest.MaxEducationGrade, opt => opt.MapFrom(src => src.MaxEducationGrade == null ? "" : src.MaxEducationGrade.title))
           .ForMember(dest => dest.ProcessArea, opt => opt.MapFrom(src => src.ProcessArea == null ? "" : src.ProcessArea.title))

           ;
            CreateMap<OrganizationJobDTO, OrganizationJob>();

            CreateMap<JobScoreAbundanceComplexity, JobScoreAbundanceComplexityDTO>()
           .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
           .ForMember(dest => dest.JobScoringFactor, opt => opt.MapFrom(src => src.JobScoringFactor == null ? "" : src.JobScoringFactor.title))
           .ForMember(dest => dest.Abundance, opt => opt.MapFrom(src => src.Abundance == null ? "" : src.Abundance.title))
           .ForMember(dest => dest.AbundanceLevel, opt => opt.MapFrom(src => src.Abundance == null ? 0 : src.Abundance.Level))
           .ForMember(dest => dest.Complexity, opt => opt.MapFrom(src => src.Complexity == null ? "" : src.Complexity.title))
           .ForMember(dest => dest.ComplexityLevel, opt => opt.MapFrom(src => src.Complexity == null ? 0 : src.Complexity.Level))
           .ForMember(dest => dest.jobScoringFactorGroup, opt => opt.MapFrom(src => src.JobScoringFactor == null ? "" : (src.JobScoringFactor.Group == null ? "" : src.JobScoringFactor.Group.title)))
            ;
            CreateMap<JobScoreAbundanceComplexityDTO, JobScoreAbundanceComplexity>();



            CreateMap<OrganizationGoal, OrganizationGoalDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
             ;

            CreateMap<OrganizationGoalDTO, OrganizationGoal>();  
            
            
            CreateMap<OrganizationJobAbilityQualification, OrganizationJobAbilityQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.AbilityType, opt => opt.MapFrom(src => src.AbilityType == null ? "" : src.AbilityType.title))
            .ForMember(dest => dest.LevelType, opt => opt.MapFrom(src => src.LevelType == null ? "" : src.LevelType.title))
             ;

            CreateMap<OrganizationJobAbilityQualificationDTO, OrganizationJobAbilityQualification>();  
            
            
            
            CreateMap<OrganizationJobCompetencyQualification, OrganizationJobCompetencyQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.CompetencyType, opt => opt.MapFrom(src => src.CompetencyType == null ? "" : src.CompetencyType.title))
            .ForMember(dest => dest.CompetencyLevel, opt => opt.MapFrom(src => src.CompetencyLevel == null ? "" : src.CompetencyLevel.title))
             ;

            CreateMap<OrganizationJobCompetencyQualificationDTO, OrganizationJobCompetencyQualification>();    



            CreateMap<OrganizationJobEducationFieldQualification, OrganizationJobEducationFieldQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.EducationField, opt => opt.MapFrom(src => src.EducationField == null ? "" : src.EducationField.title))
             ;

            CreateMap<OrganizationJobEducationFieldQualificationDTO, OrganizationJobEducationFieldQualification>();



            CreateMap<OrganizationJobEducationGradeQualification, OrganizationJobEducationGradeQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.EducationGrade, opt => opt.MapFrom(src => src.EducationGrade == null ? "" : src.EducationGrade.title))
             ;

            CreateMap<OrganizationJobEducationGradeQualificationDTO, OrganizationJobEducationGradeQualification>();



                CreateMap<OrganizationJobForeignLanguageQualification, OrganizationJobForeignLanguageQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.LanguageType, opt => opt.MapFrom(src => src.LanguageType == null ? "" : src.LanguageType.title))
            .ForMember(dest => dest.LanguageLevelType, opt => opt.MapFrom(src => src.LanguageLevelType == null ? "" : src.LanguageLevelType.title))
            .ForMember(dest => dest.LanguageSkillType, opt => opt.MapFrom(src => src.LanguageSkillType == null ? "" : src.LanguageSkillType.title))
             ;

            CreateMap<OrganizationJobForeignLanguageQualificationDTO, OrganizationJobForeignLanguageQualification>();



            

             CreateMap<OrganizationJobInitialCourseQualification, OrganizationJobInitialCourseQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.CourseType, opt => opt.MapFrom(src => src.CourseType == null ? "" : src.CourseType.title))
            .ForMember(dest => dest.CourseLevel, opt => opt.MapFrom(src => src.CourseLevel == null ? "" : src.CourseLevel.title))
             ;

            CreateMap<OrganizationJobInitialCourseQualificationDTO, OrganizationJobInitialCourseQualification>();


            
            

             CreateMap<OrganizationJobPerformanceEvaluationCriteriaDescription, OrganizationJobPerformanceEvaluationCriteriaDescriptionDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.CriteriaType, opt => opt.MapFrom(src => src.CriteriaType == null ? "" : src.CriteriaType.title))
             ;

            CreateMap<OrganizationJobPerformanceEvaluationCriteriaDescriptionDTO, OrganizationJobPerformanceEvaluationCriteriaDescription>();


            
             CreateMap<OrganizationJobPeriodicTaskDescription, OrganizationJobPeriodicTaskDescriptionDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.TaskPeriod, opt => opt.MapFrom(src => src.TaskPeriod == null ? "" : src.TaskPeriod.title))
             ;

            CreateMap<OrganizationJobPeriodicTaskDescriptionDTO, OrganizationJobPeriodicTaskDescription>();     
            
            CreateMap<OrganizationJobRequiredCharacterQualification, OrganizationJobRequiredCharacterQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.CharacterType, opt => opt.MapFrom(src => src.CharacterType == null ? "" : src.CharacterType.title))
            .ForMember(dest => dest.RequiredLevel, opt => opt.MapFrom(src => src.RequiredLevel == null ? "" : src.RequiredLevel.title))
             ;

            CreateMap<OrganizationJobRequiredCharacterQualificationDTO, OrganizationJobRequiredCharacterQualification>();      
            
            
            
            CreateMap<OrganizationJobRequiredSoftwaresQualification, OrganizationJobRequiredSoftwaresQualificationDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.SoftwareType, opt => opt.MapFrom(src => src.SoftwareType == null ? "" : src.SoftwareType.title))
            .ForMember(dest => dest.MasteryLevelType, opt => opt.MapFrom(src => src.MasteryLevelType == null ? "" : src.MasteryLevelType.title))
            .ForMember(dest => dest.Software, opt => opt.MapFrom(src => src.Software == null ? "" : src.Software.title))
             ;

            CreateMap<OrganizationJobRequiredSoftwaresQualificationDTO, OrganizationJobRequiredSoftwaresQualification>();       
            

            
            CreateMap<OrganizationJobRiskAndFaultDescription, OrganizationJobRiskAndFaultDescriptionDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.RiskOrFaultType, opt => opt.MapFrom(src => src.RiskOrFaultType == null ? "" : src.RiskOrFaultType.title))
             ;

            CreateMap<OrganizationJobRiskAndFaultDescriptionDTO, OrganizationJobRiskAndFaultDescription>();       

            
            CreateMap<OrganizationJobTaskDescription, OrganizationJobTaskDescriptionDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.TaskType, opt => opt.MapFrom(src => src.TaskType == null ? "" : src.TaskType.title))
             ;

            CreateMap<OrganizationJobTaskDescriptionDTO, OrganizationJobTaskDescription>(); 
            
            
            
            CreateMap<RelatedOrganizationJobDescription, RelatedOrganizationJobDescriptionDTO>()
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.OrganizationRelatedJob, opt => opt.MapFrom(src => src.OrganizationRelatedJob == null ? "" : (src.OrganizationRelatedJob.Job == null ? "" : src.OrganizationRelatedJob.Job.title)))
            
             ;

            CreateMap<RelatedOrganizationJobDescriptionDTO, RelatedOrganizationJobDescription>();






            CreateMap<JobScoringFactor, JobScoringFactorDTO>()
           .ForMember(dest => dest.Group, opt => opt.MapFrom(src => src.Group == null ? "" : src.Group.title))
            ;
            CreateMap<JobScoringFactorDTO, JobScoringFactor>();


            CreateMap<AbundanceDTO, Abundance>().ReverseMap();
            CreateMap<ComplexityDTO, Complexity>().ReverseMap();
            CreateMap<G20ScoreDomainJobDegreeDTO, G20ScoreDomainJobDegree>().ReverseMap();


            CreateMap<OrganisationJobSkillYearSetting, OrganisationJobSkillYearSettingDTO>()
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
            .ForMember(dest => dest.OrganizationJob, opt => opt.MapFrom(src => src.OrganizationJob == null ? "" : (src.OrganizationJob.Job == null ? "" : src.OrganizationJob.Job.title)))
            .ForMember(dest => dest.SkillLevel, opt => opt.MapFrom(src => src.SkillLevel == null ? "" : src.SkillLevel.title))
             ;

            CreateMap<OrganisationJobSkillYearSettingDTO, OrganisationJobSkillYearSetting>();



        }
    }
}
