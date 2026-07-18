using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Configuration;

namespace HRMS.API.HostedServices
{
    /// <summary>
    /// Hosted service that pings the application every minute to prevent IIS from going idle
    /// </summary>
    public class KeepAliveHostedService : BackgroundService
    {
        private readonly ILogger<KeepAliveHostedService> _logger;
        private readonly IConfiguration _configuration;
        private readonly HttpClient _httpClient;
        private readonly KeepAliveOptions _options;
        private readonly List<string> _targetUrls;

        public KeepAliveHostedService(
            ILogger<KeepAliveHostedService> logger,
            IConfiguration configuration,
            IHttpClientFactory httpClientFactory)
        {
            _logger = logger;
            _configuration = configuration;
            // Use named client "KeepAlive" configured in Program.cs (with optional SSL bypass for IP)
            _httpClient = httpClientFactory.CreateClient("KeepAlive");
            _options = BuildOptions(configuration);
            _targetUrls = BuildTargets(_options);

            _httpClient.Timeout = TimeSpan.FromSeconds(_options.TimeoutSeconds);
            try
            {
                _httpClient.DefaultRequestHeaders.UserAgent.ParseAdd("HRMS-KeepAlive/1.0");
            }
            catch { }
        }

        protected override async Task ExecuteAsync(CancellationToken stoppingToken)
        {
            if (!_options.Enabled)
            {
                _logger.LogInformation("KeepAlive service is disabled by configuration.");
                return;
            }

            if (_targetUrls.Count == 0)
            {
                _logger.LogWarning("KeepAlive service has no valid targets. Configure KeepAlive:BaseUrl or KeepAlive:Urls.");
                return;
            }

            _logger.LogInformation(
                "KeepAlive service started. Targets: {TargetCount}, Interval: {IntervalSeconds}s, StartupDelay: {StartupDelaySeconds}s, Path: {Path}",
                _targetUrls.Count,
                _options.IntervalSeconds,
                _options.StartupDelaySeconds,
                _options.RequestPath);

            try
			{
				await Task.Delay(TimeSpan.FromSeconds(_options.StartupDelaySeconds), stoppingToken);
			}
			catch (OperationCanceledException)
			{
				return;
			}

			while (!stoppingToken.IsCancellationRequested)
            {
                try
                {
                    await PingTargets(stoppingToken);
                }
                catch (Exception ex)
                {
                    _logger.LogWarning(ex, "Unexpected keep-alive loop error");
                }

                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(_options.IntervalSeconds), stoppingToken);
                }
                catch (OperationCanceledException)
                {
                    // Expected when cancellation is requested
                    break;
                }
            }

