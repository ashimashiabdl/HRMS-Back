using Dapper;
using Hr.Employee.infrastructure.Data;
using HR.Employee.Core.DTOs;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace Hr.Employee.infrastructure.Services;

public class EmployeeSearchService(
    EmployeeContext context,
    IDapper dapper) : IScopedServices
{
    private const int LazySearchMinFilterLength = 4;
    private const int LazySearchMaxResults = 20;

    private readonly EmployeeContext _context = context;
    private readonly IDapper _dapper = dapper;

    public long CurrentUserDefaultOrganId { get; set; }

    public OperationResult GetAsKeyValuePairLazy(string filter)
    {
        if (string.IsNullOrWhiteSpace(filter) || filter.Trim().Length < LazySearchMinFilterLength)
        {
            return OperationResult.Succeeded(payload: new List<HR.SharedKernel.Data.KeyValuePair>());
        }

        var term = filter.Trim();
        var results = _context.Employees
            .Where(DateValidityExtension<HR.Employee.Core.Entities.Employee>.GetDateValidationPredicate())
            .AsNoTracking()
            .Where(i =>
                (i.FirstName != null && i.FirstName.Contains(term)) ||
                (i.LastName != null && i.LastName.Contains(term)) ||
                (i.EnglishFirstName != null && i.EnglishFirstName.Contains(term)) ||
                (i.EnglishLastName != null && i.EnglishLastName.Contains(term)) ||
                (i.NationalNo != null && i.NationalNo.Contains(term)) ||
                (i.PersonelCode != null && i.PersonelCode.Contains(term)))
            .OrderByDescending(i => i.Id)
            .Take(LazySearchMaxResults)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.FirstName + " " + i.LastName + " ( " + i.NationalNo + " ) ",
            })
            .ToList();

        return OperationResult.Succeeded(payload: results);
    }

    public OperationResult AdvanceSearch(AdvanceSearchDTO dto, long currentUserId)
    {
        if (dto.IsQuickSearch == true)
        {
            dto.PayLocationId = CurrentUserDefaultOrganId;
        }

        var parameters = new DynamicParameters();
        parameters.Add("@CostCenterId", dto.CostCenterId);
        parameters.Add("@EducationGradeId", dto.EducationGradeId);
        parameters.Add("@JobNatureId", dto.JobNatureId);
        parameters.Add("@OrganizationJobId", dto.OrganizationJobId);
        parameters.Add("@GenderId", dto.GenderId);
        parameters.Add("@MaritalStatusId", dto.MaritalStatusId);
        parameters.Add("@OrganizationUnitId", dto.OrganizationUnitId);
        parameters.Add("@WorkPlaceId", dto.WorkPlaceId);
        parameters.Add("@PayLocationId", dto.PayLocationId);
        parameters.Add("@EmployeeStatusId", dto.EmployeeStatusId);
        parameters.Add("@EmployeeTypeId", dto.EmployeeTypeId);
        parameters.Add("@FirstName", dto.FirstName);
        parameters.Add("@LastName", dto.LastName);
        parameters.Add("@PersonelCode", dto.PersonelCode);
        parameters.Add("@IdentityNo", dto.IdentityNo);
        parameters.Add("@NationalNo", dto.NationalNo);
        parameters.Add("@limit", dto.limit);
        parameters.Add("@CurrentUserId", currentUserId);
        parameters.Add("@ActiveName", null);
        parameters.Add("@IsRecruitment", dto.IsRecruitment);
        parameters.Add("@IsQuickSearch", dto.IsQuickSearch);
        parameters.Add("@SortBy", dto.SortBy ?? "FirstName");
        parameters.Add("@SortDirection", dto.SortDirection ?? "ASC");

        var results = _dapper.GetAll<Search_Result>("[emp].[Search]", parameters);
        return OperationResult.Succeeded(payload: results);
    }
}
