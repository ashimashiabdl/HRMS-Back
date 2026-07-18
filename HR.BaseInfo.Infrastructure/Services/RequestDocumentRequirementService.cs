using AutoMapper;
using HR.BaseInfo.Core.DTOs;
using HR.BaseInfo.Core.Entities;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.BaseInfo.infrastructure.Services;

public class RequestDocumentRequirementService(
    IMapper mapper,
    IUnitOfWork<BaseInfoContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<RequestDocumentRequirement, BaseInfoContext, RequestDocumentRequirementDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    public new async Task<OperationResult> CreateForAsync(RequestDocumentRequirementDTO dto)
    {
        _unitOfWork.CreateTransaction();
        try
        {
            var details = dto.Details;
            dto.Details = null;
            dto.Description = NormalizeDescription(dto.Description);

            var mapped = _mapper.Map<RequestDocumentRequirement>(dto);
            if (string.IsNullOrEmpty(mapped.title))
            {
                mapped.title = string.Empty;
            }

            Add(mapped);
            if (await _unitOfWork.Save() <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در ایجاد نوع سند");
            }

            if (details != null && details.Any())
            {
                await SaveDetailsAsync(mapped.Id, details, false);
                await _unitOfWork.Save();
            }

            _unitOfWork.Commit();
            return OperationResult.Succeeded(payload: mapped.Id);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در ایجاد نوع سند: {ex.Message}");
        }
    }

    public new async Task<OperationResult> UpdateForAsync(RequestDocumentRequirementDTO dto)
    {
        _unitOfWork.CreateTransaction();
        try
        {
            var details = dto.Details;
            dto.Details = null;
            dto.Description = NormalizeDescription(dto.Description);

            var requirementId = dto.Id ?? 0;
            if (requirementId <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("شناسه نوع سند معتبر نیست");
            }

            var existing = _unitOfWork.Context.RequestDocumentRequirements
                .FirstOrDefault(x => x.Id == requirementId);
            if (existing == null)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("نوع سند یافت نشد");
            }

            _mapper.Map(dto, existing);

            Update(existing);
            if (await _unitOfWork.Save() <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در بروزرسانی نوع سند");
            }

            if (details != null)
            {
                await SaveDetailsAsync(requirementId, details, false);
                await _unitOfWork.Save();
            }

            _unitOfWork.Commit();
            return OperationResult.Succeeded(payload: requirementId);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی نوع سند: {ex.Message}");
        }
    }

    public OperationResult GetWithDetails(long id)
    {
        var entity = _unitOfWork.Context.RequestDocumentRequirements
            .Include(x => x.Details.Where(d => !d.IsDeleted))
            .FirstOrDefault(x => x.Id == id);

        if (entity == null)
        {
            return OperationResult.Failed("نوع سند یافت نشد");
        }

        var dto = _mapper.Map<RequestDocumentRequirementDTO>(entity);
        return OperationResult.Succeeded(payload: dto);
    }

    public new OperationResult DeleteRecord(long id)
    {
        var details = _unitOfWork.Context.RequestDocumentRequirementDetails
            .Where(x => x.RequestDocumentRequirementId == id && !x.IsDeleted)
            .ToList();

        foreach (var detail in details)
        {
            detail.IsDeleted = true;
            detail.LastModifiedDate = DateTime.Now;
            _unitOfWork.Context.RequestDocumentRequirementDetails.Update(detail);
        }

        LogicalRemove(id);
        if (_unitOfWork.Save().Result > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }

    private async Task SaveDetailsAsync(long requirementId, List<RequestDocumentRequirementDetailDTO> details, bool saveChanges = true)
    {
        if (details == null || !details.Any())
        {
            return;
        }

        var existingDetails = _unitOfWork.Context.RequestDocumentRequirementDetails
            .Where(x => x.RequestDocumentRequirementId == requirementId && !x.IsDeleted)
            .ToList();

        var existingIds = existingDetails.Select(x => x.Id).ToList();
        var currentIds = details.Where(x => x.Id.HasValue && x.Id.Value > 0).Select(x => x.Id!.Value).ToList();

        var toDelete = existingIds.Where(x => !currentIds.Contains(x)).ToList();
        foreach (var detailId in toDelete)
        {
            var detailToDelete = existingDetails.FirstOrDefault(x => x.Id == detailId);
            if (detailToDelete != null)
            {
                detailToDelete.IsDeleted = true;
                detailToDelete.LastModifiedDate = DateTime.Now;
                _unitOfWork.Context.RequestDocumentRequirementDetails.Update(detailToDelete);
            }
        }

        var ipAddress = GetIpAddress();

        foreach (var detailDto in details)
        {
            if (string.IsNullOrWhiteSpace(detailDto.title))
            {
                continue;
            }

            if (detailDto.Id.HasValue && detailDto.Id.Value > 0)
            {
                var existingDetail = existingDetails.FirstOrDefault(x => x.Id == detailDto.Id.Value);
                if (existingDetail != null)
                {
                    existingDetail.title = detailDto.title.Trim();
                    existingDetail.IsRequired = detailDto.IsRequired;
                    existingDetail.Description = string.IsNullOrWhiteSpace(detailDto.Description)
                        ? null
                        : detailDto.Description.Trim();
                    existingDetail.LastModifiedDate = DateTime.Now;
                    existingDetail.IPAddress = ipAddress;
                    _unitOfWork.Context.RequestDocumentRequirementDetails.Update(existingDetail);
                }
            }
            else
            {
                var newDetail = _mapper.Map<RequestDocumentRequirementDetail>(detailDto);
                newDetail.RequestDocumentRequirementId = requirementId;
                newDetail.title = detailDto.title.Trim();
                newDetail.Description = string.IsNullOrWhiteSpace(detailDto.Description)
                    ? null
                    : detailDto.Description.Trim();
                newDetail.CreateDate = DateTime.Now;
                newDetail.LastModifiedDate = DateTime.Now;
                newDetail.IPAddress = ipAddress;
                newDetail.IsDeleted = false;
                _unitOfWork.Context.RequestDocumentRequirementDetails.Add(newDetail);
            }
        }

        if (saveChanges)
        {
            await _unitOfWork.Save();
        }
    }

    private string GetIpAddress()
    {
        try
        {
            var ipAddress = userService.GetIP();
            if (string.IsNullOrEmpty(ipAddress) || ipAddress == "Notfound")
            {
                return "Local";
            }

            return ipAddress;
        }
        catch
        {
            return "Local";
        }
    }

    private static string? NormalizeDescription(string? description)
    {
        return string.IsNullOrWhiteSpace(description) ? null : description.Trim();
    }
}