            _logger.LogInformation("KeepAlive service stopped");
        }

        private async Task PingTargets(CancellationToken stoppingToken)
        {
            var failures = 0;

            foreach (var baseUrl in _targetUrls)
            {
                if (stoppingToken.IsCancellationRequested)
                {
                    return;
                }

                if (!await PingSingleTarget(baseUrl, stoppingToken))
                {
                    failures++;
                }
            }

            if (failures > 0)
            {
                _logger.LogWarning("KeepAlive cycle completed with {Failures} failed target(s) from {TotalTargets}.", failures, _targetUrls.Count);
            }
        }

        private async Task<bool> PingSingleTarget(string baseUrl, CancellationToken stoppingToken)
        {
            try
            {
                var requestUrl = BuildRequestUrl(baseUrl, _options.RequestPath);
                using var response = await _httpClient.GetAsync(requestUrl, HttpCompletionOption.ResponseHeadersRead, stoppingToken);

                if (response.IsSuccessStatusCode)
                {
                    _logger.LogDebug("KeepAlive successful: {Url} ({StatusCode})", requestUrl, (int)response.StatusCode);
                    return true;
                }

                _logger.LogWarning("KeepAlive non-success status {StatusCode} for {Url}", (int)response.StatusCode, requestUrl);
                return false;
            }
            catch (HttpRequestException ex)
            {
                var requestUrl = BuildRequestUrl(baseUrl, _options.RequestPath);
                if (ex.InnerException?.Message?.Contains("SSL") == true || 
                    ex.InnerException?.Message?.Contains("certificate") == true)
                {
                    _logger.LogWarning(ex, "SSL certificate error during keep-alive ping to {Url}. " +
                        "Consider setting KeepAlive:SkipCertificateValidation=true in appsettings.json " +
                        "or using HTTP instead of HTTPS for KeepAlive:BaseUrl", requestUrl);
                }
                else
                {
                    _logger.LogWarning(ex, "HTTP error during keep-alive ping to {Url}", requestUrl);
                }

                return false;
            }
            catch (TaskCanceledException) when (stoppingToken.IsCancellationRequested)
            {
                return false;
            }
            catch (TaskCanceledException ex)
            {
                var requestUrl = BuildRequestUrl(baseUrl, _options.RequestPath);
                _logger.LogWarning(ex, "KeepAlive timeout for {Url} after {TimeoutSeconds}s", requestUrl, _options.TimeoutSeconds);
                return false;
            }
        }

        private static string BuildRequestUrl(string baseUrl, string requestPath)
        {
            var normalizedBase = baseUrl.TrimEnd('/');
            var normalizedPath = string.IsNullOrWhiteSpace(requestPath) ? "/" : requestPath.Trim();
            if (!normalizedPath.StartsWith("/"))
            {
                normalizedPath = "/" + normalizedPath;
            }

            return normalizedBase + normalizedPath;
        }

        private static KeepAliveOptions BuildOptions(IConfiguration configuration)
        {
            var section = configuration.GetSection("KeepAlive");
            var options = new KeepAliveOptions
            {
                Enabled = section.GetValue<bool?>("Enabled") ?? true,
                BaseUrl = section["BaseUrl"],
                RequestPath = section["RequestPath"] ?? "/",
                IntervalSeconds = section.GetValue<int?>("IntervalSeconds") ?? 45,
                StartupDelaySeconds = section.GetValue<int?>("StartupDelaySeconds") ?? 10,
                TimeoutSeconds = section.GetValue<int?>("TimeoutSeconds") ?? 20,
                Urls = section.GetSection("Urls").Get<string[]>() ?? Array.Empty<string>()
            };

            options.IntervalSeconds = Math.Max(5, options.IntervalSeconds);
            options.StartupDelaySeconds = Math.Max(0, options.StartupDelaySeconds);
            options.TimeoutSeconds = Math.Max(3, options.TimeoutSeconds);
            options.RequestPath = string.IsNullOrWhiteSpace(options.RequestPath) ? "/" : options.RequestPath;

            return options;
        }

        private List<string> BuildTargets(KeepAliveOptions options)
        {
            var targets = new List<string>();

            if (!string.IsNullOrWhiteSpace(options.BaseUrl))
            {
                targets.Add(options.BaseUrl.TrimEnd('/'));
            }

            foreach (var url in options.Urls)
            {
                if (!string.IsNullOrWhiteSpace(url))
                {
                    targets.Add(url.Trim().TrimEnd('/'));
                }
            }

            if (targets.Count == 0)
            {
                var fallbackUrl = GetFallbackApplicationUrl();
                if (!string.IsNullOrWhiteSpace(fallbackUrl))
                {
                    targets.Add(fallbackUrl.TrimEnd('/'));
                }
            }

            return targets
                .Where(static x => !string.IsNullOrWhiteSpace(x))
                .Distinct(StringComparer.OrdinalIgnoreCase)
                .ToList();
        }

        private string GetFallbackApplicationUrl()
        {
            // Try to get from ASPNETCORE_URLS
            var urls = _configuration["ASPNETCORE_URLS"] ?? _configuration["urls"];
            if (!string.IsNullOrEmpty(urls))
            {
                var firstUrl = urls.Split(';')[0].Trim();
                _logger.LogDebug("Using URL from ASPNETCORE_URLS: {Url}", firstUrl);
                return firstUrl.TrimEnd('/');
            }

            // Default to localhost with configured port or default port
            var port = _configuration.GetValue<int?>("HttpPort") ?? 5000;
            var defaultUrl = $"http://localhost:{port}";
            _logger.LogDebug("Using default URL: {Url}", defaultUrl);
            return defaultUrl;
        }

        public override void Dispose()
        {
            _httpClient?.Dispose();
            base.Dispose();
        }

        private sealed class KeepAliveOptions
        {
            public bool Enabled { get; set; }
            public string BaseUrl { get; set; } = string.Empty;
            public string RequestPath { get; set; } = "/";
            public int IntervalSeconds { get; set; }
            public int StartupDelaySeconds { get; set; }
            public int TimeoutSeconds { get; set; }
            public string[] Urls { get; set; } = Array.Empty<string>();
        }
    }
}
