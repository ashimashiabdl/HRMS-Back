using HR.Identity.infrastructure.Data;
using HRMS.API.Scanner;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.HostedServices;

/// <summary>
/// Ensures default PermissionRoute rows exist so new modules appear in the sidebar menu.
/// </summary>
public sealed class DefaultPermissionRouteHostedService(
    IServiceScopeFactory scopeFactory,
    ILogger<DefaultPermissionRouteHostedService> logger) : IHostedService
{
    public async Task StartAsync(CancellationToken cancellationToken)
    {
        try
        {
            await using var scope = scopeFactory.CreateAsyncScope();
            var dbContext = scope.ServiceProvider.GetRequiredService<IdentityContext>();

            if (!await dbContext.Database.CanConnectAsync(cancellationToken))
            {
                logger.LogWarning("DefaultPermissionRouteHostedService skipped: database is not reachable.");
                return;
            }

            var seeder = new DefaultPermissionRouteSeeder(dbContext);
            await seeder.EnsureAttendanceRoutesAsync(cancellationToken);
            logger.LogInformation("Default attendance PermissionRoute entries ensured.");
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "Failed to ensure default PermissionRoute entries.");
        }
    }

    public Task StopAsync(CancellationToken cancellationToken) => Task.CompletedTask;
}
