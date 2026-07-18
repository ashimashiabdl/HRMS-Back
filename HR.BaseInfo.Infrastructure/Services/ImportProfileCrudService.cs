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

public class ImportProfileCrudService(
    IMapper mapper,
    IUnitOfWork<BaseInfoContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<ImportProfile, BaseInfoContext, ImportProfileCrudDTO>(
        unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService), IScopedServices
{
    public OperationResult GetActiveProfiles()
    {
        var list = _unitOfWork.Context.Set<ImportProfile>()
            .AsNoTracking()
            .Where(p => p.IsActive && !p.IsDeleted)
            .OrderBy(p => p.TargetSchema ?? "")
            .ThenBy(p => p.TargetEntityName ?? "")
            .ThenBy(p => p.title)
            .Select(p => new ImportProfileListItemDTO
            {
                Id = p.Id,
                title = p.title,
                TargetEntityName = p.TargetEntityName,
                TargetSchema = p.TargetSchema,
                ModuleKey = p.ModuleKey,
                HandlerType = p.HandlerType,
                RequiresEmployeeLookup = p.RequiresEmployeeLookup,
                AllowedExtensions = p.AllowedExtensions,
                Description = p.Description
            })
            .ToList();

        return OperationResult.Succeeded(payload: list, rowCount: list.Count);
    }

    public OperationResult GetProfileDetail(long importProfileId)
    {
        var profile = _unitOfWork.Context.Set<ImportProfile>()
            .AsNoTracking()
            .Include(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefault(p => p.Id == importProfileId && !p.IsDeleted);

        if (profile == null)
            return OperationResult.NotFound();

        var dto = new ImportProfileDetailDTO
        {
            Id = profile.Id,
            title = profile.title,
            TargetEntityName = profile.TargetEntityName,
            TargetSchema = profile.TargetSchema,
            ModuleKey = profile.ModuleKey,
            HandlerType = profile.HandlerType,
            RequiresEmployeeLookup = profile.RequiresEmployeeLookup,
            AllowedExtensions = profile.AllowedExtensions,
            Description = profile.Description,
            HasHeaderRow = profile.HasHeaderRow,
            MaxRowCount = profile.MaxRowCount,
            Fields = profile.Fields
                .OrderBy(f => f.DisplayOrder)
                .ThenBy(f => f.ExcelColumnOrder)
                .Select(f => new ImportProfileFieldDTO
                {
                    Id = f.Id,
                    ExcelColumnOrder = f.ExcelColumnOrder,
                    ExcelColumnHeader = f.ExcelColumnHeader,
                    TargetPropertyName = f.TargetPropertyName,
                    DataType = f.DataType,
                    IsRequired = f.IsRequired,
                    IsUniqueKey = f.IsUniqueKey,
                    DisplayOrder = f.DisplayOrder
                })
                .ToList(),
            ContextFields = profile.ContextFields
                .OrderBy(f => f.DisplayOrder)
                .ThenBy(f => f.Id)
                .Select(f => new ImportProfileContextFieldDTO
                {
                    Id = f.Id,
                    title = f.title,
                    TargetPropertyName = f.TargetPropertyName,
                    DataType = f.DataType,
                    ControlType = f.ControlType,
                    IsRequired = f.IsRequired,
                    FkLookupType = f.FkLookupType,
                    FkReferenceEntity = f.FkReferenceEntity,
                    FkReferenceSchema = f.FkReferenceSchema,
                    DisplayOrder = f.DisplayOrder
                })
                .ToList()
        };

        return OperationResult.Succeeded(payload: dto);
    }

    public async Task<ImportProfile?> GetProfileWithFieldsAsync(long importProfileId)
    {
        return await _unitOfWork.Context.Set<ImportProfile>()
            .Include(p => p.Fields.Where(f => !f.IsDeleted))
            .Include(p => p.ContextFields.Where(f => !f.IsDeleted))
            .FirstOrDefaultAsync(p => p.Id == importProfileId && p.IsActive && !p.IsDeleted);
    }
}
