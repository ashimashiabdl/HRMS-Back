using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HR.SharedKernel.Extensions;

/// <summary>
/// Catches JSON/model-binding errors in middleware registered before <c>UseExceptionHandler</c>.
/// Controller-level binding errors are handled by <see cref="GlobalExceptionHandler"/>.
/// </summary>
public sealed class JsonDeserializationErrorMiddleware
{
    private readonly RequestDelegate _next;
    private readonly ILogger<JsonDeserializationErrorMiddleware> _logger;

    public JsonDeserializationErrorMiddleware(
        RequestDelegate next,
        ILogger<JsonDeserializationErrorMiddleware> logger)
    {
        _next = next;
        _logger = logger;
    }

    public async Task InvokeAsync(HttpContext context)
    {
        try
        {
            await _next(context);
        }
        catch (Exception ex) when (JsonBindingErrorMapper.TryGetBindingError(ex, out var statusCode, out var title, out var detail))
        {
            if (!ExceptionHandlerHelpers.CanWriteResponse(context, context.RequestAborted))
            {
                throw;
            }

            _logger.LogWarning(
                ex,
                "JSON deserialization error - {ExceptionType}: {Message} | Path: {RequestPath}",
                ex.GetType().Name,
                ex.Message,
                context.Request.Path);

            var mapping = new ExceptionMappingResult(statusCode, title, detail, ExceptionLogLevel.Warning, IsSecurityEvent: false);
            await ExceptionHandlerHelpers.WriteProblemDetailsAsync(context, mapping, context.RequestAborted);
        }
    }
}
