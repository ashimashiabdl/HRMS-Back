using HR.Attendance.Core.DTOs;
using HR.Attendance.Core.Enums;
using HR.Attendance.Infrastructure.Services;
using HR.SharedKernel;
using HR.SharedKernel.DTOs;

namespace HR.Attendance.Infrastructure.Helpers;

internal static class ShiftDetailHelper
{
    internal static readonly int[] AllWeekDays = [6, 0, 1, 2, 3, 4, 5];

    internal static string ToWeekDayTitle(int weekDay) =>
        AttendanceCalendarService.ToPersianWeekDay(weekDay);

    internal static void ApplySummaryFields<TDetail>(IList<TDetail>? details, Action<bool, TimeSpan, TimeSpan, int, bool, bool> apply)
        where TDetail : class
    {
        if (details == null || details.Count == 0)
        {
            return;
        }

        var summary = details.FirstOrDefault(d => GetWeekDay(d) == 6) ?? details[0];
        apply(
            GetBool(summary, nameof(ShiftDetailDTO.IsFlexible)),
            GetTimeSpan(summary, nameof(ShiftDetailDTO.StartTime)),
            GetTimeSpan(summary, nameof(ShiftDetailDTO.EndTime)),
            GetInt(summary, nameof(ShiftDetailDTO.RequiredWorkSeconds)),
            GetBool(summary, nameof(ShiftDetailDTO.NightShift)),
            GetBool(summary, nameof(ShiftDetailDTO.CrossDay)));
    }

    internal static OperationResult ValidateDetailTiming(
        int weekDay,
        bool isHoliday,
        bool isFlexible,
        TimeSpan startTime,
        TimeSpan endTime,
        TimeSpan? restStart,
        TimeSpan? restEnd,
        int requiredWorkSeconds,
        bool crossDay,
        TimeSpan? minInTime,
        TimeSpan? maxInTime,
        TimeSpan? minOutTime,
        TimeSpan? maxOutTime,
        ShiftRoundType? roundType)
    {
        if (isHoliday)
        {
            return OperationResult.Succeeded();
        }

        var dayTitle = ToWeekDayTitle(weekDay);

        if (requiredWorkSeconds < 0)
        {
            return OperationResult.Failed($"مدت کار مورد نیاز در روز {dayTitle} نمی‌تواند منفی باشد");
        }

        if (!isFlexible)
        {
            if (startTime == default)
            {
                return OperationResult.Failed($"ساعت شروع کار در روز {dayTitle} الزامی است");
            }

            if (endTime == default)
            {
                return OperationResult.Failed($"ساعت پایان کار در روز {dayTitle} الزامی است");
            }

            if (!crossDay && endTime <= startTime)
            {
                return OperationResult.Failed($"ساعت پایان باید بعد از ساعت شروع در روز {dayTitle} باشد (یا گزینه عبور از نیمه‌شب را فعال کنید)");
            }
        }

        if (restStart.HasValue ^ restEnd.HasValue)
        {
            return OperationResult.Failed($"شروع و پایان استراحت در روز {dayTitle} باید هر دو مقداردهی شوند یا هر دو خالی باشند");
        }

        if (restStart.HasValue && restEnd.HasValue && restEnd <= restStart)
        {
            return OperationResult.Failed($"پایان استراحت باید بعد از شروع استراحت در روز {dayTitle} باشد");
        }

        if (minInTime.HasValue && maxInTime.HasValue && maxInTime < minInTime)
        {
            return OperationResult.Failed($"حداکثر زمان ورود باید بزرگ‌تر یا مساوی حداقل زمان ورود در روز {dayTitle} باشد");
        }

        if (minOutTime.HasValue && maxOutTime.HasValue && maxOutTime < minOutTime)
        {
            return OperationResult.Failed($"حداکثر زمان خروج باید بزرگ‌تر یا مساوی حداقل زمان خروج در روز {dayTitle} باشد");
        }

        if (roundType.HasValue && !Enum.IsDefined(typeof(ShiftRoundType), roundType.Value))
        {
            return OperationResult.Failed($"نوع گرد کردن زمان در روز {dayTitle} نامعتبر است");
        }

        return OperationResult.Succeeded();
    }

    internal static OperationResult ValidateShiftDetails(IList<ShiftDetailDTO>? details)
    {
        if (details == null || details.Count == 0)
        {
            return OperationResult.Failed("حداقل یک روز هفته باید تعریف شود");
        }

        var weekDays = details.Select(d => d.WeekDay).ToList();
        if (weekDays.Distinct().Count() != weekDays.Count)
        {
            return OperationResult.Failed("روز هفته تکراری در جزئیات شیفت وجود دارد");
        }

        foreach (var detail in details)
        {
            if (!AllWeekDays.Contains(detail.WeekDay))
            {
                return OperationResult.Failed($"روز هفته {detail.WeekDay} نامعتبر است");
            }

            var validation = ValidateDetailTiming(
                detail.WeekDay,
                detail.IsHoliday,
                detail.IsFlexible,
                detail.StartTime,
                detail.EndTime,
                detail.RestStart,
                detail.RestEnd,
                detail.RequiredWorkSeconds,
                detail.CrossDay,
                detail.MinInTime,
                detail.MaxInTime,
                detail.MinOutTime,
                detail.MaxOutTime,
                detail.RoundType);

            if (!validation.Success)
            {
                return validation;
            }
        }

        return OperationResult.Succeeded();
    }

    internal static OperationResult ValidateOverrideDetails(IList<ShiftOverrideDetailDTO>? details)
    {
        if (details == null || details.Count == 0)
        {
            return OperationResult.Failed("حداقل یک روز هفته باید تعریف شود");
        }

        var weekDays = details.Select(d => d.WeekDay).ToList();
        if (weekDays.Distinct().Count() != weekDays.Count)
        {
            return OperationResult.Failed("روز هفته تکراری در جزئیات بازتعریف شیفت وجود دارد");
        }

        foreach (var detail in details)
        {
            if (!AllWeekDays.Contains(detail.WeekDay))
            {
                return OperationResult.Failed($"روز هفته {detail.WeekDay} نامعتبر است");
            }

            var validation = ValidateDetailTiming(
                detail.WeekDay,
                detail.IsHoliday,
                detail.IsFlexible,
                detail.StartTime,
                detail.EndTime,
                detail.RestStart,
                detail.RestEnd,
                detail.RequiredWorkSeconds,
                detail.CrossDay,
                detail.MinInTime,
                detail.MaxInTime,
                detail.MinOutTime,
                detail.MaxOutTime,
                detail.RoundType);

            if (!validation.Success)
            {
                return validation;
            }
        }

        return OperationResult.Succeeded();
    }

    private static int GetWeekDay<TDetail>(TDetail detail) =>
        (int)(detail.GetType().GetProperty(nameof(ShiftDetailDTO.WeekDay))?.GetValue(detail) ?? 0);

    private static bool GetBool<TDetail>(TDetail detail, string propertyName) =>
        (bool)(detail.GetType().GetProperty(propertyName)?.GetValue(detail) ?? false);

    private static int GetInt<TDetail>(TDetail detail, string propertyName) =>
        (int)(detail.GetType().GetProperty(propertyName)?.GetValue(detail) ?? 0);

    private static TimeSpan GetTimeSpan<TDetail>(TDetail detail, string propertyName) =>
        (TimeSpan)(detail.GetType().GetProperty(propertyName)?.GetValue(detail) ?? TimeSpan.Zero);
}
