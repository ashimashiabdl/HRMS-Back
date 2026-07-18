using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;

namespace HR.Attendance.Infrastructure.Data;

public class AttendanceContextFactory : IDesignTimeDbContextFactory<AttendanceContext>
{
    public AttendanceContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<AttendanceContext>();

        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "HRMS.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var rawConnectionString = configuration.GetConnectionString("HRMSConnection");
        var connectionString = ConnectionStringProtector.TryUnprotect(rawConnectionString) ?? rawConnectionString;

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(AttendanceContext).Assembly.FullName);
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "att");
        });

        var userService = UserResolverService.CreateForDesignTime();
        return new AttendanceContext(optionsBuilder.Options, userService);
    }
}
