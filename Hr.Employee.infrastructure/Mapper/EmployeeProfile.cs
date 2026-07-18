using AutoMapper;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Employee.infrastructure.Mapper;

public class EmployeeProfile : Profile
{
    public EmployeeProfile()
    {
        CreateMap<HR.Employee.Core.Entities.Employee, AdvanceSearchResultDTO>();
        CreateMap<HR.Employee.Core.Entities.Employee, EmployeeDTO>()
            .ForMember(dest => dest.BaseOrganisationTitle, opt => opt.MapFrom(src => src.BaseOrganisation == null ? "" : src.BaseOrganisation.title))
            .ForMember(dest => dest.ReligeonTitle, opt => opt.MapFrom(src => src.Religeon == null ? "" : src.Religeon.title))
            .ForMember(dest => dest.MazhabTitle, opt => opt.MapFrom(src => src.Mazhab == null ? "" : src.Mazhab.title))
            .ForMember(dest => dest.BirthPlaceTitle, opt => opt.MapFrom(src => src.BirthPlace == null ? "" : src.BirthPlace.title))
            .ForMember(dest => dest.IssuePlaceTitle, opt => opt.MapFrom(src => src.IssuePlace == null ? "" : src.IssuePlace.title))
            .ForMember(dest => dest.TaxExemptionTypeTitle, opt => opt.MapFrom(src => src.TaxExemptionType == null ? "" : src.TaxExemptionType.title))
            .ForMember(dest => dest.SkillLevelTitle, opt => opt.MapFrom(src => src.SkillLevel == null ? "" : src.SkillLevel.title))
            //.ForMember(dest => dest.JobTitle, opt => opt.MapFrom(src => src.Job == null ? "" : src.Job.title))
            .ForMember(dest => dest.MartyrRelationTitle, opt => opt.MapFrom(src => src.MartyrRelation == null ? "" : src.MartyrRelation.title))
            .ForMember(dest => dest.ServicePlaceTitle, opt => opt.MapFrom(src => src.ServicePlace == null ? "" : src.ServicePlace.title))
            .ForMember(dest => dest.TaminInsuranceJobListTitle, opt => opt.MapFrom(src => src.TaminInsuranceJobList == null ? "" : src.TaminInsuranceJobList.title))
            .ForMember(dest => dest.NationalityTitle, opt => opt.MapFrom(src => src.Nationality == null ? "" : src.Nationality.title))
            .ForMember(dest => dest.CitizenshipTitle, opt => opt.MapFrom(src => src.Citizenship == null ? "" : src.Citizenship.title))
            .ForMember(dest => dest.GenderTitle, opt => opt.MapFrom(src => src.Gender == null ? "" : src.Gender.title));
        CreateMap<HR.Employee.Core.Entities.EmployeeLoginHistory, EmployeeLoginHistoryDTO>()
       .ForMember(dest => dest.ActiveName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.ActiveName))

            ;
        CreateMap<EmployeeDTO, HR.Employee.Core.Entities.Employee>();

        CreateMap<AttendanceDTO, HR.Employee.Core.Entities.Attendance>().ReverseMap();

        CreateMap<HR.Employee.Core.Entities.Competency, CompetencyDTO>()
       .ForMember(dest => dest.CompetencyLevel, opt => opt.MapFrom(src => src.CompetencyLevel == null ? "" : src.CompetencyLevel.title))
       .ForMember(dest => dest.CompetencyType, opt => opt.MapFrom(src => src.CompetencyType == null ? "" : src.CompetencyType.title))
       ;
        CreateMap<CompetencyDTO, HR.Employee.Core.Entities.Competency>();


        CreateMap<HR.Employee.Core.Entities.Ability, AbilityDTO>()
       .ForMember(dest => dest.AbilityType, opt => opt.MapFrom(src => src.AbilityType == null ? "" : src.AbilityType.title))
       .ForMember(dest => dest.LevelType, opt => opt.MapFrom(src => src.LevelType == null ? "" : src.LevelType.title))
       ;
        CreateMap<AbilityDTO, HR.Employee.Core.Entities.Ability>();


