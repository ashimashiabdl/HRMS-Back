using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using HR.SharedKernel.Service;
using HR.SharedKernel.Security;
using System.IO;

namespace HR.WorkFlow.Infrastructure.Data;

public class WorkFlowContextFactory : IDesignTimeDbContextFactory<WorkFlowContext>
{
    public WorkFlowContext CreateDbContext(string[] args)
    {
        var optionsBuilder = new DbContextOptionsBuilder<WorkFlowContext>();

        // Build configuration
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Path.Combine(Directory.GetCurrentDirectory(), "..", "HRMS.API"))
            .AddJsonFile("appsettings.json", optional: false)
            .AddJsonFile("appsettings.Development.json", optional: true)
            .Build();

        var rawConnectionString = configuration.GetConnectionString("HRMSConnection");
        
        // Decrypt connection string if it's encrypted
        var connectionString = ConnectionStringProtector.TryUnprotect(rawConnectionString) ?? rawConnectionString;

        optionsBuilder.UseSqlServer(connectionString, sql =>
        {
            sql.MigrationsAssembly(typeof(WorkFlowContext).Assembly.FullName);
            sql.MigrationsHistoryTable("__EFMigrationsHistory", "wf");
        });

        // Create a dummy UserResolverService for design-time
        var userService = UserResolverService.CreateForDesignTime();

        return new WorkFlowContext(optionsBuilder.Options, userService);
    }
}
