using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class ImportProfileFieldCrudService(
    IMapper mapper,
    IUnitOfWork<BaseInfoContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<ImportProfileField, BaseInfoContext, ImportProfileFieldCrudDTO>(
        unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetPagedDataByProfile(
        long importProfileId,
        int currentPage = 0,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string sortDirection = "",
        bool ignoreExpired = true)
    {
        var query = _unitOfWork.Context.Set<ImportProfileField>()
            .AsNoTracking()
            .Include(f => f.ImportProfile)
            .Where(f => !f.IsDeleted && f.ImportProfileId == importProfileId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(x =>
                (x.title != null && x.title.ToLower().Contains(f)) ||
                (x.TargetPropertyName != null && x.TargetPropertyName.ToLower().Contains(f)) ||
                (x.ExcelColumnHeader != null && x.ExcelColumnHeader.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "displayorder";

        query = orderBy switch
        {
            "excelcolumnorder" => desc ? query.OrderByDescending(x => x.ExcelColumnOrder) : query.OrderBy(x => x.ExcelColumnOrder),
            "targetpropertyname" => desc ? query.OrderByDescending(x => x.TargetPropertyName) : query.OrderBy(x => x.TargetPropertyName),
            _ => desc ? query.OrderByDescending(x => x.DisplayOrder).ThenByDescending(x => x.ExcelColumnOrder)
                : query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.ExcelColumnOrder)
        };

        var page = query
            .Skip(currentPage * pageSize)
            .Take(pageSize)
            .Select(f => new ImportProfileFieldCrudDTO
            {
                Id = f.Id,
                title = f.title,
                ImportProfileId = f.ImportProfileId,
                ImportProfileTitle = f.ImportProfile != null ? f.ImportProfile.title : null,
                ExcelColumnOrder = f.ExcelColumnOrder,
                ExcelColumnLetter = f.ExcelColumnLetter,
                ExcelColumnHeader = f.ExcelColumnHeader,
                TargetPropertyName = f.TargetPropertyName,
                DataType = f.DataType,
                IsRequired = f.IsRequired,
                IsUniqueKey = f.IsUniqueKey,
                FkLookupType = f.FkLookupType,
                FkReferenceEntity = f.FkReferenceEntity,
                FkReferenceField = f.FkReferenceField,
                DisplayOrder = f.DisplayOrder,
                CreateDate = f.CreateDate,
                StartDate = f.StartDate,
                EndDate = f.EndDate
            })
            .ToList();

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }
}
