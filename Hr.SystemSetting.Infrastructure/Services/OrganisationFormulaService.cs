using AutoMapper;
using Dapper;
using Hr.SystemSetting.Core.DTOs;
using Hr.SystemSetting.Core.Entities;
using Hr.SystemSetting.Infrastructure.Data;
using HR.BaseInfo.Core.Enums;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.Data;


using Microsoft.Extensions.Configuration;

namespace Hr.SystemSetting.Infrastructure.Services;

public class OrganisationFormulaService(IMapper mapper, IUnitOfWork<SystemSettingContext> unitOfWork, IDapper dapper, IConfiguration configuration, UserResolverService userService) : BaseService<OrganisationFormula, SystemSettingContext, OrganisationFormulaDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: BaseQuery()
            .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId)
            .OrderByDescending(i => i.CreateDate)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.Formula.title
            }));
    }

    public OperationResult GetAsKeyValuePairByUsageLocation(FormulaUsageLocationId usageLocationId, long includeOrganisationFormulaId = 0)
    {
        return OperationResult.Succeeded(payload: BaseQuery()
            .Where(o => o.OrganisationChartId == _currentUserDefaultOrganId
                && (o.FormulaUsageLocationId == (long)usageLocationId
                    || (includeOrganisationFormulaId > 0 && o.Id == includeOrganisationFormulaId)))
            .OrderByDescending(i => i.CreateDate)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.Formula.title
            }));
    }

    public OperationResult GetFormulaUsageLocationsAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: _unitOfWork.Context.Set<HR.BaseInfo.Core.Entities.FormulaUsageLocation>()
            .AsNoTracking()
            .Where(x => x.IsDeleted != true)
            .OrderByDescending(x => x.Id)
            .Select(x => new HR.SharedKernel.Data.KeyValuePair
            {
                key = x.Id,
                value = x.title
            })
            .ToList());
    }

    public new async Task<OperationResult> CreateForAsync(OrganisationFormulaDTO entityToCreate)
    {
        if (entityToCreate.FormulaUsageLocationId <= 0)
        {
            return OperationResult.Failed("محل استفاده فرمول الزامی است");
        }

        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(OrganisationFormulaDTO entityToUpdate)
    {
        if (entityToUpdate.FormulaUsageLocationId <= 0)
        {
            return OperationResult.Failed("محل استفاده فرمول الزامی است");
        }

        return await base.UpdateForAsync(entityToUpdate);
    }

    public bool Validate(OrganisationFormula entity, object etc = null)
    {
        return entity.FormulaUsageLocationId > 0;
    }

    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<OrganisationFormula>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        var result = base.GetPagedData(
            currentPage,
            pageSize,
            filter,
            activeSortColumn,
            Sortdirection,
            IgnoreExpired,
            SelectedEmployeeTypeId,
            EmployeeId,
            CustomDataSource,
            IgnoreDefaultOrganId);

        if (!result.Success || result.Payload is not List<OrganisationFormulaDTO> list || list.Count == 0)
        {
            return result;
        }

        var organisationFormulaIds = list
            .Where(x => x.Id.HasValue)
            .Select(x => x.Id!.Value)
            .Distinct()
            .ToList();
        var formulaDefinitions = GetFormulaDefinitionSummaries(organisationFormulaIds);

        foreach (var dto in list)
        {
            if (!dto.Id.HasValue || !formulaDefinitions.TryGetValue(dto.Id.Value, out var summary))
            {
                dto.HasFormulaDefinition = false;
                dto.FormulaDefinitionVersion = null;
                dto.FormulaDefinitionLastEditor = null;
                continue;
            }

            dto.HasFormulaDefinition = true;
            dto.FormulaDefinitionVersion = summary.Version;
            dto.FormulaDefinitionLastEditor = ResolveLastEditor(summary.LastModifiedBy, summary.CreatedBy);
        }

        return result;
    }

    private static string? ResolveLastEditor(string? lastModifiedBy, string? createdBy)
    {
        var raw = !string.IsNullOrWhiteSpace(lastModifiedBy) ? lastModifiedBy : createdBy;
        if (string.IsNullOrWhiteSpace(raw))
        {
            return null;
        }

        var separatorIndex = raw.IndexOf(" | ", StringComparison.Ordinal);
        return separatorIndex >= 0 ? raw[..separatorIndex] : raw;
    }

    private Dictionary<long, FormulaDefinitionSummary> GetFormulaDefinitionSummaries(IReadOnlyCollection<long> organisationFormulaIds)
    {
        if (organisationFormulaIds.Count == 0)
        {
            return [];
        }

        using var connection = _dapper.GetDbconnection();
        connection.Open();

        const string sql = @"
            SELECT Id, Version, LastModifiedBy, CreatedBy
            FROM [For].[Formula_Definition]
            WHERE Id IN @Ids AND IsDeleted = 0";

        var rows = connection.Query<FormulaDefinitionSummary>(sql, new { Ids = organisationFormulaIds }, commandType: CommandType.Text).ToList();
        return rows.ToDictionary(x => x.Id);
    }

    private sealed class FormulaDefinitionSummary
    {
        public long Id { get; set; }
        public int Version { get; set; }
        public string? LastModifiedBy { get; set; }
        public string? CreatedBy { get; set; }
    }

    private IQueryable<OrganisationFormula> BaseQuery() =>
        All().Include(i => i.Formula).Include(i => i.FormulaUsageLocation);
}
