using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using SystemGuide = HR.BaseInfo.Core.Entities.SystemGuide;
using SystemGuideDTO = HR.BaseInfo.Core.DTOs.SystemGuideDTO;



namespace HR.BaseInfo.infrastructure.Mapper
{
    public class BaseInfoProfile : Profile
    {
        public BaseInfoProfile()
        {
            CreateMap<HR.BaseInfo.Core.Entities.BaseTable, BaseTableDTO>().ReverseMap();
            CreateMap<ConfidentialityLevel, ConfidentialityLevelDTO>().ReverseMap();
            CreateMap<HR.BaseInfo.Core.Entities.AgentOfPunishmentEncourageGroup, AgentOfPunishmentEncourageGroupDTO>().ReverseMap();

            CreateMap<AgentOfPunishmentEncourage, AgentOfPunishmentEncourageDTO>().ReverseMap();

            CreateMap<HR.BaseInfo.Core.Entities.TaminInsuranceJobList, TaminInsuranceJobListDTO>().ReverseMap();
            CreateMap<Core.Entities.BaseTableValue, BaseTableValueDTO>()
                .ForMember(dest => dest.BaseTableTitle, opt => opt.MapFrom(src => src.BaseTable == null ? "" : src.BaseTable.title + " ( " + src.BaseTable.Id + " ) "));
            CreateMap<BaseTableValueDTO, Core.Entities.BaseTableValue>();

            CreateMap<Core.Entities.OrderType, OrderTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.University, UniversityDTO>().ReverseMap();
            CreateMap<Core.Entities.Job, JobDTO>().ReverseMap();
            CreateMap<Core.Entities.EmployeeType, EmployeeTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.EmployeeStatus, EmployeeStatusDTO>().ReverseMap();
            CreateMap<Core.Entities.HistoryType, HistoryTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.MeasurementUnit, MeasurementUnitDTO>().ReverseMap();
            CreateMap<Core.Entities.AttendanceHoliday, AttendanceHolidayDTO>()
                .ForMember(dest => dest.Place, opt => opt.MapFrom(src => src.Place == null ? "" : src.Place.title));
            CreateMap<AttendanceHolidayDTO, Core.Entities.AttendanceHoliday>();

            CreateMap<Core.Entities.LeaveType, LeaveTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.TaxOccupation, TaxOccupationDTO>().ReverseMap();
            CreateMap<Core.Entities.JobActivityType, JobActivityTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.JobLevel, JobLevelDTO>().ReverseMap();
            CreateMap<Core.Entities.SkillLevel, SkillLevelDTO>().ReverseMap();
            CreateMap<Core.Entities.TaxExemptionType, TaxExemptionTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.SettlementItem, SettlementItemDTO>().ReverseMap();
            CreateMap<Core.Entities.SettlementCause, SettlementCauseDTO>().ReverseMap();
            CreateMap<Core.Entities.Rank, RankDTO>().ReverseMap();
            CreateMap<Core.Entities.PositionManagementLevel, PositionManagementLevelDTO>().ReverseMap();
            CreateMap<Core.Entities.PositionState, PositionStateDTO>().ReverseMap();
            CreateMap<Core.Entities.SettlementStatus, SettlementStatusDTO>().ReverseMap();
            CreateMap<Core.Entities.SettlementDocumentAttachmentType, SettlementDocumentAttachmentTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.ReportMapColumn, ReportMapColumnDTO>().ReverseMap();
            CreateMap<Core.Entities.FundType, FundTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.WageItem, WageItemDTO>().ReverseMap();
            CreateMap<Core.Entities.Coefficient, CoefficientDTO>().ReverseMap();
            CreateMap<Core.Entities.ManagementAndStewardshipJob, ManagementAndStewardshipJobDTO>().ReverseMap();

            CreateMap<Core.Entities.EmployeeStatusGroup, EmployeeStatusGroupDTO>().ReverseMap();
            CreateMap<Core.Entities.EmployeeTypeGroup, EmployeeTypeGroupDTO>().ReverseMap();
            CreateMap<Core.Entities.Formula, FormulaDTO>().ReverseMap();
            CreateMap<Core.Entities.FormulaUsageLocation, FormulaUsageLocationDTO>().ReverseMap();
            CreateMap<Core.Entities.OrderTypeGroup, OrderTypeGroupDTO>().ReverseMap();
            //CreateMap<Core.Entities.Places, PlacesDTO>().ReverseMap();
            CreateMap<Core.Entities.Position, PositionDTO>().ReverseMap();
            CreateMap<Core.Entities.Setting, SettingDTO>().ReverseMap();

            CreateMap<Core.Entities.EducationField, EducationFieldDTO>().ReverseMap();

            CreateMap<Core.Entities.Places, PlacesDTO>()
    .ForMember(dest => dest.PlaceType, opt => opt.MapFrom(src => src.PlaceType == null ? "" : src.PlaceType.title))
    .ForMember(dest => dest.ParentPlace, opt => opt.MapFrom(src => src.ParentPlace == null ? "" : src.ParentPlace.title))
    ;
            CreateMap<PlacesDTO, Core.Entities.Places>();



            CreateMap<Core.Entities.EducationGrade, EducationGradeDTO>().ReverseMap();
            CreateMap<Core.Entities.EducationGroup, EducationGroupDTO>().ReverseMap();
            
            
            CreateMap<Core.Entities.OrganizationType, OrganizationTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.PositionType, PositionTypeDTO>().ReverseMap();
            CreateMap<Core.Entities.InsurancePosition, InsurancePositionDTO>().ReverseMap();
            


            CreateMap<Core.Entities.StaffingRule, StaffingRuleDTO>().ReverseMap();
            CreateMap<Core.Entities.OrderStatus, OrderStatusDTO>().ReverseMap();
            CreateMap<Core.Entities.JobSeries, JobSeriesDTO>().ReverseMap();
            CreateMap<Core.Entities.JobGroup, JobGroupDTO>().ReverseMap();
            CreateMap<Core.Entities.JobCategory, JobCategoryDTO>().ReverseMap();
            CreateMap<Core.Entities.Project, ProjectDTO>().ReverseMap();
            CreateMap<Core.Entities.ExcelDefinitionType, ExcelDefinitionTypeDTO>().ReverseMap();
            CreateMap<ImportProfile, ImportProfileCrudDTO>().ReverseMap();
            CreateMap<ImportProfileField, ImportProfileFieldCrudDTO>().ReverseMap();
            CreateMap<ImportProfileContextField, ImportProfileContextFieldCrudDTO>().ReverseMap();

            CreateMap<Core.Entities.EducationOrientation, EducationOrientationDTO>().ReverseMap();

            // Mapping for user issue reports
            CreateMap<UserIssueReport, UserIssueReportDTO>().ReverseMap();
            
            // Mapping for user file upload
            CreateMap<UserFileUpload, UserFileUploadDTO>().ReverseMap();
            
            // Mapping for feedback
            CreateMap<Feedback, FeedbackDTO>().ReverseMap();
            
            // Mapping for FAQ
            CreateMap<FAQ, FAQDTO>().ReverseMap();
            
            // Mapping for SystemGuide
            CreateMap<SystemGuide, SystemGuideDTO>()
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.title));
            CreateMap<SystemGuideDTO, SystemGuide>()
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.title));
            
            // Mapping for Version
            CreateMap<Core.Entities.Version, VersionDTO>()
                .ForMember(dest => dest.ChangeLogs, opt => opt.MapFrom(src => src.ChangeLogs))
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.VersionNumber));
            CreateMap<VersionDTO, Core.Entities.Version>()
                .ForMember(dest => dest.ChangeLogs, opt => opt.Ignore())
                .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.VersionNumber));
            
            // Mapping for VersionChangeLog
CreateMap<VersionChangeLog, VersionChangeLogDTO>().ReverseMap();

            CreateMap<Core.Entities.ImageAttachment, ImageAttachmentDTO>().ReverseMap();

            CreateMap<Carousel, CarouselDTO>().ReverseMap();
            CreateMap<RequestDocumentRequirement, RequestDocumentRequirementDTO>().ReverseMap();
            CreateMap<RequestDocumentRequirementDetail, RequestDocumentRequirementDetailDTO>().ReverseMap();
            CreateMap<Core.Entities.EmployeeRequestStatus, EmployeeRequestStatusDTO>().ReverseMap();
        }
    }
}
