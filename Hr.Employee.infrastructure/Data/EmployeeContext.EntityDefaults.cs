using HR.Employee.Core.Entities;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Metadata.Builders;

namespace Hr.Employee.infrastructure.Data;

public partial class EmployeeContext
{
    private const string BitFalse = "(CONVERT([bit],(0)))";
    private const string IntZero = "(0)";
    private const string EmptyString = "(N'')";
    private const string DecimalZero = "((0))";

    private static void ConfigureEmployeeEntityDefaults(ModelBuilder modelBuilder)
    {
        ConfigureAbilityDefaults(modelBuilder);
        ConfigureAbsenceRecordDefaults(modelBuilder);
        ConfigureAppearanceDefaults(modelBuilder);
        ConfigureAttendanceDefaults(modelBuilder);
        ConfigureBankAccountDefaults(modelBuilder);
        ConfigureBasijDefaults(modelBuilder);
        ConfigureBasijGradeDefaults(modelBuilder);
        ConfigureCaptivityDefaults(modelBuilder);
        ConfigureCharacterDefaults(modelBuilder);
        ConfigureCoefficientDefaults(modelBuilder);
        ConfigureCompetencyDefaults(modelBuilder);
        ConfigureContactInfoDefaults(modelBuilder);
        ConfigureCourseDefaults(modelBuilder);
        ConfigureDisabilityDefaults(modelBuilder);
        ConfigureDrivingLicenseDefaults(modelBuilder);
        ConfigureEducationDefaults(modelBuilder);
        ConfigureEmployeeFileDefaults(modelBuilder);
        ConfigureEmployeeLoginHistoryDefaults(modelBuilder);
        ConfigureEmployeeOtpDefaults(modelBuilder);
        ConfigureEmployeeRefreshTokenDefaults(modelBuilder);
        ConfigureEmployeeRequestDefaults(modelBuilder);
        ConfigureEmployeeRequestDetailDefaults(modelBuilder);
        ConfigureEmployeeSoftwareDefaults(modelBuilder);
        ConfigureEvaluationResultDefaults(modelBuilder);
        ConfigureExperienceDefaults(modelBuilder);
        ConfigureFamilyDefaults(modelBuilder);
        ConfigureFileDefaults(modelBuilder);
        ConfigureForeignLanguageDefaults(modelBuilder);
        ConfigureForeignTravelDefaults(modelBuilder);
        ConfigureGroupPunishmentEncourageDefaults(modelBuilder);
        ConfigureGroupPunishmentEncourageFileDefaults(modelBuilder);
        ConfigureHistoryStopDefaults(modelBuilder);
        ConfigureImageDefaults(modelBuilder);
        ConfigureInsuranceDefaults(modelBuilder);
        ConfigureInsuranceDetailDefaults(modelBuilder);
        ConfigureIsarDefaults(modelBuilder);
        ConfigureMilitaryServiceDefaults(modelBuilder);
        ConfigureOtherVeteranDefaults(modelBuilder);
        ConfigurePunishmentEncourageDefaults(modelBuilder);
        ConfigureTempPunishmentEncourageDefaults(modelBuilder);
        ConfigureWarDefaults(modelBuilder);
        ConfigureWorkDefaults(modelBuilder);
    }

