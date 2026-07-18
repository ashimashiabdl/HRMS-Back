namespace HR.SharedKernel.Extensions;

public sealed record ExceptionMappingResult(
    int StatusCode,
    string Title,
    string Detail,
    ExceptionLogLevel LogLevel,
    bool IsSecurityEvent);

public enum ExceptionLogLevel
{
    Debug,
    Warning,
    Error
}
