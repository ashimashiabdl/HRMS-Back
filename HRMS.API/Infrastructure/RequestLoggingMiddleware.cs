using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;
using System.Diagnostics;

namespace HRMS.API.Infrastructure
{
    public sealed class RequestLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<RequestLoggingMiddleware> _logger;

        public RequestLoggingMiddleware(RequestDelegate next, ILogger<RequestLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            try
            {
                await _next(context);
            }
            finally
            {
                stopwatch.Stop();

                try
                {
                    LogCompletedRequest(context, stopwatch.ElapsedMilliseconds);
                }
                catch (Exception ex)
                {
                    // Logging must never break the HTTP pipeline
                    _logger.LogWarning(ex, "Failed to write request completion log for {Path}", context.Request.Path);
                }
            }
        }

        private void LogCompletedRequest(HttpContext context, long elapsedMs)
        {
            var request = context.Request;
            var response = context.Response;

            var host = request.Host;
            var port = host.Port ?? (string.Equals(request.Scheme, "https", StringComparison.OrdinalIgnoreCase) ? 443 : 80);
            var url = !string.IsNullOrWhiteSpace(request.Scheme) && !string.IsNullOrWhiteSpace(host.Host)
                ? $"{request.Scheme}://{host}{request.Path}{request.QueryString}"
                : request.Path + request.QueryString;

            var userAgent = request.Headers["User-Agent"].FirstOrDefault();
            if (string.IsNullOrWhiteSpace(userAgent))
            {
                userAgent = "Unknown";
            }

            var remoteIp = context.Connection.RemoteIpAddress?.ToString();
            if (string.IsNullOrWhiteSpace(remoteIp))
            {
                remoteIp = "Unknown";
            }

            var statusCode = ResolveStatusCode(context, response);
            var success = statusCode < 400;

            if (!context.Items.ContainsKey("UserName") ||
                context.Items["UserName"] == null ||
                string.IsNullOrWhiteSpace(context.Items["UserName"]?.ToString()))
            {
                context.Items["UserName"] = "Anonymous";
            }

            context.Items["Success"] = success ? "1" : "0";
            context.Items["StatusCode"] = statusCode;
            context.Items["Method"] = !string.IsNullOrWhiteSpace(request.Method) ? request.Method : "GET";
            context.Items["UserAgent"] = userAgent;
            context.Items["Port"] = port;
            context.Items["DurationMs"] = (int)elapsedMs;
            context.Items["IP"] = remoteIp;

            var message = "request_completed " +
                          "success=" + success.ToString().ToLowerInvariant() +
                          " status=" + statusCode +
                          " method=" + request.Method +
                          " url=\"" + url + "\"" +
                          " host=\"" + host.Host + "\"" +
                          " port=" + port +
                          " ua=\"" + userAgent.Replace("\"", "'") + "\"" +
                          " ip=" + remoteIp +
                          " durationMs=" + elapsedMs;

            if (statusCode >= 500)
            {
                _logger.LogError(message);
            }
            else if (statusCode >= 400)
            {
                _logger.LogWarning(message);
            }
            else
            {
                _logger.LogInformation(message);
            }
        }

        private static int ResolveStatusCode(HttpContext context, HttpResponse response)
        {
            if (response.StatusCode >= 400)
            {
                return response.StatusCode;
            }

            if (context.Features.Get<IExceptionHandlerFeature>()?.Error != null)
            {
                return StatusCodes.Status500InternalServerError;
            }

            return response.StatusCode > 0 ? response.StatusCode : StatusCodes.Status200OK;
        }
    }
}


