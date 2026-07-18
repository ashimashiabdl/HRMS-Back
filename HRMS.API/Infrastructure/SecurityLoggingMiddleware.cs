using Microsoft.Extensions.Logging;
using System.Collections.Concurrent;
using System.Diagnostics;

namespace HRMS.API.Infrastructure
{
    /// <summary>
    /// Middleware for comprehensive security event logging
    /// </summary>
    public class SecurityLoggingMiddleware
    {
        private readonly RequestDelegate _next;
        private readonly ILogger<SecurityLoggingMiddleware> _logger;

        public SecurityLoggingMiddleware(RequestDelegate next, ILogger<SecurityLoggingMiddleware> logger)
        {
            _next = next;
            _logger = logger;
        }

        public async Task InvokeAsync(HttpContext context)
        {
            var stopwatch = Stopwatch.StartNew();
            var requestInfo = GetRequestInfo(context);
            
            // Log suspicious request patterns
            LogSuspiciousPatterns(context, requestInfo);

            try
            {
                await _next(context);
            }
            catch (Exception ex)
            {
                // This will be handled by GlobalExceptionHandler, but we can add middleware-specific logging here
                _logger.LogError(ex, "Request processing failed in SecurityLoggingMiddleware");
                throw;
            }
            finally
            {
                stopwatch.Stop();
                
                // Log security-relevant response information
                LogSecurityResponse(context, requestInfo, stopwatch.ElapsedMilliseconds);
            }
        }

        private void LogSuspiciousPatterns(HttpContext context, RequestInfo requestInfo)
        {
            // Check for common attack patterns
            var userAgent = context.Request.Headers["User-Agent"].ToString();
            var path = context.Request.Path.ToString();
            var queryString = context.Request.QueryString.ToString();

            // SQL Injection attempts
            if (ContainsSqlInjectionPattern(queryString) || ContainsSqlInjectionPattern(path))
            {
                _logger.LogWarning("SECURITY FAILURE - Possible SQL injection attempt from IP: {IP}, Path: {Path}, Query: {Query}, UserAgent: {UserAgent}",
                    requestInfo.IP, path, queryString, userAgent);
            }

            // XSS attempts
            if (ContainsXssPattern(queryString) || ContainsXssPattern(path))
            {
                _logger.LogWarning("SECURITY FAILURE - Possible XSS attempt from IP: {IP}, Path: {Path}, Query: {Query}, UserAgent: {UserAgent}",
                    requestInfo.IP, path, queryString, userAgent);
            }

            // Path traversal attempts
            if (ContainsPathTraversalPattern(path))
            {
                _logger.LogWarning("SECURITY FAILURE - Possible path traversal attempt from IP: {IP}, Path: {Path}, UserAgent: {UserAgent}",
                    requestInfo.IP, path, userAgent);
            }

            // Suspicious user agents (bots, scanners, etc.)
            if (IsSuspiciousUserAgent(userAgent))
            {
                // Reduce noise: ignore empty UA coming from local/internal probes or benign paths
                if (string.IsNullOrEmpty(userAgent))
                {
                    if (IsLocalAddress(requestInfo.IP) || IsLikelyInternalPath(path))
                    {
                        // skip noisy local/internal checks with empty user-agent
                    }
                    else
                    {
                        _logger.LogWarning("SECURITY FAILURE - Suspicious user agent detected from IP: {IP}, UserAgent: {UserAgent}, Path: {Path}",
                            requestInfo.IP, userAgent, path);
                    }
                }
                else
                {
                    _logger.LogWarning("SECURITY FAILURE - Suspicious user agent detected from IP: {IP}, UserAgent: {UserAgent}, Path: {Path}",
                        requestInfo.IP, userAgent, path);
                }
            }

            // Multiple rapid requests from same IP (basic rate limiting detection)
            CheckRateLimiting(context, requestInfo);
        }

        private void LogSecurityResponse(HttpContext context, RequestInfo requestInfo, long elapsedMs)
        {
            // Check if response has started before accessing response properties
            // Once response has started, we cannot modify response headers/status code
            if (context.Response.HasStarted)
            {
                // Skip logging response info if response has already started
                // to avoid InvalidOperationException when accessing response properties
                return;
            }

            var statusCode = context.Response.StatusCode;
            
            // Log security-relevant status codes
            if (statusCode == 401)
            {
                _logger.LogWarning("SECURITY EVENT - Unauthorized response ({StatusCode}) for IP: {IP}, Path: {Path}, Duration: {Duration}ms",
                    statusCode, requestInfo.IP, requestInfo.Path, elapsedMs);
            }
            else if (statusCode == 403)
            {
                _logger.LogWarning("SECURITY EVENT - Forbidden response ({StatusCode}) for IP: {IP}, Path: {Path}, Duration: {Duration}ms",
                    statusCode, requestInfo.IP, requestInfo.Path, elapsedMs);
            }
            else if (statusCode >= 400 && statusCode < 500)
            {
                _logger.LogInformation("Security-relevant client error ({StatusCode}) for IP: {IP}, Path: {Path}, Duration: {Duration}ms",
                    statusCode, requestInfo.IP, requestInfo.Path, elapsedMs);
            }
        }

