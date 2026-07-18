using Microsoft.AspNetCore.Http;
using System.Text.Json;
using System.Text.RegularExpressions;

namespace HR.SharedKernel.Extensions;

/// <summary>
/// Shared detection and user-friendly Persian messages for JSON deserialization / model binding errors.
/// Used by <see cref="GlobalExceptionHandler"/> and <see cref="JsonDeserializationErrorMiddleware"/>.
/// </summary>
public static class JsonBindingErrorMapper
{
    private const string DefaultDetail =
        "داده‌های ارسالی نامعتبر است. لطفاً فیلدهای فرم را بررسی و مقادیر را به درستی وارد کنید.";

    private static readonly Regex ConversionPathRegex = new(
        @"Path:\s*\$\.([^\s|]+)",
        RegexOptions.Compiled | RegexOptions.CultureInvariant);

    private static readonly Regex ConversionTypeRegex = new(
        @"could not be converted to (?:type )?'?(System\.[\w\.]+)'?",
        RegexOptions.Compiled | RegexOptions.IgnoreCase | RegexOptions.CultureInvariant);

    public static bool TryGetBindingError(Exception exception, out int statusCode, out string title, out string detail)
    {
        statusCode = StatusCodes.Status400BadRequest;
        title = "درخواست نامعتبر";
        detail = DefaultDetail;

        var ex = exception;
        while (ex != null)
        {
            if (ex is JsonException jsonEx)
            {
                detail = BuildFieldDetail(jsonEx.Path, jsonEx.Message);
                return true;
            }

            if (ex is BadHttpRequestException badReq)
            {
                var msg = badReq.Message ?? string.Empty;
                if (msg.Contains("body", StringComparison.OrdinalIgnoreCase) ||
                    msg.Contains("required", StringComparison.OrdinalIgnoreCase))
                {
                    detail = "بدنه درخواست خالی یا نامعتبر است. لطفاً اطلاعات را ارسال کنید.";
                }

                return true;
            }

            var message = ex.Message ?? string.Empty;
            if (message.Contains("could not be converted", StringComparison.OrdinalIgnoreCase) ||
                message.Contains("JSON value could not be converted", StringComparison.OrdinalIgnoreCase))
            {
                var pathMatch = ConversionPathRegex.Match(message);
                var fieldName = pathMatch.Success ? pathMatch.Groups[1].Value.Trim() : null;
                detail = BuildFieldDetail(fieldName, message);
                return true;
            }

            ex = ex.InnerException;
        }

        return false;
    }

    private static string BuildFieldDetail(string? jsonPath, string? sourceMessage)
    {
        var fieldName = TryParseJsonPathToFieldName(jsonPath);
        var typeHint = GetConversionTypeHint(sourceMessage);

        if (!string.IsNullOrEmpty(fieldName))
        {
            return $"داده‌های ارسالی نامعتبر است. مقدار فیلد «{fieldName}» باید {typeHint} باشد. لطفاً فیلدهای فرم را بررسی کنید.";
        }

        return DefaultDetail;
    }

    private static string GetConversionTypeHint(string? message)
    {
        if (string.IsNullOrWhiteSpace(message))
        {
            return "مقدار معتبر";
        }

        var typeMatch = ConversionTypeRegex.Match(message);
        if (!typeMatch.Success)
        {
            return "مقدار معتبر";
        }

        var typeName = typeMatch.Groups[1].Value;

        if (typeName.Contains("Int", StringComparison.OrdinalIgnoreCase) ||
            typeName.Contains("Byte", StringComparison.OrdinalIgnoreCase) ||
            typeName.Contains("Decimal", StringComparison.OrdinalIgnoreCase) ||
            typeName.Contains("Double", StringComparison.OrdinalIgnoreCase) ||
            typeName.Contains("Single", StringComparison.OrdinalIgnoreCase))
        {
            return typeName.Contains("Int", StringComparison.OrdinalIgnoreCase) ||
                   typeName.Contains("Byte", StringComparison.OrdinalIgnoreCase)
                ? "عدد صحیح"
                : "عدد";
        }

        if (typeName.Contains("DateTime", StringComparison.OrdinalIgnoreCase) ||
            typeName.Contains("DateOnly", StringComparison.OrdinalIgnoreCase))
        {
            return "تاریخ معتبر";
        }

        if (typeName.Contains("Boolean", StringComparison.OrdinalIgnoreCase))
        {
            return "مقدار منطقی (بله/خیر)";
        }

        if (typeName.Contains("Guid", StringComparison.OrdinalIgnoreCase))
        {
            return "شناسه معتبر";
        }

        return "مقدار معتبر";
    }

    private static string? TryParseJsonPathToFieldName(string? path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        path = path.Trim();

        if (path.StartsWith("$.", StringComparison.Ordinal))
        {
            var remainder = path.Length > 2 ? path[2..].Trim() : string.Empty;
            return GetLastPathSegment(remainder);
        }

        if (path.StartsWith("$[", StringComparison.Ordinal))
        {
            return null;
        }

        return GetLastPathSegment(path);
    }

    private static string? GetLastPathSegment(string path)
    {
        if (string.IsNullOrWhiteSpace(path))
        {
            return null;
        }

        var dotIndex = path.LastIndexOf('.');
        return dotIndex >= 0 ? path[(dotIndex + 1)..].Trim() : path.Trim();
    }
}
