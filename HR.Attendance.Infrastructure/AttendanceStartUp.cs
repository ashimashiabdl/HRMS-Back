using HR.Attendance.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

namespace HR.Attendance.Infrastructure;

public static class AttendanceStartUp
{
    public static IServiceCollection AddAttendance(
        this IServiceCollection services,
        Action<DbContextOptionsBuilder> options,
        ConfigurationManager configuration)
    {
        services.AddDbContext<AttendanceContext>(options);
        services.AddScoped<DbContext, AttendanceContext>();
        return services;
    }
}
