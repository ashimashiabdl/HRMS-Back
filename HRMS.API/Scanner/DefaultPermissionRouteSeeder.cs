using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Scanner;

public sealed class DefaultPermissionRouteSeeder(IdentityContext dbContext)
{
    private sealed record DefaultRouteSeed(
        string Claim,
        string Title,
        string Route,
        string Icon,
        string Tooltip,
        string Description,
        int Priority,
        bool IsEmployeeSpecific = false);

    private static readonly DefaultRouteSeed[] AttendanceRoutes =
    [
        new(
            Claim: "Attendance",
            Title: "حضور و غیاب",
            Route: "attendance",
            Icon: "fingerprint",
            Tooltip: "حضور و غیاب",
            Description: "ماژول حضور و غیاب",
            Priority: 50),
        new(
            Claim: "Attendance.AbsenceType.view",
            Title: "نوع عدم حضور",
            Route: "attendance/absence-type",
            Icon: "category",
            Tooltip: "نوع عدم حضور",
            Description: "مدیریت انواع عدم حضور (مرخصی، ماموریت، غیبت و ...)",
            Priority: 8),
        new(
            Claim: "Attendance.AttendanceLocation.view",
            Title: "محل حضور",
            Route: "attendance/attendance-location",
            Icon: "location",
            Tooltip: "محل حضور",
            Description: "مدیریت محل حضور",
            Priority: 10),
        new(
            Claim: "Attendance.AttendanceDevice.view",
            Title: "دستگاه حضور",
            Route: "attendance/attendance-device",
            Icon: "technology",
            Tooltip: "دستگاه حضور",
            Description: "مدیریت دستگاه‌های حضور و غیاب",
            Priority: 20),
        new(
            Claim: "Attendance.AttendanceLog.view",
            Title: "لاگ تردد",
            Route: "attendance/attendance-log",
            Icon: "history",
            Tooltip: "لاگ تردد",
            Description: "مدیریت لاگ‌های تردد",
            Priority: 30,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.AttendanceCalendar.view",
            Title: "تقویم حضور و غیاب",
            Route: "attendance/attendance-calendar",
            Icon: "calendar",
            Tooltip: "تقویم حضور و غیاب",
            Description: "مدیریت تقویم حضور و غیاب",
            Priority: 25),
        new(
            Claim: "Attendance.Shift.view",
            Title: "شیفت",
            Route: "attendance/attendance-shift",
            Icon: "time",
            Tooltip: "شیفت",
            Description: "مدیریت شیفت‌های کاری",
            Priority: 15),
        new(
            Claim: "Attendance.ShiftOverride.view",
            Title: "بازتعریف شیفت",
            Route: "attendance/shift-override",
            Icon: "shift-override",
            Tooltip: "بازتعریف شیفت",
            Description: "بازتعریف موقت رفتار شیفت در بازه تاریخی",
            Priority: 14),
        new(
            Claim: "Attendance.EmployeeShiftAssignment.view",
            Title: "تخصیص شیفت کارمند",
            Route: "attendance/employee-shift-assignment",
            Icon: "group",
            Tooltip: "تخصیص شیفت کارمند",
            Description: "مدیریت تخصیص شیفت به کارمندان",
            Priority: 16,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.EmployeeAttendanceDailyResult.view",
            Title: "نتیجه روزانه حضور",
            Route: "attendance/employee-attendance-daily-result",
            Icon: "chart",
            Tooltip: "نتیجه روزانه حضور",
            Description: "مشاهده و مدیریت نتایج روزانه حضور کارمندان",
            Priority: 17,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.EmployeeAttendanceException.view",
            Title: "استثناء عدم حضور",
            Route: "attendance/employee-attendance-exception",
            Icon: "event_busy",
            Tooltip: "استثناء عدم حضور",
            Description: "مشاهده و مدیریت بازه‌های زمانی عدم حضور کارمندان بر اساس شیفت",
            Priority: 18,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.EmployeeExceptionJustificationRequestState.view",
            Title: "وضعیت درخواست توجیه استثناء",
            Route: "attendance/employee-exception-justification-request-state",
            Icon: "flag",
            Tooltip: "وضعیت درخواست توجیه استثناء",
            Description: "مدیریت وضعیت‌های درخواست توجیه استثناء عدم حضور کارمند",
            Priority: 7),
        new(
            Claim: "Attendance.EmployeeExceptionJustificationRequest.view",
            Title: "درخواست توجیه استثناء",
            Route: "attendance/employee-exception-justification-request",
            Icon: "assignment",
            Tooltip: "درخواست توجیه استثناء",
            Description: "مدیریت درخواست‌های توجیه استثناء عدم حضور کارمند",
            Priority: 19,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.EmployeeMonthlySummary.view",
            Title: "خلاصه ماهانه حضور",
            Route: "attendance/employee-monthly-summary",
            Icon: "summarize",
            Tooltip: "خلاصه ماهانه حضور",
            Description: "مشاهده و مدیریت خلاصه ماهانه حضور و کارکرد کارمندان",
            Priority: 20,
            IsEmployeeSpecific: true),
        new(
            Claim: "Attendance.EmployeeHome.view",
            Title: "حضور و غیاب کارمند",
            Route: "attendance/employee-home",
            Icon: "fingerprint",
            Tooltip: "حضور و غیاب کارمند",
            Description: "صفحه اختصاصی حضور و غیاب کارمند",
            Priority: 5,
            IsEmployeeSpecific: true),
    ];

    public async Task EnsureAttendanceRoutesAsync(CancellationToken cancellationToken = default)
    {
        await EnsureRoutesAsync(AttendanceRoutes, cancellationToken);
    }

    private async Task EnsureRoutesAsync(IEnumerable<DefaultRouteSeed> routes, CancellationToken cancellationToken)
    {
        var existingRoutes = await dbContext.PermissionRoutes
            .IgnoreQueryFilters()
            .Where(r => r.Claim != null)
            .ToListAsync(cancellationToken);

        var existingByClaim = existingRoutes.ToDictionary(
            r => NormalizeClaim(r.Claim!),
            StringComparer.OrdinalIgnoreCase);

        var now = DateTime.Now;
        var changed = false;

        foreach (var route in routes)
        {
            var claimKey = NormalizeClaim(route.Claim);
            if (existingByClaim.TryGetValue(claimKey, out var existing))
            {
                if (existing.Icon != route.Icon
                    || existing.Route != route.Route
                    || existing.title != route.Title
                    || existing.IsEmployeeSpecific != route.IsEmployeeSpecific)
                {
                    existing.Icon = route.Icon;
                    existing.Route = route.Route;
                    existing.title = route.Title;
                    existing.Tooltip = route.Tooltip;
                    existing.Description = route.Description;
                    existing.Priority = route.Priority;
                    existing.IsEmployeeSpecific = route.IsEmployeeSpecific;
                    existing.LastModifiedDate = now;
                    changed = true;
                }

                continue;
            }

            dbContext.PermissionRoutes.Add(new PermissionRoute
            {
                title = route.Title,
                Claim = route.Claim,
                Route = route.Route,
                Icon = route.Icon,
                Tooltip = route.Tooltip,
                Description = route.Description,
                Priority = route.Priority,
                IsEmployeeSpecific = route.IsEmployeeSpecific,
                IsSpecial = false,
                CreateDate = now,
                StartDate = now,
                IsDeleted = false,
                IPAddress = "system",
            });
            changed = true;
        }

        if (changed)
        {
            await dbContext.SaveChangesAsync(cancellationToken);
        }
    }

    private static string NormalizeClaim(string value) =>
        value.Trim().ToLowerInvariant();
}
