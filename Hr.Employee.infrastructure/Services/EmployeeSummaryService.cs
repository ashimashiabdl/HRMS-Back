using Dapper;
using Hr.Employee.infrastructure.Data;
using HR.Employee.Core.DTOs;
using HR.Employee.Core.DTOs.Summary;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeSummaryService(
    IUnitOfWork<EmployeeContext> unitOfWork,
    IDapper dapper,
    IServiceScopeFactory serviceScopeFactory) : IScopedServices
{
    private readonly IUnitOfWork<EmployeeContext> _unitOfWork = unitOfWork;
    private readonly IDapper _dapper = dapper;
    private readonly IServiceScopeFactory _serviceScopeFactory = serviceScopeFactory;

    private async Task<T> WithEmployeeContextAsync<T>(Func<EmployeeContext, Task<T>> action)
    {
        using var scope = _serviceScopeFactory.CreateScope();
        var ctx = scope.ServiceProvider.GetRequiredService<EmployeeContext>();
        return await action(ctx);
    }

    public OperationResult GetEmployeeEntityCounts(long employeeId)
    {
        var parms = new DynamicParameters();
        parms.Add("@EmployeeId", employeeId);
        var counts = _dapper.GetAll<EmployeeEntityCountDTO>("[emp].[GetEmployeeEntityCounts]", parms);
        return OperationResult.Succeeded(payload: counts);
    }

    public async Task<OperationResult> GetEmployeeSummaryAsync(long employeeId)
    {
        var dto = new EmployeeSummaryDto();
        var ctx = _unitOfWork.Context;

        var employee = await ctx.Employees
            .Include(e => e.Gender)
            .Include(e => e.BirthPlace)
            .Include(e => e.Religeon)
            .Include(e => e.Mazhab)
            .Include(e => e.Nationality)
            .Include(e => e.Citizenship)
            .Include(e => e.IssuePlace)
            .Include(e => e.ServicePlace)
            .Include(e => e.BloodGroup)
            .Include(e => e.BaseOrganisation)
            .Include(e => e.TaxExemptionType)
            .Include(e => e.TaminInsuranceJobList)
            .Include(e => e.SkillLevel)
            .FirstOrDefaultAsync(x => x.Id == employeeId);

        if (employee == null)
        {
            return OperationResult.NotFound();
        }

        dto.Employee = new EmployeeDetailsSummaryDto
        {
            FirstName = employee.FirstName,
            LastName = employee.LastName,
            EnglishFirstName = employee.EnglishFirstName,
            EnglishLastName = employee.EnglishLastName,
            FatherName = employee.FatherName,
            PersonelCode = employee.PersonelCode,
            AccountingSystemEmployeeId = employee.AccountingSystemEmployeeId,
            Gender = employee.Gender?.title,
            IdentityNo = employee.IdentityNo,
            BirthPlace = employee.BirthPlace?.title,
            Religeon = employee.Religeon?.title,
            Mazhab = employee.Mazhab?.title,
            NationalNo = employee.NationalNo,
            Nationality = employee.Nationality?.title,
            Citizenship = employee.Citizenship?.title,
            IssuePlace = employee.IssuePlace?.title,
            ServicePlace = employee.ServicePlace?.title,
            BirthDate = employee.BirthDate,
            BloodGroup = employee.BloodGroup?.title,
            BaseOrganisation = employee.BaseOrganisation?.title,
            TaxExemptionType = employee.TaxExemptionType?.title,
            TaminInsuranceJobList = employee.TaminInsuranceJobList?.title,
            SkillLevel = employee.SkillLevel?.title
        };

        var abilitiesTask = WithEmployeeContextAsync(c => c.Abilities
            .Where(x => x.EmployeeId == employeeId && x.IsDeleted != true)
            .Include(x => x.AbilityType)
            .Include(x => x.LevelType)
            .Select(x => new AbilitySummaryDto
            {
                AbilityType = x.AbilityType.title,
                LevelType = x.LevelType.title,
                Title = x.title
            }).ToListAsync());
        var appearancesTask = WithEmployeeContextAsync(c => c.Appearances.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var attendancesTask = WithEmployeeContextAsync(c => c.Attendances.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var bankAccountsTask = WithEmployeeContextAsync(c => c.BankAccounts
            .Where(x => x.EmployeeId == employeeId && x.IsDeleted != true)
            .Include(x => x.AccountType)
            .Select(x => new BankAccountSummaryDto
            {
                AccountNumber = x.AccountNumber,
                AccountType = x.AccountType.title,
                CardNumber = x.CardNumber,
                ShabaNumber = x.ShabaNumber,
                Status = x.Status,
                Description = x.Description,
                Title = x.title
            }).ToListAsync());
        var basijsTask = WithEmployeeContextAsync(c => c.Basijs.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var basijGradesTask = WithEmployeeContextAsync(c => c.BasijGrades.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var captivitiesTask = WithEmployeeContextAsync(c => c.Captivities.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var charactersTask = WithEmployeeContextAsync(c => c.Characters.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var coefficientsTask = WithEmployeeContextAsync(c => c.Coefficients.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var competenciesTask = WithEmployeeContextAsync(c => c.Competencies.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var contactInfosTask = WithEmployeeContextAsync(c => c.ContactInfos.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var coursesTask = WithEmployeeContextAsync(c => c.Courses.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var disabilitiesTask = WithEmployeeContextAsync(c => c.Disabilities.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var drivingLicensesTask = WithEmployeeContextAsync(c => c.DrivingLicenses.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var educationsTask = WithEmployeeContextAsync(c => c.Educations
            .Where(x => x.EmployeeId == employeeId && x.IsDeleted != true)
            .Include(x => x.EducationGrade)
            .Include(x => x.EffectiveEducationGrade)
            .Include(x => x.EducationField)
            .Include(x => x.EducationOrientation)
            .Include(x => x.EducationState)
            .Include(x => x.UniversityType)
            .Include(x => x.University)
            .Select(x => new EducationSummaryDto
            {
                EducationGrade = x.EducationGrade.title,
                EffectiveEducationGrade = x.EffectiveEducationGrade.title,
                EducationField = x.EducationField.title,
                EducationOrientation = x.EducationOrientation.title,
                EducationState = x.EducationState.title,
                UniversityType = x.UniversityType.title,
                University = x.University.title,
                EducationAverage = x.EducationAverage,
                GraduationDate = x.EducationToDate,
                ThesisTitle = x.ThesisTitle,
                Descriptions = x.Descriptions
            }).ToListAsync());
        var employeeFilesTask = WithEmployeeContextAsync(c => c.EmployeeFiles.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var employeeLoginHistoriesTask = WithEmployeeContextAsync(c => c.EmployeeLoginHistories.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var employeeSoftwaresTask = WithEmployeeContextAsync(c => c.EmployeeSoftwares.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var evaluationResultsTask = WithEmployeeContextAsync(c => c.EvaluationResults.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var experiencesTask = WithEmployeeContextAsync(c => c.Experiences.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var familiesTask = WithEmployeeContextAsync(c => c.Families.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var filesTask = WithEmployeeContextAsync(c => c.Files.Where(x => x.IsDeleted != true && c.EmployeeFiles.Any(ef => ef.FileId == x.Id && ef.EmployeeId == employeeId && ef.IsDeleted != true)).ToListAsync());
        var foreignTravelsTask = WithEmployeeContextAsync(c => c.ForeignTravels.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var foreignLanguagesTask = WithEmployeeContextAsync(c => c.ForeignLanguages.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var historyStopsTask = WithEmployeeContextAsync(c => c.HistoryStops.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var imagesTask = WithEmployeeContextAsync(c => c.Images.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var insurancesTask = WithEmployeeContextAsync(c => c.Insurances.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var insuranceDetailsTask = WithEmployeeContextAsync(c => c.InsuranceDetails.Where(x => x.Insurance.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var isarsTask = WithEmployeeContextAsync(c => c.Isars.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var militaryServicesTask = WithEmployeeContextAsync(c => c.MilitaryServices.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var otherVeteransTask = WithEmployeeContextAsync(c => c.OtherVeterans.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var punishmentEncouragesTask = WithEmployeeContextAsync(c => c.PunishmentEncourages.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var warsTask = WithEmployeeContextAsync(c => c.Wars.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());
        var worksTask = WithEmployeeContextAsync(c => c.Works.Where(x => x.EmployeeId == employeeId && x.IsDeleted != true).ToListAsync());

        await Task.WhenAll(
            abilitiesTask, appearancesTask, attendancesTask, bankAccountsTask, basijsTask, basijGradesTask,
            captivitiesTask, charactersTask, coefficientsTask, competenciesTask, contactInfosTask, coursesTask,
            disabilitiesTask, drivingLicensesTask, educationsTask, employeeFilesTask, employeeLoginHistoriesTask,
            employeeSoftwaresTask, evaluationResultsTask, experiencesTask, familiesTask, filesTask,
            foreignTravelsTask, foreignLanguagesTask, historyStopsTask, imagesTask, insurancesTask,
            insuranceDetailsTask, isarsTask, militaryServicesTask, otherVeteransTask, punishmentEncouragesTask,
            warsTask, worksTask);

        dto.Abilities = await abilitiesTask;
        dto.Appearances = await appearancesTask;
        dto.Attendances = await attendancesTask;
        dto.BankAccounts = await bankAccountsTask;
        dto.Basijs = await basijsTask;
        dto.BasijGrades = await basijGradesTask;
        dto.Captivities = await captivitiesTask;
        dto.Characters = await charactersTask;
        dto.Coefficients = await coefficientsTask;
        dto.Competencies = await competenciesTask;
        dto.ContactInfos = await contactInfosTask;
        dto.Courses = await coursesTask;
        dto.Disabilities = await disabilitiesTask;
        dto.DrivingLicenses = await drivingLicensesTask;
        dto.Educations = await educationsTask;
        dto.EmployeeFiles = await employeeFilesTask;
        dto.EmployeeLoginHistories = await employeeLoginHistoriesTask;
        dto.EmployeeSoftwares = await employeeSoftwaresTask;
        dto.EvaluationResults = await evaluationResultsTask;
        dto.Experiences = await experiencesTask;
        dto.Families = await familiesTask;
        dto.Files = await filesTask;
        dto.ForeignTravels = await foreignTravelsTask;
        dto.ForeignLanguages = await foreignLanguagesTask;
        dto.HistoryStops = await historyStopsTask;
        dto.Images = await imagesTask;
        dto.Insurances = await insurancesTask;
        dto.InsuranceDetails = await insuranceDetailsTask;
        dto.Isars = await isarsTask;
        dto.MilitaryServices = await militaryServicesTask;
        dto.OtherVeterans = await otherVeteransTask;
        dto.PunishmentEncourages = await punishmentEncouragesTask;
        dto.Wars = await warsTask;
        dto.Works = await worksTask;

        return OperationResult.Succeeded(payload: dto);
    }
}