        private static bool ContainsSqlInjectionPattern(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            
            var sqlPatterns = new[]
            {
                "union select", "union all select", "'; drop table", "'; delete from",
                "'; insert into", "'; update", "or 1=1", "or '1'='1", "and 1=1",
                "exec xp_", "sp_executesql", "'; exec", "union.*select", "select.*from"
            };

            return sqlPatterns.Any(pattern => 
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsXssPattern(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            
            var xssPatterns = new[]
            {
                "<script", "javascript:", "vbscript:", "onload=", "onerror=",
                "onclick=", "onmouseover=", "onfocus=", "onblur=", "eval(",
                "expression(", "url(javascript", "mocha:", "livescript:", "alert("
            };

            return xssPatterns.Any(pattern => 
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private static bool ContainsPathTraversalPattern(string input)
        {
            if (string.IsNullOrEmpty(input)) return false;
            
            var pathTraversalPatterns = new[]
            {
                "../", "..\\", "..%2f", "..%5c", "..%252f", "..%255c",
                "%2e%2e%2f", "%2e%2e%5c", ".%2e/", ".%2e\\", "%252e%252e%252f"
            };

            return pathTraversalPatterns.Any(pattern => 
                input.Contains(pattern, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsSuspiciousUserAgent(string userAgent)
        {
            if (string.IsNullOrEmpty(userAgent)) return true; // handled above to reduce local noise
            
            var suspiciousAgents = new[]
            {
                "nmap", "sqlmap", "nikto", "burpsuite", "owasp", "zap",
                "dirbuster", "gobuster", "wfuzz", "ffuf", "curl", "wget",
                "python-requests", "scanner", "bot", "crawler"
            };

            return suspiciousAgents.Any(agent => 
                userAgent.Contains(agent, StringComparison.OrdinalIgnoreCase));
        }

        private static bool IsLocalAddress(string ip)
        {
            if (string.IsNullOrWhiteSpace(ip)) return false;
            if (ip == "127.0.0.1" || ip == "::1") return true;
            if (System.Net.IPAddress.TryParse(ip, out var address))
            {
                return System.Net.IPAddress.IsLoopback(address);
            }
            return false;
        }

        private static bool IsLikelyInternalPath(string path)
        {
            if (string.IsNullOrEmpty(path)) return true; // root or unknown
            path = path.ToLowerInvariant();
            return path == "/" ||
                   path == "/404.html" ||
                   path.StartsWith("/swagger") ||
                   path.StartsWith("/favicon") ||
                   path.StartsWith("/health") ||
                   path.StartsWith("/metrics");
        }

        private static readonly ConcurrentDictionary<string, ConcurrentQueue<DateTime>> IpRequestTimestamps = new();

        private void CheckRateLimiting(HttpContext context, RequestInfo requestInfo)
        {
            var now = DateTime.UtcNow;
            var windowStart = now.AddMinutes(-1);
            var queue = IpRequestTimestamps.GetOrAdd(requestInfo.IP, _ => new ConcurrentQueue<DateTime>());

            queue.Enqueue(now);
            while (queue.TryPeek(out var oldest) && oldest < windowStart)
            {
                queue.TryDequeue(out _);
            }

            if (queue.Count > 100)
            {
                _logger.LogWarning("SECURITY FAILURE - Rate limit exceeded from IP: {IP}, Requests in last minute: {RequestCount}, Path: {Path}",
                    requestInfo.IP, queue.Count, requestInfo.Path);
            }

            if (IpRequestTimestamps.Count > 10000)
            {
                var fiveMinutesAgo = now.AddMinutes(-5);
                foreach (var key in IpRequestTimestamps.Keys)
                {
                    if (IpRequestTimestamps.TryGetValue(key, out var ipQueue)
                        && ipQueue.IsEmpty)
                    {
                        IpRequestTimestamps.TryRemove(key, out _);
                        continue;
                    }

                    if (IpRequestTimestamps.TryGetValue(key, out ipQueue))
                    {
                        while (ipQueue.TryPeek(out var oldest) && oldest < fiveMinutesAgo)
                        {
                            ipQueue.TryDequeue(out _);
                        }

                        if (ipQueue.IsEmpty)
                            IpRequestTimestamps.TryRemove(key, out _);
                    }
                }
            }
        }

        private RequestInfo GetRequestInfo(HttpContext context)
        {
            return new RequestInfo
            {
                IP = context.Connection.RemoteIpAddress?.ToString() ?? "Unknown",
                Path = context.Request.Path.ToString(),
                Method = context.Request.Method,
                UserAgent = context.Request.Headers["User-Agent"].ToString(),
                Referer = context.Request.Headers["Referer"].ToString()
            };
        }

        private class RequestInfo
        {
            public string IP { get; set; } = string.Empty;
            public string Path { get; set; } = string.Empty;
            public string Method { get; set; } = string.Empty;
            public string UserAgent { get; set; } = string.Empty;
            public string Referer { get; set; } = string.Empty;
        }
    }
}
