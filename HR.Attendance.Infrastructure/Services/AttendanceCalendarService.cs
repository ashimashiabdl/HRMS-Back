using AutoMapper;
using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Entities;
using HR.Attendance.Infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;

using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Services;

public class AttendanceCalendarService(
    IMapper mapper,
    IUnitOfWork<AttendanceContext> unitOfWork,
    IDapper dapper,
    IConfiguration configuration,
    UserResolverService userService)
    : BaseService<AttendanceCalendar, AttendanceContext, AttendanceCalendarDTO>(unitOfWork.Context, mapper, unitOfWork, dapper, configuration, userService),
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
        IQueryable<AttendanceCalendar>? CustomDataSource = null,
        bool IgnoreDefaultOrganId = false)
    {
        IQueryable<AttendanceCalendar> all = All(IgnoreExpired)
            .Include(i => i.Holiday);

        var rowCount = 0;
        var pagedData = PagerUtility<AttendanceCalendar>.GetPagedData(
            all, out rowCount, currentPage, pageSize, filter, activeSortColumn, Sortdirection);

        var result = _mapper.Map<List<AttendanceCalendarDTO>>(pagedData);
        return OperationResult.Succeeded(payload: result, rowCount: rowCount);
    }

    public new OperationResult Get(long id)
    {
        var row = All(false)
            .Include(i => i.Holiday)
            .SingleOrDefault(i => i.Id == id);

        var record = _mapper.Map<AttendanceCalendarDTO>(row);
        if (record == null)
        {
            return OperationResult.NotFound();
        }

        return OperationResult.Succeeded(payload: record);
    }

    public new async Task<OperationResult> CreateForAsync(AttendanceCalendarDTO entityToCreate)
    {
        var validation = PrepareCalendarDto(entityToCreate);
        if (!validation.Success)
        {
            return validation;
        }

        var duplicateValidation = await ValidateUniqueDateAsync(entityToCreate.Date);
        if (!duplicateValidation.Success)
        {
            return duplicateValidation;
        }

        return await base.CreateForAsync(entityToCreate);
    }

    public new async Task<OperationResult> UpdateForAsync(AttendanceCalendarDTO entityToUpdate)
    {
        var existing = await _unitOfWork.Context.AttendanceCalendars
            .AsNoTracking()
            .SingleOrDefaultAsync(i => i.Id == entityToUpdate.Id && !i.IsDeleted);

        if (existing == null)
        {
            return OperationResult.NotFound();
        }

        var validation = PrepareCalendarDto(entityToUpdate, keepTitle: true);
        if (!validation.Success)
        {
            return validation;
        }

        var duplicateValidation = await ValidateUniqueDateAsync(entityToUpdate.Date, entityToUpdate.Id);
        if (!duplicateValidation.Success)
        {
            return duplicateValidation;
        }

        return await base.UpdateForAsync(entityToUpdate);
    }

    private async Task<OperationResult> ValidateUniqueDateAsync(DateTime date, long? excludeId = null)
    {
        var normalizedDate = date.Date;
        var nextDate = normalizedDate.AddDays(1);

        var duplicateExists = await _unitOfWork.Context.AttendanceCalendars
            .AnyAsync(i =>
                i.Date >= normalizedDate
                && i.Date < nextDate
                && !i.IsDeleted
                && (!excludeId.HasValue || i.Id != excludeId.Value));

        if (duplicateExists)
        {
            var label = FormatPersianDateLabel(normalizedDate, (int)normalizedDate.DayOfWeek);
            return OperationResult.Failed($"برای تاریخ «{label}» قبلاً رکورد تقویم حضور و غیاب ثبت شده است");
        }

        return OperationResult.Succeeded();
    }

    private static OperationResult PrepareCalendarDto(AttendanceCalendarDTO dto, bool keepTitle = false)
    {
        if (dto.Date == default)
        {
            return OperationResult.Failed("تاریخ الزامی است");
        }

        dto.Date = dto.Date.Date;

        if (dto.IsHoliday && (!dto.HolidayId.HasValue || dto.HolidayId <= 0))
        {
            return OperationResult.Failed("برای روز تعطیل، انتخاب تعطیلات الزامی است");
        }

        if (!dto.IsHoliday)
        {
            dto.HolidayId = null;
        }

        dto.WeekDay = (int)dto.Date.DayOfWeek;
        dto.WeekDayTitle = ToPersianWeekDay(dto.WeekDay);

        if (!keepTitle && string.IsNullOrWhiteSpace(dto.title))
        {
            dto.title = dto.Date.ToString("yyyy-MM-dd");
        }

        if (!dto.StartDate.HasValue)
        {
            dto.StartDate = dto.Date;
        }

        return OperationResult.Succeeded();
    }

    internal static string ToPersianWeekDay(int weekDay) => weekDay switch
    {
        0 => "یکشنبه",
        1 => "دوشنبه",
        2 => "سه‌شنبه",
        3 => "چهارشنبه",
        4 => "پنج‌شنبه",
        5 => "جمعه",
        6 => "شنبه",
        _ => weekDay.ToString()
    };

    private static readonly string[] PersianMonths =
    [
        "فروردین", "اردیبهشت", "خرداد", "تیر", "مرداد", "شهریور",
        "مهر", "آبان", "آذر", "دی", "بهمن", "اسفند"
    ];

    public static string FormatPersianDateLabel(DateTime date, int? weekDay = null)
    {
        var persianCalendar = new System.Globalization.PersianCalendar();
        var persianYear = persianCalendar.GetYear(date);
        var persianMonth = persianCalendar.GetMonth(date);
        var persianDay = persianCalendar.GetDayOfMonth(date);
        var weekDayTitle = weekDay.HasValue
            ? ToPersianWeekDay(weekDay.Value)
            : ToPersianWeekDay((int)date.DayOfWeek);

        return $"{weekDayTitle} {persianDay} {PersianMonths[persianMonth - 1]} {persianYear}";
    }

    public new OperationResult GetAsKeyValuePair()
    {
        return OperationResult.Succeeded(payload: All()
            .OrderByDescending(i => i.Date)
            .Select(i => new HR.SharedKernel.Data.KeyValuePair
            {
                key = i.Id,
                value = FormatPersianDateLabel(i.Date, i.WeekDay)
            }));
    }
}