    private static void ApplyBaseEntityDefaults<T>(EntityTypeBuilder<T> entity, bool mapTitle = false) where T : BaseEntity
    {
        entity.Property(e => e.IPAddress).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CreatedBy).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LastModifiedBy).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsDeleted).HasDefaultValueSql(BitFalse);

        if (mapTitle)
            entity.Property(e => e.title).HasDefaultValueSql(EmptyString);
        else
            entity.Ignore(e => e.title);
    }

    private static void ConfigureAbilityDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Ability>();
        ApplyBaseEntityDefaults(entity);
    }

    private static void ConfigureAbsenceRecordDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<AbsenceRecord>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.FirstApprove).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.SecondApprove).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureAppearanceDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Appearance>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.SpecificSymptoms).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Weight).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Height).HasDefaultValueSql(IntZero);
        entity.Property(e => e.FootSize).HasDefaultValueSql(IntZero);
    }

    private static void ConfigureAttendanceDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Attendance>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.InOutType).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DeviceName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.InOutCard).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureBankAccountDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BankAccount>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.AccountNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Priority).HasDefaultValueSql(IntZero);
        entity.Property(e => e.FromPrice).HasDefaultValueSql(IntZero);
        entity.Property(e => e.ToPrice).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Status).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.BankBranchId).HasDefaultValueSql(IntZero);
        entity.Property(e => e.OldId).HasDefaultValueSql(IntZero);
        entity.Property(e => e.BonCardNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CardNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.ShabaNumber).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureBasijDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Basij>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.DurationYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsContinues).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsActive).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsComputeableInHistory).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.YearCoefficient).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Year).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsPercent).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.TrackingCode).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureBasijGradeDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<BasijGrade>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Grade).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Year).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureCaptivityDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Captivity>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Year).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Month).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Day).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsContinues).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.SacrificePercent).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.IsActive).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.TrackingCode).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureCharacterDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Character>();
        ApplyBaseEntityDefaults(entity);
    }

    private static void ConfigureCoefficientDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Coefficient>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Value).HasDefaultValueSql(DecimalZero);
    }

    private static void ConfigureCompetencyDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Competency>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Acceptable).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureContactInfoDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ContactInfo>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Address).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Zipcode).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Phone).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.EmergencyPhone).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Fax).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Mail).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MobileNo).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsVerify).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureCourseDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Course>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.CourseMark).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CourseTime).HasDefaultValueSql(IntZero);
        entity.Property(e => e.CoursepPlace).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CourseSession).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CourseSerial).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureDisabilityDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Disability>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.DisabilityPercent).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.HasCertification).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureDrivingLicenseDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<DrivingLicense>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.LicenseSerialNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.PreviousDerivingNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Licencedescription).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEducationDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Education>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.EducationAverage).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsInDutyTime).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsBoursie).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.ThesisTitle).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsDefaultEducation).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsUsedInOrder).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.SetByEmployee).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.LicenceNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.OtherUniversityName).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEmployeeFileDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeFile>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.OtherFileGroupName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Name).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsImage).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureEmployeeLoginHistoryDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeLoginHistory>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.IsSuccess).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureEmployeeOtpDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeOtp>();
        entity.Property(e => e.CodeHash).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsUsed).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.CreatedByIp).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Purpose).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEmployeeRefreshTokenDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeRefreshToken>();
        entity.Property(e => e.Token).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CreatedByIp).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.RevokedByIp).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.ReplacedByToken).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.RevocationReason).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEmployeeRequestDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeRequest>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEmployeeRequestDetailDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeRequestDetail>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureEmployeeSoftwareDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EmployeeSoftware>();
        ApplyBaseEntityDefaults(entity);
    }

    private static void ConfigureEvaluationResultDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<EvaluationResult>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Average).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.Year).HasDefaultValueSql(IntZero);
        entity.Property(e => e.EvaluationCoefficent).HasDefaultValueSql(IntZero);
        entity.Property(e => e.YearCoefficent).HasDefaultValueSql(IntZero);
    }

    private static void ConfigureExperienceDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Experience>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Duration).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsInternal).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsAcceptable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.AcceptablePercent).HasDefaultValueSql(IntZero);
        entity.Property(e => e.CompanyTitle).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureFamilyDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Family>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.FirstName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LastName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.FatherName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.NationalNo).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.EffectivePercent).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.MaintenanceCost).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.IdentityNo).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.AccountNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsImperfective).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsVerify).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.UsedinOrder).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsPremierStudent).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsDependent).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsCoveredInsurance).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsHekmat).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsCashBenefits).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsWelfareServices).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.DisabilityPercent).HasDefaultValueSql(IntZero);
        entity.Property(e => e.HasCertification).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.InsuranceNumber).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureFileDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<HR.Employee.Core.Entities.File>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.MimeType).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureForeignLanguageDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ForeignLanguage>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Languagescore).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.OtherLanguageName).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Acceptable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureForeignTravelDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<ForeignTravel>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.CountryNames).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MissionSubject).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MissionCost).HasDefaultValueSql(IntZero);
        entity.Property(e => e.CountryCount).HasDefaultValueSql(IntZero);
        entity.Property(e => e.ArchiveId).HasDefaultValueSql(IntZero);
        entity.Property(e => e.CountryList).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureGroupPunishmentEncourageDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<GroupPunishmentEncourage>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.LastModifiedUser).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.EmPloyeeCount).HasDefaultValueSql(IntZero);
    }

    private static void ConfigureGroupPunishmentEncourageFileDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<GroupPunishmentEncourageFile>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.Extension).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MimeType).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureHistoryStopDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<HistoryStop>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.IsComputable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.HistoryStopDays).HasDefaultValueSql(IntZero);
    }

    private static void ConfigureImageDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Image>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.IsDefault).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureInsuranceDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Insurance>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.AccDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsComputable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.InsuranceNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.HasSupplementaryInsurance).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureInsuranceDetailDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<InsuranceDetail>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Year).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Month).HasDefaultValueSql(IntZero);
        entity.Property(e => e.AccDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsFullInsurnce).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsComputable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsOptionalInsurnce).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Desc).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureIsarDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Isar>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.Isarpercent).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.IsarInability).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsarInjuerdOrgan).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsarDurationYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsarDurationMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsarDurationDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsContinues).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsActive).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.TrackingCode).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureMilitaryServiceDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<MilitaryService>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.NameOfPeriod).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MilitaryDuration).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MilitaryFullDuration).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MilitaryMinDuration).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.ConfirmedLetterNo).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.MilitariSerialNo).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsContinue).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsComputable).HasDefaultValueSql(BitFalse);
    }

    private static void ConfigureOtherVeteranDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<OtherVeteran>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.IsLast).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.DurationYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsActive).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.IsComputeable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.SacrificePercent).HasDefaultValueSql(IntZero);
        entity.Property(e => e.TrackingCode).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigurePunishmentEncourageDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<PunishmentEncourage>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.UnitValue).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsGroup).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Value).HasDefaultValueSql(IntZero);
    }

    private static void ConfigureTempPunishmentEncourageDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<TempPunishmentEncourage>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.UnitValue).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Value).HasDefaultValueSql(IntZero);
        entity.Property(e => e.NationalNo).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureWarDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<War>();
        ApplyBaseEntityDefaults(entity);
        entity.Property(e => e.JebheOperations).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.PercentAnnualIncrease).HasDefaultValueSql(DecimalZero);
        entity.Property(e => e.DurationYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.DurationDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.IsContinues).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.Descriptions).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsActive).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.TrackingCode).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.AcceptableDurationForTaxExemption).HasDefaultValueSql(EmptyString);
    }

    private static void ConfigureWorkDefaults(ModelBuilder modelBuilder)
    {
        var entity = modelBuilder.Entity<Work>();
        ApplyBaseEntityDefaults(entity, mapTitle: true);
        entity.Property(e => e.WorkPlaceDesc).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LetterNumber).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.LastTitle).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.InsHsyYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.InsHsyMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.InsHsyDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.AcptInsHsyYear).HasDefaultValueSql(IntZero);
        entity.Property(e => e.AcptInsHsyMonth).HasDefaultValueSql(IntZero);
        entity.Property(e => e.AcptInsHsyDay).HasDefaultValueSql(IntZero);
        entity.Property(e => e.Description).HasDefaultValueSql(EmptyString);
        entity.Property(e => e.IsComputeable).HasDefaultValueSql(BitFalse);
        entity.Property(e => e.ExperienceMult).HasDefaultValueSql(IntZero);
        entity.Property(e => e.RetiredMult).HasDefaultValueSql(IntZero);
        entity.Property(e => e.YearMult).HasDefaultValueSql(IntZero);
    }
}
