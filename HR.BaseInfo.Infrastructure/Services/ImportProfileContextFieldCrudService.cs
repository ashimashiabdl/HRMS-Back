using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.BaseInfo.infrastructure.Import;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class ImportProfileContextFieldCrudService(
    IMapper mapper,
    IUnitOfWork<BaseInfoContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<ImportProfileContextField, BaseInfoContext, ImportProfileContextFieldCrudDTO>(
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
        var query = _unitOfWork.Context.Set<ImportProfileContextField>()
            .AsNoTracking()
            .Include(f => f.ImportProfile)
            .Where(f => !f.IsDeleted && f.ImportProfileId == importProfileId);

        if (!string.IsNullOrWhiteSpace(filter))
        {
            var f = filter.Trim().ToLower();
            query = query.Where(x =>
                (x.title != null && x.title.ToLower().Contains(f)) ||
                (x.TargetPropertyName != null && x.TargetPropertyName.ToLower().Contains(f)));
        }

        var totalCount = query.Count();
        var desc = string.Equals(sortDirection, "desc", StringComparison.OrdinalIgnoreCase);
        var orderBy = activeSortColumn?.ToLowerInvariant() ?? "displayorder";

        query = orderBy switch
        {
            "targetpropertyname" => desc ? query.OrderByDescending(x => x.TargetPropertyName) : query.OrderBy(x => x.TargetPropertyName),
            _ => desc ? query.OrderByDescending(x => x.DisplayOrder).ThenByDescending(x => x.Id)
                : query.OrderBy(x => x.DisplayOrder).ThenBy(x => x.Id)
        };

        var page = query
            .Skip(currentPage * pageSize)
            .Take(pageSize)
            .Select(f => new ImportProfileContextFieldCrudDTO
            {
                Id = f.Id,
                title = f.title,
                ImportProfileId = f.ImportProfileId,
                ImportProfileTitle = f.ImportProfile != null ? f.ImportProfile.title : null,
                TargetPropertyName = f.TargetPropertyName,
                DataType = f.DataType,
                ControlType = f.ControlType,
                IsRequired = f.IsRequired,
                FkLookupType = f.FkLookupType,
                FkReferenceEntity = f.FkReferenceEntity,
                FkReferenceSchema = f.FkReferenceSchema,
                DisplayOrder = f.DisplayOrder,
                ControlTypeTitle = ImportDisplayHelper.GetContextControlTypeTitle(f.ControlType),
                FkLookupTypeTitle = ImportDisplayHelper.GetFkLookupTypeTitle(f.FkLookupType),
                CreateDate = f.CreateDate,
                StartDate = f.StartDate,
                EndDate = f.EndDate
            })
            .ToList();

        return OperationResult.Succeeded(payload: page, rowCount: totalCount);
    }

    public new OperationResult Get(long id)
    {
        var baseResult = base.Get(id);
        if (!baseResult.Success || baseResult.Payload is not ImportProfileContextFieldCrudDTO dto)
            return baseResult;

        dto.ControlTypeTitle = ImportDisplayHelper.GetContextControlTypeTitle(dto.ControlType);
        dto.FkLookupTypeTitle = ImportDisplayHelper.GetFkLookupTypeTitle(dto.FkLookupType);
        return baseResult;
    }
}
