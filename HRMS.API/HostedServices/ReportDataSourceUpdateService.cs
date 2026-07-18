using HR.SharedKernel.Security;
using HR.SharedKernel.Sql;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;

namespace HRMS.API.HostedServices;

/// <summary>
/// Periodically runs [rpt].[SP_Populate_EmployeeProperty] to keep report employee-property data in sync.
/// </summary>
public class ReportDataSourceUpdateService(
    ILogger<ReportDataSourceUpdateService> logger,
    IConfiguration configuration) : BackgroundService
{
    private const int DefaultIntervalMinutes = 60;
    private const int DefaultStartupDelayMinutes = 1;
    private const int DefaultCommandTimeoutSeconds = 600;
    private const int MinIntervalMinutes = 5;

    private readonly ILogger<ReportDataSourceUpdateService> _logger = logger;
    private readonly ReportDataSourceUpdateOptions _options = BuildOptions(configuration);
    private readonly string _connectionString = ResolveConnectionString(configuration);

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        if (!_options.Enabled)
        {
            _logger.LogInformation("ReportDataSourceUpdateService is disabled by configuration.");
            return;
        }

        _logger.LogInformation(
            "ReportDataSourceUpdateService started. Interval={IntervalMinutes}m, StartupDelay={StartupDelayMinutes}m, CommandTimeout={CommandTimeoutSeconds}s",
            _options.IntervalMinutes,
            _options.StartupDelayMinutes,
            _options.CommandTimeoutSeconds);

        if (_options.StartupDelayMinutes > 0)
        {
            try
            {
                await Task.Delay(TimeSpan.FromMinutes(_options.StartupDelayMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("ReportDataSourceUpdateService cancelled during startup delay.");
                return;
            }
        }

        while (!stoppingToken.IsCancellationRequested)
        {
            await RunPopulationCycleAsync(stoppingToken);

            try
            {
                var nextRun = DateTimeOffset.Now.AddMinutes(_options.IntervalMinutes);
                _logger.LogInformation(
                    "ReportDataSourceUpdateService waiting {IntervalMinutes} minute(s). Next run at {NextRun}",
                    _options.IntervalMinutes,
                    nextRun);
                await Task.Delay(TimeSpan.FromMinutes(_options.IntervalMinutes), stoppingToken);
            }
            catch (OperationCanceledException)
            {
                break;
            }
        }

        _logger.LogInformation("ReportDataSourceUpdateService stopped at {Time}", DateTimeOffset.Now);
    }

    private async Task RunPopulationCycleAsync(CancellationToken stoppingToken)
    {
        var stopwatch = System.Diagnostics.Stopwatch.StartNew();

        try
        {
            _logger.LogInformation(
                "Starting {Procedure} at {Time}",
                EmployeePropertyPopulationExecutor.ProcedureName,
                DateTimeOffset.Now);

            var result = await EmployeePropertyPopulationExecutor.ExecuteAsync(
                _connectionString,
                new EmployeePropertyPopulationExecutor.ExecutionOptions(_options.CommandTimeoutSeconds),
                stoppingToken);

            stopwatch.Stop();

            _logger.LogInformation(
                "Completed {Procedure} successfully in {DurationSeconds:F1}s ({DurationMinutes:F2}m). RowsAffected={RowsAffected}. Message={Message}",
                EmployeePropertyPopulationExecutor.ProcedureName,
                stopwatch.Elapsed.TotalSeconds,
                stopwatch.Elapsed.TotalMinutes,
                result.RowsAffected,
                result.Message);
        }
        catch (OperationCanceledException) when (stoppingToken.IsCancellationRequested)
        {
            throw;
        }
        catch (SqlException ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "SQL error in {Procedure} after {DurationSeconds:F1}s. {SqlError}",
                EmployeePropertyPopulationExecutor.ProcedureName,
                stopwatch.Elapsed.TotalSeconds,
                EmployeePropertyPopulationExecutor.FormatSqlException(ex));
        }
        catch (Exception ex)
        {
            stopwatch.Stop();
            _logger.LogError(
                ex,
                "Unexpected error in {Procedure} after {DurationSeconds:F1}s. Message={Message}",
                EmployeePropertyPopulationExecutor.ProcedureName,
                stopwatch.Elapsed.TotalSeconds,
                ex.Message);
        }
    }

    private static string ResolveConnectionString(IConfiguration configuration)
    {
        var rawConnectionString = configuration.GetConnectionString("HRMSConnection")
            ?? throw new InvalidOperationException("Connection string 'HRMSConnection' is not configured.");

        var decrypted = ConnectionStringProtector.TryUnprotect(rawConnectionString);
        return decrypted ?? rawConnectionString;
    }

    private static ReportDataSourceUpdateOptions BuildOptions(IConfiguration configuration)
    {
        var section = configuration.GetSection("ReportDataSourceUpdate");
        var options = new ReportDataSourceUpdateOptions
        {
            Enabled = section.GetValue<bool?>("Enabled") ?? true,
            IntervalMinutes = section.GetValue<int?>("IntervalMinutes") ?? DefaultIntervalMinutes,
            StartupDelayMinutes = section.GetValue<int?>("StartupDelayMinutes") ?? DefaultStartupDelayMinutes,
            CommandTimeoutSeconds = section.GetValue<int?>("CommandTimeoutSeconds") ?? DefaultCommandTimeoutSeconds,
        };

        options.IntervalMinutes = Math.Max(MinIntervalMinutes, options.IntervalMinutes);
        options.StartupDelayMinutes = Math.Max(0, options.StartupDelayMinutes);
        options.CommandTimeoutSeconds = Math.Max(30, options.CommandTimeoutSeconds);

        return options;
    }

    private sealed class ReportDataSourceUpdateOptions
    {
        public bool Enabled { get; set; }
        public int IntervalMinutes { get; set; }
        public int StartupDelayMinutes { get; set; }
        public int CommandTimeoutSeconds { get; set; }
    }
}
