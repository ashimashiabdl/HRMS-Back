using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Infrastructure.Data;
using HR.Attendance.Infrastructure.Helpers;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class ShiftOverrideService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<ShiftOverride, AttendanceContext, ShiftOverrideDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    public new OperationResult GetPagedData(
        int currentPage = 1,
        int pageSize = 10,
        string filter = "",
        string activeSortColumn = "",
        string Sortdirection = "",
        bool IgnoreExpired = true,
        long? SelectedEmployeeTypeId = null,
        long? EmployeeId = null,
        IQueryable<ShiftOverride>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<ShiftOverride> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart)
            .Include(i => i.Details.Where(d => !d.IsDeleted));

        var rowCount = 0;
        var pagedData = PagerUtility<ShiftOverride>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = pagedData.Select(MapOverrideToDto).ToList();
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Shift)
            .Include(i => i.OrganisationChart)
            .Include(i => i.Details.Where(d => !d.IsDeleted))
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        if (row == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: MapOverrideToDto(row));
    }

    public new async Task<OperationResult> CreateForAsync(ShiftOverrideDTO entityToCreate)
    {
        var validation = await ValidateOverrideAsync(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        var details = entityToCreate.Details;
        entityToCreate.Details = null;
        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareOverrideDto(entityToCreate);

        _unitOfWork.CreateTransaction();
        try
        {
            var mapped = _mapper.Map<ShiftOverride>(entityToCreate);
            if (string.IsNullOrEmpty(mapped.title))
            {
                mapped.title = string.Empty;
            }

            Add(mapped);
            if (await _unitOfWork.Save() <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در ایجاد بازتعریف شیفت");
            }

            if (details != null && details.Count > 0)
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
            return OperationResult.Failed($"خطا در ایجاد بازتعریف شیفت: {ex.Message}");
        }
    }

    public new async Task<OperationResult> UpdateForAsync(ShiftOverrideDTO entityToUpdate)
    {
        var validation = await ValidateOverrideAsync(entityToUpdate);
        if (!validation.Success)
        {
            return validation;
        }

        var details = entityToUpdate.Details;
        entityToUpdate.Details = null;
        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;
        PrepareOverrideDto(entityToUpdate, keepTitle: true);

        var overrideId = entityToUpdate.Id ?? 0;
        if (overrideId <= 0)
        {
            return OperationResult.Failed("شناسه بازتعریف شیفت معتبر نیست");
        }

        _unitOfWork.CreateTransaction();
        try
        {
            var existing = _unitOfWork.Context.ShiftOverrides
                .SingleOrDefault(i => i.Id == overrideId && i.OrganisationChartId == _currentUserDefaultOrganId && !i.IsDeleted);

            if (existing == null)
            {
                _unitOfWork.Rollback();
                return OperationResult.NotFound();
            }

            _mapper.Map(entityToUpdate, existing);
            Update(existing);

            if (await _unitOfWork.Save() <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در بروزرسانی بازتعریف شیفت");
            }

            if (details != null)
            {
                await SaveDetailsAsync(overrideId, details, false);
                await _unitOfWork.Save();
            }

            _unitOfWork.Commit();
            return OperationResult.Succeeded(payload: overrideId);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی بازتعریف شیفت: {ex.Message}");
        }
    }

    public new OperationResult DeleteRecord(long id)
    {
        var details = _unitOfWork.Context.ShiftOverrideDetails
            .Where(x => x.ShiftOverrideId == id && !x.IsDeleted)
            .ToList();

        foreach (var detail in details)
        {
            detail.IsDeleted = true;
            detail.LastModifiedDate = DateTime.Now;
            _unitOfWork.Context.ShiftOverrideDetails.Update(detail);
        }

        LogicalRemove(id);
        if (_unitOfWork.Save().Result > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }

    private async Task<OperationResult> ValidateOverrideAsync(ShiftOverrideDTO dto)
    {
        if (dto.ShiftId <= 0)
        {
            return OperationResult.Failed("شیفت الزامی است");
        }

        if (!dto.StartDate.HasValue)
        {
            return OperationResult.Failed("تاریخ شروع اعتبار الزامی است");
        }

        if (dto.EndDate.HasValue && dto.EndDate.Value.Date < dto.StartDate.Value.Date)
        {
            return OperationResult.Failed("تاریخ پایان اعتبار باید بعد از تاریخ شروع باشد");
        }

        var shiftExists = await _unitOfWork.Context.Shifts
            .AnyAsync(i => i.Id == dto.ShiftId
                && i.OrganisationChartId == _currentUserDefaultOrganId
                && !i.IsDeleted
                && i.IsActive);

        if (!shiftExists)
        {
            return OperationResult.Failed("شیفت انتخاب‌شده معتبر نیست یا غیرفعال است");
        }

        return ShiftDetailHelper.ValidateOverrideDetails(dto.Details);
    }

    private static void PrepareOverrideDto(ShiftOverrideDTO dto, bool keepTitle = false)
    {
        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            var shiftPart = string.IsNullOrWhiteSpace(dto.Shift) ? dto.ShiftId.ToString() : dto.Shift;
            dto.title = $"بازتعریف {shiftPart}";
        }
    }

    private ShiftOverrideDTO MapOverrideToDto(ShiftOverride entity)
    {
        var dto = _mapper.Map<ShiftOverrideDTO>(entity);
        dto.Details = entity.Details?
            .Where(d => !d.IsDeleted)
            .OrderBy(d => Array.IndexOf(ShiftDetailHelper.AllWeekDays, d.WeekDay))
            .Select(d =>
            {
                var detailDto = _mapper.Map<ShiftOverrideDetailDTO>(d);
                detailDto.WeekDayTitle = ShiftDetailHelper.ToWeekDayTitle(d.WeekDay);
                detailDto.RoundTypeTitle = ShiftService.ToRoundTypeTitle(d.RoundType);
                return detailDto;
            })
            .ToList();

        ShiftDetailHelper.ApplySummaryFields(dto.Details, (isFlexible, start, end, required, night, cross) =>
        {
            dto.IsFlexible = isFlexible;
            dto.StartTime = start;
            dto.EndTime = end;
            dto.RequiredWorkSeconds = required;
            dto.NightShift = night;
            dto.CrossDay = cross;
        });

        return dto;
    }

    private async Task SaveDetailsAsync(long shiftOverrideId, List<ShiftOverrideDetailDTO> details, bool saveChanges = true)
    {
        if (details == null || details.Count == 0)
        {
            return;
        }

        var existingDetails = _unitOfWork.Context.ShiftOverrideDetails
            .Where(x => x.ShiftOverrideId == shiftOverrideId && !x.IsDeleted)
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
                _unitOfWork.Context.ShiftOverrideDetails.Update(detailToDelete);
            }
        }

        var ipAddress = GetIpAddress();

        foreach (var detailDto in details)
        {
            var weekDayTitle = ShiftDetailHelper.ToWeekDayTitle(detailDto.WeekDay);

            if (detailDto.Id.HasValue && detailDto.Id.Value > 0)
            {
                var existingDetail = existingDetails.FirstOrDefault(x => x.Id == detailDto.Id.Value);
                if (existingDetail != null)
                {
                    existingDetail.WeekDay = detailDto.WeekDay;
                    existingDetail.title = weekDayTitle;
                    existingDetail.IsHoliday = detailDto.IsHoliday;
                    existingDetail.IsFlexible = detailDto.IsFlexible;
                    existingDetail.StartTime = detailDto.StartTime;
                    existingDetail.EndTime = detailDto.EndTime;
                    existingDetail.RestStart = detailDto.RestStart;
                    existingDetail.RestEnd = detailDto.RestEnd;
                    existingDetail.RequiredWorkSeconds = detailDto.RequiredWorkSeconds;
                    existingDetail.NightShift = detailDto.NightShift;
                    existingDetail.CrossDay = detailDto.CrossDay;
                    existingDetail.MinInTime = detailDto.MinInTime;
                    existingDetail.MaxInTime = detailDto.MaxInTime;
                    existingDetail.MinOutTime = detailDto.MinOutTime;
                    existingDetail.MaxOutTime = detailDto.MaxOutTime;
                    existingDetail.RoundType = detailDto.RoundType;
                    existingDetail.LastModifiedDate = DateTime.Now;
                    existingDetail.IPAddress = ipAddress;
                    _unitOfWork.Context.ShiftOverrideDetails.Update(existingDetail);
                }
            }
            else
            {
                var newDetail = _mapper.Map<ShiftOverrideDetail>(detailDto);
                newDetail.ShiftOverrideId = shiftOverrideId;
                newDetail.title = weekDayTitle;
                newDetail.CreateDate = DateTime.Now;
                newDetail.IPAddress = ipAddress;
                await _unitOfWork.Context.ShiftOverrideDetails.AddAsync(newDetail);
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
}
