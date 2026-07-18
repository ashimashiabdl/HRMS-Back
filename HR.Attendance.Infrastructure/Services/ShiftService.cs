using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Core.Enums;
using HR.Attendance.Infrastructure.Data;
using HR.Attendance.Infrastructure.Helpers;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Reflection;
using System.Text.RegularExpressions;

using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public partial class ShiftService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<Shift, AttendanceContext, ShiftDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
        IScopedServices
{
    [GeneratedRegex(@"^#[0-9A-Fa-f]{6}$")]
    private static partial Regex HexColorRegex();

    public OperationResult GetAsKeyValuePair(long currentUserDefaultOrganId)
    {
        return OperationResult.Succeeded(payload: All()
            .Where(i => i.OrganisationChartId == currentUserDefaultOrganId && i.IsActive)
            .OrderBy(i => i.Code)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = i.Code + " - " + i.title
            }));
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
        IQueryable<Shift>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<Shift> all = All(IgnoreExpired)
            .Where(i => i.OrganisationChartId == _currentUserDefaultOrganId)
            .Include(i => i.OrganisationChart)
            .Include(i => i.Details.Where(d => !d.IsDeleted));

        var rowCount = 0;
        var pagedData = PagerUtility<Shift>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = pagedData.Select(MapShiftToDto).ToList();
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.OrganisationChart)
            .Include(i => i.Details.Where(d => !d.IsDeleted))
            .SingleOrDefault(i => i.Id == id && i.OrganisationChartId == _currentUserDefaultOrganId);

        if (row == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: MapShiftToDto(row));
    }

    public new async Task<OperationResult> CreateForAsync(ShiftDTO entityToCreate)
    {
        var validation = ValidateShift(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        var details = entityToCreate.Details;
        entityToCreate.Details = null;
        entityToCreate.OrganisationChartId = _currentUserDefaultOrganId;

        _unitOfWork.CreateTransaction();
        try
        {
            var mapped = _mapper.Map<Shift>(entityToCreate);
            if (string.IsNullOrEmpty(mapped.title))
            {
                mapped.title = string.Empty;
            }

            Add(mapped);
            if (await _unitOfWork.Save() <= 0)
            {
                _unitOfWork.Rollback();
                return OperationResult.Failed("خطا در ایجاد شیفت");
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
            return OperationResult.Failed($"خطا در ایجاد شیفت: {ex.Message}");
        }
    }

    public new async Task<OperationResult> UpdateForAsync(ShiftDTO entityToUpdate)
    {
        var validation = ValidateShift(entityToUpdate);
        if (!validation.Success)
        {
            return validation;
        }

        var details = entityToUpdate.Details;
        entityToUpdate.Details = null;
        entityToUpdate.OrganisationChartId = _currentUserDefaultOrganId;

        var shiftId = entityToUpdate.Id ?? 0;
        if (shiftId <= 0)
        {
            return OperationResult.Failed("شناسه شیفت معتبر نیست");
        }

        _unitOfWork.CreateTransaction();
        try
        {
            var existing = _unitOfWork.Context.Shifts
                .SingleOrDefault(i => i.Id == shiftId && i.OrganisationChartId == _currentUserDefaultOrganId && !i.IsDeleted);

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
                return OperationResult.Failed("خطا در بروزرسانی شیفت");
            }

            if (details != null)
            {
                await SaveDetailsAsync(shiftId, details, false);
                await _unitOfWork.Save();
            }

            _unitOfWork.Commit();
            return OperationResult.Succeeded(payload: shiftId);
        }
        catch (Exception ex)
        {
            _unitOfWork.Rollback();
            return OperationResult.Failed($"خطا در بروزرسانی شیفت: {ex.Message}");
        }
    }

    public new OperationResult DeleteRecord(long id)
    {
        var details = _unitOfWork.Context.ShiftDetails
            .Where(x => x.ShiftId == id && !x.IsDeleted)
            .ToList();

        foreach (var detail in details)
        {
            detail.IsDeleted = true;
            detail.LastModifiedDate = DateTime.Now;
            _unitOfWork.Context.ShiftDetails.Update(detail);
        }

        LogicalRemove(id);
        if (_unitOfWork.Save().Result > 0)
        {
            return OperationResult.Succeeded(payload: 1);
        }

        return OperationResult.Failed();
    }

    internal static string ToRoundTypeTitle(ShiftRoundType? roundType)
    {
        if (!roundType.HasValue)
        {
            return string.Empty;
        }

        var member = typeof(ShiftRoundType).GetMember(roundType.Value.ToString()).FirstOrDefault();
        var description = member?.GetCustomAttribute<DescriptionAttribute>()?.Description;
        return string.IsNullOrWhiteSpace(description) ? roundType.Value.ToString() : description;
    }

    private ShiftDTO MapShiftToDto(Shift shift)
    {
        var dto = _mapper.Map<ShiftDTO>(shift);
        dto.Details = shift.Details?
            .Where(d => !d.IsDeleted)
            .OrderBy(d => Array.IndexOf(ShiftDetailHelper.AllWeekDays, d.WeekDay))
            .Select(d =>
            {
                var detailDto = _mapper.Map<ShiftDetailDTO>(d);
                detailDto.WeekDayTitle = ShiftDetailHelper.ToWeekDayTitle(d.WeekDay);
                detailDto.RoundTypeTitle = ToRoundTypeTitle(d.RoundType);
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

    private async Task SaveDetailsAsync(long shiftId, List<ShiftDetailDTO> details, bool saveChanges = true)
    {
        if (details == null || details.Count == 0)
        {
            return;
        }

        var existingDetails = _unitOfWork.Context.ShiftDetails
            .Where(x => x.ShiftId == shiftId && !x.IsDeleted)
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
                _unitOfWork.Context.ShiftDetails.Update(detailToDelete);
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
                    _unitOfWork.Context.ShiftDetails.Update(existingDetail);
                }
            }
            else
            {
                var newDetail = _mapper.Map<ShiftDetail>(detailDto);
                newDetail.ShiftId = shiftId;
                newDetail.title = weekDayTitle;
                newDetail.CreateDate = DateTime.Now;
                newDetail.IPAddress = ipAddress;
                await _unitOfWork.Context.ShiftDetails.AddAsync(newDetail);
            }
        }

        if (saveChanges)
        {
            await _unitOfWork.Save();
        }
    }

    private static OperationResult ValidateShift(ShiftDTO dto)
    {
        if (string.IsNullOrWhiteSpace(dto.Code))
        {
            return OperationResult.Failed("کد شیفت الزامی است");
        }

        if (string.IsNullOrWhiteSpace(dto.Color) || !HexColorRegex().IsMatch(dto.Color))
        {
            return OperationResult.Failed("رنگ شیفت باید به صورت hex معتبر (مثلاً #2563eb) باشد");
        }

        return ShiftDetailHelper.ValidateShiftDetails(dto.Details);
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
