using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HR.SharedKernel.Extensions;

public sealed class GlobalExceptionHandler : IExceptionHandler
{
    private readonly ILogger<GlobalExceptionHandler> _logger;

    public GlobalExceptionHandler(ILogger<GlobalExceptionHandler> logger)
    {
        _logger = logger;
    }

    public async ValueTask<bool> TryHandleAsync(
        HttpContext httpContext,
        Exception exception,
        CancellationToken cancellationToken)
    {
        if (!ExceptionHandlerHelpers.CanWriteResponse(httpContext, cancellationToken))
        {
            return false;
        }

        var mapping = ExceptionResponseMapper.Map(exception);
        var securityContext = ExceptionHandlerHelpers.GetSecurityContext(httpContext);

        LogException(exception, mapping, securityContext);

        await ExceptionHandlerHelpers.WriteProblemDetailsAsync(httpContext, mapping, cancellationToken);
        return true;
    }

    private void LogException(
        Exception exception,
        ExceptionMappingResult mapping,
        RequestSecurityContext securityContext)
    {
        var eventId = mapping.IsSecurityEvent ? SecurityEventId : DefaultEventId;

        switch (mapping.LogLevel)
        {
            case ExceptionLogLevel.Debug:
                _logger.LogDebug(
                    eventId,
                    exception,
                    "Request cancelled - {ExceptionType} | IP: {IP} | User: {UserId} | Path: {Path}",
                    exception.GetType().Name,
                    securityContext.IP,
                    securityContext.UserId,
                    securityContext.RequestPath);
                break;

            case ExceptionLogLevel.Warning:
                if (mapping.IsSecurityEvent)
                {
                    _logger.LogWarning(
                        eventId,
                        exception,
                        "SECURITY EVENT - {ExceptionType}: {Message} | IP: {IP} | User: {UserId} | Path: {Path} | UserAgent: {UserAgent} | Status: {StatusCode}",
                        exception.GetType().Name,
                        exception.Message,
                        securityContext.IP,
                        securityContext.UserId,
                        securityContext.RequestPath,
                        securityContext.UserAgent,
                        mapping.StatusCode);
                }
                else
                {
                    _logger.LogWarning(
                        eventId,
                        exception,
                        "Handled exception - {ExceptionType}: {Message} | IP: {IP} | User: {UserId} | Path: {Path} | Status: {StatusCode}",
                        exception.GetType().Name,
                        exception.Message,
                        securityContext.IP,
                        securityContext.UserId,
                        securityContext.RequestPath,
                        mapping.StatusCode);
                }

                break;

            default:
                _logger.LogError(
                    eventId,
                    exception,
                    "Unhandled exception - {ExceptionType}: {Message} | IP: {IP} | User: {UserId} | Path: {Path} | Status: {StatusCode}",
                    exception.GetType().Name,
                    exception.Message,
                    securityContext.IP,
                    securityContext.UserId,
                    securityContext.RequestPath,
                    mapping.StatusCode);
                break;
        }
    }

    private static readonly EventId DefaultEventId = new(1000, "GlobalExceptionHandler");
    private static readonly EventId SecurityEventId = new(1001, "GlobalExceptionHandler.Security");
}
