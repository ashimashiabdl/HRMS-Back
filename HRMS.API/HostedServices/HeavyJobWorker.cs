using System.Diagnostics;
using HR.Order.Infrastructure.Services;
using HR.Payroll.Infrastructure.Services;
using HRMS.API.HostedServices.HeavyJobStatus;

namespace HRMS.API.HostedServices;

/// <summary>
/// سرویس پس‌زمینه اجرای پردازش‌های سنگین.
/// وضعیت درخواست‌ها در دیتابیس نگه داشته می‌شود؛ در ابتدای هر چرخه درخواست‌های گیرکرده Running بازیابی می‌شوند.
/// </summary>
public class HeavyJobWorker(
    IServiceProvider serviceProvider,
    IConfiguration configuration,
    ILogger<HeavyJobWorker> logger) : BackgroundService
{
    private const int DefaultPollingIntervalSeconds = 60;
    private const int MinPollingIntervalSeconds = 1;
    private const int DefaultStartupDelaySeconds = 0;

    private readonly IServiceProvider _serviceProvider = serviceProvider;
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<HeavyJobWorker> _logger = logger;

    private sealed record HeavyJob(string Name, Action<IServiceProvider> Execute);

    private static readonly HeavyJob[] Jobs =
    [
        new("ArearService.CheckArearsAndCalculate",
            sp => sp.GetRequiredService<ArearService>().CheckArearsAndCalculate()),

        new("InsuranceDisketteService.CalculateInsuranceDisketteBatch",
            sp => sp.GetRequiredService<InsuranceDisketteService>().CalculateInsuranceDisketteBatch()),

        new("TaxDisketteService.CalculateTaxDisketteBatch",
            sp => sp.GetRequiredService<TaxDisketteService>().CalculateTaxDisketteBatch()),

        new("BatchOrderIssueService.DoWork",
            sp => sp.GetRequiredService<BatchOrderIssueService>().DoWork(null)),

        new("FicheService.BatchCalculation",
            sp => sp.GetRequiredService<FicheService>().BatchCalculation()),

        new("BankDisketteService.CalculateBankDiskBatch",
            sp => sp.GetRequiredService<BankDisketteService>().CalculateBankDiskBatch()),
    ];

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        var startupDelay = ReadStartupDelay();
        _logger.LogInformation(
            "HeavyJobWorker started. PollingInterval={Interval}s, RunInParallel={Parallel}, StartupDelay={Startup}s",
            ReadPollingInterval().TotalSeconds, ReadRunInParallel(), startupDelay.TotalSeconds);

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

        RecoverStuckRequestsSafe("startup");

        while (!stoppingToken.IsCancellationRequested)
        {
            var runInParallel = ReadRunInParallel();
            var pollingInterval = ReadPollingInterval();
            var cycleStopwatch = Stopwatch.StartNew();

            try
            {
                _logger.LogInformation("HeavyJobWorker cycle started at {Time} (Mode={Mode})",
                    DateTimeOffset.Now, runInParallel ? "Parallel" : "Sequential");

                RecoverStuckRequestsSafe("cycle-start");

                if (runInParallel)
                {
                    await RunJobsInParallelAsync(stoppingToken);
                }
                else
                {
                    RunJobsSequentially(stoppingToken);
                }
            }
            catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                break;
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "خطای کلی در اجرای HeavyJobWorker");
            }
            finally
            {
                cycleStopwatch.Stop();
                _logger.LogInformation("HeavyJobWorker cycle finished in {Elapsed}ms",
                    cycleStopwatch.ElapsedMilliseconds);
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

        _logger.LogInformation("HeavyJobWorker stopped");
    }

    private void RunJobsSequentially(CancellationToken stoppingToken)
    {
        foreach (var job in Jobs)
        {
            if (stoppingToken.IsCancellationRequested)
            {
                return;
            }

            RunJob(job, stoppingToken);
        }
    }

    private async Task RunJobsInParallelAsync(CancellationToken stoppingToken)
    {
        var tasks = Jobs
            .Select(job => Task.Run(() => RunJob(job, stoppingToken), stoppingToken))
            .ToArray();

        await Task.WhenAll(tasks);
    }

    private void RunJob(HeavyJob job, CancellationToken stoppingToken)
    {
        if (stoppingToken.IsCancellationRequested)
        {
            return;
        }

        var stopwatch = Stopwatch.StartNew();
        try
        {
            _logger.LogInformation("شروع {Job}", job.Name);

            using var scope = _serviceProvider.CreateScope();
            job.Execute(scope.ServiceProvider);

            stopwatch.Stop();
            _logger.LogInformation("پایان موفق {Job} در {Elapsed}ms", job.Name, stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(ex, "خطا در {Job} بعد از {Elapsed}ms — در چرخه بعد دوباره اجرا می‌شود", job.Name, stopwatch.ElapsedMilliseconds);
        }
    }

    private void RecoverStuckRequestsSafe(string reason)
    {
        try
        {
            using var scope = _serviceProvider.CreateScope();
            var recovery = scope.ServiceProvider.GetRequiredService<HeavyJobStuckRequestRecovery>();
            var result = recovery.RecoverStuckRequests();
            if (result.Total > 0)
            {
                _logger.LogWarning(
                    "بازیابی گیرکرده‌ها ({Reason}): Payroll={Payroll}, Order={Order}",
                    reason, result.PayrollBatchRecovered, result.OrderBatchRecovered);
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "خطا در بازیابی درخواست‌های گیرکرده ({Reason})", reason);
        }
    }

    private TimeSpan ReadPollingInterval()
    {
        var seconds = _configuration.GetValue<int?>("HeavyJobWorker:PollingIntervalSeconds")
                      ?? DefaultPollingIntervalSeconds;

        if (seconds < MinPollingIntervalSeconds)
        {
            seconds = DefaultPollingIntervalSeconds;
        }

        return TimeSpan.FromSeconds(seconds);
    }

    private TimeSpan ReadStartupDelay()
    {
        var seconds = _configuration.GetValue<int?>("HeavyJobWorker:StartupDelaySeconds")
                      ?? DefaultStartupDelaySeconds;

        if (seconds < 0)
        {
            seconds = DefaultStartupDelaySeconds;
        }

        return TimeSpan.FromSeconds(seconds);
    }

    private bool ReadRunInParallel()
        => _configuration.GetValue<bool?>("HeavyJobWorker:RunInParallel") ?? false;
}
