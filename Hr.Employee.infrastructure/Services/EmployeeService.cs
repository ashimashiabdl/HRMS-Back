using AutoMapper;
using HR.Employee.Core.Constants;
using HR.Employee.Core.DTOs;
using Hr.Employee.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

/// <summary>
/// سرویس اصلی CRUD کارمند — متدهای تخصصی به سرویس‌های جداگانه واگذار شده‌اند.
/// </summary>
public class EmployeeService(
    IMapper mapper,
    IUnitOfWork<EmployeeContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService,
    EmployeeAccessService accessService,
    EmployeeSearchService searchService,
    EmployeeCredentialService credentialService,
    EmployeeSummaryService summaryService)
    : BaseService<HR.Employee.Core.Entities.Employee, EmployeeContext, EmployeeDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    private readonly EmployeeAccessService _accessService = accessService;
    private readonly EmployeeSearchService _searchService = searchService;
    private readonly EmployeeCredentialService _credentialService = credentialService;
    private readonly EmployeeSummaryService _summaryService = summaryService;

    public OperationResult GetAsKeyValuePairLazy(string filter) =>
        _searchService.GetAsKeyValuePairLazy(filter);

    public bool CheckAccess(long currentUserId, long employeeId) =>
        _accessService.CheckAccess(currentUserId, employeeId);

    public IEnumerable<long> GetAccessibleEmployeeIds(long currentUserId) =>
        _accessService.GetAccessibleEmployeeIds(currentUserId);

    public IQueryable<HR.Employee.Core.Entities.Employee> GetAccessibleEmployeesQueryable(long currentUserId, long? baseOrganisationId = null) =>
        _accessService.GetAccessibleEmployeesQueryable(currentUserId, baseOrganisationId);

    public OperationResult GetAccessibleEmployeesWithoutFinalOrderCount(long currentUserId) =>
        _accessService.GetAccessibleEmployeesWithoutFinalOrderCount(currentUserId);

    public bool VerifyHashedPassword(string password, string hash, byte[] salt) =>
        _credentialService.VerifyHashedPassword(password, hash, salt);

    public string HashPassword(string password, out byte[] salt) =>
        _credentialService.HashPassword(password, out salt);

    public OperationResult GetEmployeeEntityCounts(long employeeId) =>
        _summaryService.GetEmployeeEntityCounts(employeeId);

    public async Task<OperationResult> UpdateCurrentUserPassword(UpdatePassCurrentEmployeeDTO entityToUpdate) =>
        await _credentialService.UpdateCurrentUserPassword(entityToUpdate);

    public OperationResult AdvanceSearch(AdvanceSearchDTO dto, long currentUserId)
    {
        _searchService.CurrentUserDefaultOrganId = _currentUserDefaultOrganId;
        return _searchService.AdvanceSearch(dto, currentUserId);
    }

    public async Task<OperationResult> GetEmployeeSummaryAsync(long employeeId) =>
        await _summaryService.GetEmployeeSummaryAsync(employeeId);

    public async Task<OperationResult> UpdateForAsync(EmployeeDTO entityToUpdate)
    {
        if (!entityToUpdate.Id.HasValue || entityToUpdate.Id.Value <= 0)
        {
            return OperationResult.NotFound();
        }

        var existing = await GetIdAsync(entityToUpdate.Id.Value);
        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        var mappedTodo = _mapper.Map<HR.Employee.Core.Entities.Employee>(entityToUpdate);
        mappedTodo.salt = existing.salt;
        mappedTodo.LastLoginDate = existing.LastLoginDate;
        mappedTodo.LastWrongAttemptDatetime = existing.LastWrongAttemptDatetime;
        mappedTodo.PasswordHash = existing.PasswordHash;
        mappedTodo.SecurityStamp = existing.SecurityStamp;
        mappedTodo.BaseOrganisationId = existing.BaseOrganisationId;

        EmployeeMartyrRelationRules.ApplyMartyrChildTrackingCodeRule(mappedTodo);
        EmployeeAuthorizedForeignerRules.ApplyAuthorizedForeignerRule(mappedTodo);

        var deathValidation = EmployeeDeathRules.Validate(mappedTodo);
        if (deathValidation != null)
        {
            return deathValidation;
        }

        Update(mappedTodo);
        if (await _unitOfWork.Save() > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }

    public new async Task<OperationResult> CreateForAsync(EmployeeDTO entityToCreate)
    {
        var mappedTodo = _mapper.Map<HR.Employee.Core.Entities.Employee>(entityToCreate);

        EmployeeMartyrRelationRules.ApplyMartyrChildTrackingCodeRule(mappedTodo);
        EmployeeAuthorizedForeignerRules.ApplyAuthorizedForeignerRule(mappedTodo);

        var deathValidation = EmployeeDeathRules.Validate(mappedTodo);
        if (deathValidation != null)
        {
            return deathValidation;
        }

        if (string.IsNullOrEmpty(mappedTodo.title))
        {
            mappedTodo.title = "";
        }

        Add(mappedTodo);
        if (await _unitOfWork.Save() > 0)
        {
            return OperationResult.Succeeded(payload: mappedTodo.Id);
        }

        return OperationResult.Failed();
    }

    public OperationResult GetIncluded(long id)
    {
        var row = All(false)
            .Where(e => e.Id == id)
            .AsNoTracking()
            .Include(e => e.BaseOrganisation)
            .Include(e => e.Religeon)
            .Include(e => e.Mazhab)
            .Include(e => e.BirthPlace)
            .Include(e => e.IssuePlace)
            .Include(e => e.ServicePlace)
            .Include(e => e.Nationality)
            .Include(e => e.Citizenship)
            .Include(e => e.TaxExemptionType)
            .Include(e => e.MartyrRelation)
            .Include(e => e.TaminInsuranceJobList)
            .Include(e => e.SkillLevel)
            .SingleOrDefault();

        if (row == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: _mapper.Map<EmployeeDTO>(row));
    }

    public bool Validate(HR.Employee.Core.Entities.Employee entity, object? etc = null) =>
        !All().Any(i => i.NationalNo == entity.NationalNo && i.Id != entity.Id);
}