        CreateMap<HR.Employee.Core.Entities.PunishmentEncourage, PunishmentEncourageDTO>()
       .ForMember(dest => dest.Employee, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
       .ForMember(dest => dest.AgentOfPunishmentEncourage, opt => opt.MapFrom(src => src.AgentOfPunishmentEncourage == null ? "" : src.AgentOfPunishmentEncourage.title + " ( " + (src.AgentOfPunishmentEncourage.IsPunishment == true ? " تنبیه " : " تشویق") + " ) "))
       ;
        CreateMap<PunishmentEncourageDTO, HR.Employee.Core.Entities.PunishmentEncourage>();



        CreateMap<HR.Employee.Core.Entities.TempPunishmentEncourage, TempPunishmentEncourageDTO>()
       .ForMember(dest => dest.FirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
       .ForMember(dest => dest.LastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
       .ForMember(dest => dest.FatherName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FatherName))
       .ForMember(dest => dest.PersonelCode, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.PersonelCode))
       .ForMember(dest => dest.NationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
       ;
        CreateMap<TempPunishmentEncourageDTO, HR.Employee.Core.Entities.TempPunishmentEncourage>();


        CreateMap<AbsenceRecordDTO, HR.Employee.Core.Entities.AbsenceRecord>().ReverseMap();




        CreateMap<HR.Employee.Core.Entities.GroupPunishmentEncourage, GroupPunishmentEncourageDTO>()
       .ForMember(dest => dest.AgentOfPunishmentEncourage, opt => opt.MapFrom(src => src.AgentOfPunishmentEncourage == null ? "" : src.AgentOfPunishmentEncourage.title))
       .ForMember(dest => dest.OrganisationAgentOfPunishmentEncourageScoreInterval, opt => opt.MapFrom(src => src.OrganisationAgentOfPunishmentEncourageScoreInterval == null ? "" : src.OrganisationAgentOfPunishmentEncourageScoreInterval.title))
       ;
        CreateMap<GroupPunishmentEncourageDTO, HR.Employee.Core.Entities.GroupPunishmentEncourage>();



        CreateMap<HR.Employee.Core.Entities.EmployeeSoftware, EmployeeSoftwareDTO>()
       .ForMember(dest => dest.Software, opt => opt.MapFrom(src => src.Software == null ? "" : src.Software.title))
       .ForMember(dest => dest.SoftwareType, opt => opt.MapFrom(src => src.SoftwareType == null ? "" : src.SoftwareType.title))
       .ForMember(dest => dest.MasteryLevelType, opt => opt.MapFrom(src => src.MasteryLevelType == null ? "" : src.MasteryLevelType.title))
       ;
        CreateMap<EmployeeSoftwareDTO, HR.Employee.Core.Entities.EmployeeSoftware>();


        CreateMap<HR.Employee.Core.Entities.Character, CharacterDTO>()
       .ForMember(dest => dest.CharacterType, opt => opt.MapFrom(src => src.CharacterType == null ? "" : src.CharacterType.title))
       .ForMember(dest => dest.RequiredLevel, opt => opt.MapFrom(src => src.RequiredLevel == null ? "" : src.RequiredLevel.title))
       ;
        CreateMap<CharacterDTO, HR.Employee.Core.Entities.Character>();


        CreateMap<HR.Employee.Core.Entities.ContactInfo, ContactInfoDTO>();
        CreateMap<ContactInfoDTO, HR.Employee.Core.Entities.ContactInfo>();

        CreateMap<HR.Employee.Core.Entities.BankAccount, BankAccountDTO>()
            //.ForMember(dest => dest.FncBank, opt => opt.MapFrom(src => src.Bank == null ? "" : src.Bank.title))
            ;
        CreateMap<BankAccountDTO, HR.Employee.Core.Entities.BankAccount>();


        CreateMap<ImageDTO, HR.Employee.Core.Entities.Image>();


        CreateMap<FamilyDTO, HR.Employee.Core.Entities.Family>();
        CreateMap<HR.Employee.Core.Entities.Family, FamilyDTO>()
            .ForMember(dest => dest.GenderType, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.BirthPlace, opt => opt.MapFrom(src => src.BirthPlace == null ? "" : src.BirthPlace.title))
            .ForMember(dest => dest.DependentType, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
            .ForMember(dest => dest.EducationField, opt => opt.MapFrom(src => src.EducationField == null ? "" : src.EducationField.title))
            .ForMember(dest => dest.EducationOrientation, opt => opt.MapFrom(src => src.EducationOrientation == null ? "" : src.EducationOrientation.title))
            .ForMember(dest => dest.EducationGrade, opt => opt.MapFrom(src => src.EducationGrade == null ? "" : src.EducationGrade.title))
            ;

        CreateMap<EducationDTO, HR.Employee.Core.Entities.Education>();
        CreateMap<HR.Employee.Core.Entities.Education, EducationDTO>()
            .ForMember(dest => dest.UniversityTypeTitle, opt => opt.MapFrom(src => src.UniversityType == null ? "" : src.UniversityType.title))
            .ForMember(dest => dest.UniversityLevelTitle, opt => opt.MapFrom(src => src.UniversityLevel == null ? "" : src.UniversityLevel.title))
            .ForMember(dest => dest.EducationGroupTitle, opt => opt.MapFrom(src => src.EducationGroup == null ? "" : src.EducationGroup.title))
            .ForMember(dest => dest.EducationGradeTitle, opt => opt.MapFrom(src => src.EducationGrade == null ? "" : src.EducationGrade.title))
            .ForMember(dest => dest.EducationFieldTitle, opt => opt.MapFrom(src => src.EducationField == null ? "" : src.EducationField.title))
            .ForMember(dest => dest.EducationOrientationTitle, opt => opt.MapFrom(src => src.EducationOrientation == null ? "" : src.EducationOrientation.title))
            .ForMember(dest => dest.EffectiveEducationGradeTitle, opt => opt.MapFrom(src => src.EffectiveEducationGrade == null ? "" : src.EffectiveEducationGrade.title));

        CreateMap<OtherVeteranDTO, HR.Employee.Core.Entities.OtherVeteran>();
        CreateMap<HR.Employee.Core.Entities.OtherVeteran, OtherVeteranDTO>()

        ;




        CreateMap<AppearanceDTO, HR.Employee.Core.Entities.Appearance>();
        CreateMap<HR.Employee.Core.Entities.Appearance, AppearanceDTO>()
       .ForMember(dest => dest.EyeColor, opt => opt.MapFrom(src => src.EyeColor == null ? "" : src.EyeColor.title))
       .ForMember(dest => dest.SkinColor, opt => opt.MapFrom(src => src.SkinColor == null ? "" : src.SkinColor.title))
       .ForMember(dest => dest.HairColor, opt => opt.MapFrom(src => src.HairColor == null ? "" : src.HairColor.title))
       ;




        CreateMap<IsarDTO, HR.Employee.Core.Entities.Isar>();
        CreateMap<HR.Employee.Core.Entities.Isar, IsarDTO>()
            .ForMember(dest => dest.IsartypeTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.ConfirmerOrganTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.IsarLocation, opt => opt.MapFrom(src => ""))

            ;




        CreateMap<WarDTO, HR.Employee.Core.Entities.War>();
        CreateMap<HR.Employee.Core.Entities.War, WarDTO>()
            .ForMember(dest => dest.WarTypeTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.WarLocation, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.ConfirmerOrganTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.EducationGradeTitle, opt => opt.MapFrom(src => src.EducationGrade == null ? "" : src.EducationGrade.title))

            ;







        CreateMap<InsuranceDTO, HR.Employee.Core.Entities.Insurance>();
        CreateMap<HR.Employee.Core.Entities.Insurance, InsuranceDTO>()
           // .ForMember(dest => dest.InsWorkShopType, opt => opt.MapFrom(src => src.InsWorkShopType == null ? "" : src.InsWorkShopType.title))
            //.ForMember(dest => dest.InsuranceType, opt => opt.MapFrom(src => src.InsuranceType == null ? "" : src.InsuranceType.title))
            .ForMember(dest => dest.SupplementaryInsuranceType, opt => opt.MapFrom(src => src.SupplementaryInsuranceType == null ? "" : src.SupplementaryInsuranceType.title))
            .ForMember(dest => dest.InsuranceSubmissionCity, opt => opt.MapFrom(src => src.InsuranceSubmissionCity == null ? "" : src.InsuranceSubmissionCity.Description))

            ;





        CreateMap<InsuranceDetailDTO, HR.Employee.Core.Entities.InsuranceDetail>();
        CreateMap<HR.Employee.Core.Entities.InsuranceDetail, InsuranceDetailDTO>()
            .ForMember(dest => dest.Insurance, opt => opt.MapFrom(src => src.Insurance == null ? "" : " شماره بیمه اصلی " + " ( " + src.Insurance.Id + " ) " + src.Insurance.InsuranceNumber))
            .ForMember(dest => dest.Status, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.InsuranceTypeRecord, opt => opt.MapFrom(src => ""))

            ;


        CreateMap<BasijGradeDTO, HR.Employee.Core.Entities.BasijGrade>();
        CreateMap<HR.Employee.Core.Entities.BasijGrade, BasijGradeDTO>();


        CreateMap<CoefficientDTO, HR.Employee.Core.Entities.Coefficient>();
        CreateMap<HR.Employee.Core.Entities.Coefficient, CoefficientDTO>();


        CreateMap<ForignLanguageDTO, HR.Employee.Core.Entities.ForeignLanguage>();
        CreateMap<HR.Employee.Core.Entities.ForeignLanguage, ForignLanguageDTO>();



        CreateMap<ForeignTravelDTO, HR.Employee.Core.Entities.ForeignTravel>();
        CreateMap<HR.Employee.Core.Entities.ForeignTravel, ForeignTravelDTO>()
            .ForMember(dest => dest.PlaceTitle, opt => opt.MapFrom(src => src.Place == null ? "" : src.Place.title))
            .ForMember(dest => dest.TravelTypeTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.MissionTypeTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.ReasonTitle, opt => opt.MapFrom(src => ""))


            ;


        CreateMap<EvaluationResultDTO, HR.Employee.Core.Entities.EvaluationResult>();
        CreateMap<HR.Employee.Core.Entities.EvaluationResult, EvaluationResultDTO>()
            .ForMember(dest => dest.EvaluationGroupType, opt => opt.MapFrom(src => ""))

            ;

        CreateMap<DrivingLicenseDTO, HR.Employee.Core.Entities.DrivingLicense>();
        CreateMap<HR.Employee.Core.Entities.DrivingLicense, DrivingLicenseDTO>()
            .ForMember(dest => dest.LicenseTypeTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.LicenseValidationPeriodTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.DrivingConstraintTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.PrimaryOrSecondaryTitle, opt => opt.MapFrom(src => ""))

            ;


        CreateMap<DisabilityDTO, HR.Employee.Core.Entities.Disability>();
        CreateMap<HR.Employee.Core.Entities.Disability, DisabilityDTO>()
            .ForMember(dest => dest.DisabilityLevelTitle, opt => opt.MapFrom(src => ""))
            .ForMember(dest => dest.DisabilityTypeTitle, opt => opt.MapFrom(src => ""))
            ;

        CreateMap<CourseDTO, HR.Employee.Core.Entities.Course>();
        CreateMap<HR.Employee.Core.Entities.Course, CourseDTO>()
            .ForMember(dest => dest.CourseStatusTitle, opt => opt.MapFrom(src => src.CourseStatus == null ? "" : src.CourseStatus.title))
            .ForMember(dest => dest.CourseLicenseTitle, opt => opt.MapFrom(src => src.CourseLicense == null ? "" : src.CourseLicense.title))
            .ForMember(dest => dest.CourseRegTypeTitle, opt => opt.MapFrom(src => src.CourseRegType == null ? "" : src.CourseRegType.title))
            .ForMember(dest => dest.CourseTitle, opt => opt.MapFrom(src => src.CourseTitle == null ? "" : src.CourseTitle.title))
            ;


        CreateMap<MilitaryServiceDTO, HR.Employee.Core.Entities.MilitaryService>();
        CreateMap<HR.Employee.Core.Entities.MilitaryService, MilitaryServiceDTO>()
           // .ForMember(dest => dest.MilitariGradeType, opt => opt.MapFrom(src => src.MilitariGradeType == null ? "" : src.MilitariGradeType.title))
            .ForMember(dest => dest.EducationGradeTitle, opt => opt.MapFrom(src => src.EducationGrade == null ? "" : src.EducationGrade.title));




        CreateMap<BasijDTO, HR.Employee.Core.Entities.Basij>();
        CreateMap<HR.Employee.Core.Entities.Basij, BasijDTO>()
            .ForMember(dest => dest.BasijTypeTitle, opt => opt.MapFrom(src => src.BasijType == null ? "" : src.BasijType.title));


        CreateMap<CaptivityDTO, HR.Employee.Core.Entities.Captivity>();
        CreateMap<HR.Employee.Core.Entities.Captivity, CaptivityDTO>()
            .ForMember(dest => dest.CaptivityLocationTitle, opt => opt.MapFrom(src => src.CaptivityLocation == null ? "" : src.CaptivityLocation.title))
            .ForMember(dest => dest.ConfirmerOrganTitle, opt => opt.MapFrom(src => src.ConfirmerOrgan == null ? "" : src.ConfirmerOrgan.title))
            ;



        CreateMap<EmployeeFileDTO, HR.Employee.Core.Entities.EmployeeFile>()
            .ForMember(dest => dest.File, opt => opt.Ignore())
            .ForMember(dest => dest.FileGroup, opt => opt.Ignore())
            .ForMember(dest => dest.OrganisationChart, opt => opt.Ignore())
            .ForMember(dest => dest.Employee, opt => opt.Ignore());
        CreateMap<HR.Employee.Core.Entities.EmployeeFile, EmployeeFileDTO>()
        .ForMember(dest => dest.File, opt => opt.MapFrom(src => src.File != null ? src.File.title : null))
        .ForMember(dest => dest.Name, opt => opt.MapFrom(src => src.File != null ? src.File.title : src.Name))
        .ForMember(dest => dest.title, opt => opt.MapFrom(src => src.File != null ? src.File.title : src.Name))
        .ForMember(dest => dest.FileGroup, opt => opt.MapFrom(src =>
            src.FileGroupId == HR.Employee.Core.Entities.EmployeeFile.OtherFileGroupId && !string.IsNullOrWhiteSpace(src.OtherFileGroupName)
                ? src.OtherFileGroupName
                : (src.FileGroup != null ? src.FileGroup.title : null)))
        .ForMember(dest => dest.OrganisationChart, opt => opt.MapFrom(src => src.OrganisationChart != null ? src.OrganisationChart.title : null))
        .ForMember(dest => dest.Extension, opt => opt.MapFrom(src => src.File != null ? src.File.Extension : null))
        .ForMember(dest => dest.MimeType, opt => opt.MapFrom(src => src.File != null ? src.File.MimeType : null))
        .ForMember(dest => dest.Size, opt => opt.MapFrom(src => src.File != null ? src.File.Size : 0))

        ;




        CreateMap<HistoryStopDTO, HR.Employee.Core.Entities.HistoryStop>();
        CreateMap<HR.Employee.Core.Entities.HistoryStop, HistoryStopDTO>()
            ;


        CreateMap<ExperienceDTO, HR.Employee.Core.Entities.Experience>();
        CreateMap<HR.Employee.Core.Entities.Experience, ExperienceDTO>()
            .ForMember(dest => dest.OrganisationChartTitle, opt => opt.MapFrom(src => src.OrganisationChart == null ? "" : src.OrganisationChart.title))
            .ForMember(dest => dest.EmployeeTitle, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " " + src.Employee.NationalNo))
            .ForMember(dest => dest.HistoryTypeTitle, opt => opt.MapFrom(src => src.HistoryType == null ? "" : src.HistoryType.title));

        CreateMap<EmployeeRequestDTO, HR.Employee.Core.Entities.EmployeeRequest>();
        CreateMap<HR.Employee.Core.Entities.EmployeeRequest, EmployeeRequestDTO>()
            .ForMember(dest => dest.RequestDocumentRequirementTitle, opt => opt.MapFrom(src => src.RequestDocumentRequirement == null ? "" : src.RequestDocumentRequirement.title))
            .ForMember(dest => dest.EmployeeRequestStatusTitle, opt => opt.MapFrom(src => src.EmployeeRequestStatus == null ? "" : src.EmployeeRequestStatus.title))
            .ForMember(dest => dest.EmployeeRequestStatusCode, opt => opt.MapFrom(src => src.EmployeeRequestStatus == null ? (int?)null : src.EmployeeRequestStatus.StatusCode))
            .ForMember(dest => dest.EmployeeFirstName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName))
            .ForMember(dest => dest.EmployeeLastName, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.LastName))
            .ForMember(dest => dest.EmployeeNationalNo, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.NationalNo))
            .ForMember(dest => dest.EmployeeTitle, opt => opt.MapFrom(src => src.Employee == null ? "" : src.Employee.FirstName + " " + src.Employee.LastName + " - " + src.Employee.NationalNo))
            .ForMember(dest => dest.DocumentCount, opt => opt.MapFrom(src => src.Details.Count(d => !d.IsDeleted)));

        CreateMap<EmployeeRequestDetailDTO, HR.Employee.Core.Entities.EmployeeRequestDetail>();
        CreateMap<HR.Employee.Core.Entities.EmployeeRequestDetail, EmployeeRequestDetailDTO>()
            .ForMember(dest => dest.RequestDocumentRequirementDetailTitle, opt => opt.MapFrom(src => src.RequestDocumentRequirementDetail == null ? "" : src.RequestDocumentRequirementDetail.title))
            .ForMember(dest => dest.FileName, opt => opt.MapFrom(src => src.File == null ? "" : src.File.title))
            .ForMember(dest => dest.IsRequired, opt => opt.MapFrom(src => src.RequestDocumentRequirementDetail != null && src.RequestDocumentRequirementDetail.IsRequired));

    }
}
