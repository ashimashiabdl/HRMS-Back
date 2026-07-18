using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.Diagnostics;

namespace HR.SharedKernel.Extensions;

internal static class ExceptionHandlerHelpers
{
    public static bool CanWriteResponse(HttpContext httpContext, CancellationToken cancellationToken) =>
        !cancellationToken.IsCancellationRequested && !httpContext.Response.HasStarted;

    public static ProblemDetails CreateProblemDetails(
        HttpContext httpContext,
        int statusCode,
        string title,
        string detail)
    {
        var problemDetails = new ProblemDetails
        {
            Status = statusCode,
            Title = title,
            Detail = detail,
            Instance = httpContext.Request.Path
        };

        problemDetails.Extensions["traceId"] = Activity.Current?.Id ?? httpContext.TraceIdentifier;
        return problemDetails;
    }

    public static async ValueTask WriteProblemDetailsAsync(
        HttpContext httpContext,
        ExceptionMappingResult mapping,
        CancellationToken cancellationToken)
    {
        httpContext.Response.StatusCode = mapping.StatusCode;
        httpContext.Response.ContentType = "application/problem+json; charset=utf-8";

        var problemDetails = CreateProblemDetails(
            httpContext,
            mapping.StatusCode,
            mapping.Title,
            mapping.Detail);

        await httpContext.Response.WriteAsJsonAsync(problemDetails, cancellationToken);
    }

    public static RequestSecurityContext GetSecurityContext(HttpContext httpContext)
    {
        var userId = "Anonymous";
        if (httpContext.User.Identity?.IsAuthenticated == true)
        {
            userId = FindClaim(httpContext, "userId", "currentUserId") ?? "Authenticated-NoId";
        }

        return new RequestSecurityContext(
            GetClientIp(httpContext),
            httpContext.Request.Headers.UserAgent.ToString(),
            httpContext.Request.Path.ToString(),
            userId);
    }

    private static string GetClientIp(HttpContext httpContext)
    {
        var forwardedFor = httpContext.Request.Headers["X-Forwarded-For"].FirstOrDefault();
        if (!string.IsNullOrWhiteSpace(forwardedFor))
        {
            var clientIp = forwardedFor
                .Split(',', StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries)
                .FirstOrDefault();

            if (!string.IsNullOrWhiteSpace(clientIp))
            {
                return clientIp;
            }
        }

        return httpContext.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
    }

    private static string? FindClaim(HttpContext httpContext, params string[] claimTypes)
    {
        foreach (var claimType in claimTypes)
        {
            var claim = httpContext.User.Claims.FirstOrDefault(c => c.Type == claimType);
            if (!string.IsNullOrEmpty(claim?.Value))
            {
                return claim.Value;
            }
        }

        return null;
    }
}

internal sealed record RequestSecurityContext(
    string IP,
    string UserAgent,
    string RequestPath,
    string UserId);
