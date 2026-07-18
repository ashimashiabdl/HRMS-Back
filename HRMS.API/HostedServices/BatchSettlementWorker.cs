using HR.Payroll.Infrastructure.Services;

namespace HRMS.API.HostedServices;

/// <summary>
/// سرویس پس‌زمینه مستقل برای پردازش درخواست‌های تسویه حساب گروهی.
/// هر چرخه در Scope جداگانه اجرا می‌شود تا DbContext Thread-Safe بماند.
/// </summary>
public class BatchSettlementWorker(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<BatchSettlementWorker> logger) : BackgroundService
{
    private const int DefaultPollingIntervalSeconds = 60;
    private const int MinPollingIntervalSeconds = 5;
    private const int DefaultStartupDelaySeconds = 10;

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var startupDelay = ReadStartupDelay();
        var pollingInterval = ReadPollingInterval();

        logger.LogInformation(
            "BatchSettlementWorker started. PollingInterval={Interval}s, StartupDelay={Startup}s",
            pollingInterval.TotalSeconds, startupDelay.TotalSeconds);

        if (startupDelay > TimeSpan.Zero)
        {
            try
            {
                await Task.Delay(startupDelay, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                return;
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            pollingInterval = ReadPollingInterval();
            try
            {
                logger.LogInformation("BatchSettlementWorker cycle started at {Time}", DateTimeOffset.Now);
                using var scope = serviceProvider.CreateScope();
                scope.ServiceProvider.GetRequiredService<BatchSettlementProcessorService>().ProcessBatchSettlements();
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                logger.LogError(ex, "خطای کلی در BatchSettlementWorker");
            }

            try
            {
                await Task.Delay(pollingInterval, stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        logger.LogInformation("BatchSettlementWorker stopped");
    }

    private TimeSpan ReadPollingInterval()
    {
        var seconds = configuration.GetValue<int?>("BatchSettlementWorker:PollingIntervalSeconds")
                      ?? DefaultPollingIntervalSeconds;
        if (seconds < MinPollingIntervalSeconds)
        {
            seconds = DefaultPollingIntervalSeconds;
        }

        return TimeSpan.FromSeconds(seconds);
    }

    private TimeSpan ReadStartupDelay()
    {
        var seconds = configuration.GetValue<int?>("BatchSettlementWorker:StartupDelaySeconds")
                      ?? DefaultStartupDelaySeconds;
        return seconds < 0 ? TimeSpan.Zero : TimeSpan.FromSeconds(seconds);
    }
}
