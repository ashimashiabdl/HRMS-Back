using Microsoft.AspNetCore.Http;
using Microsoft.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using System.Security;

namespace HR.SharedKernel.Extensions;

/// <summary>
/// Maps unhandled exceptions to HTTP status codes and user-facing Persian messages.
/// </summary>
public static class ExceptionResponseMapper
{
    private const string GenericServerErrorDetail =
        "خطایی در پردازش درخواست شما رخ داده است. لطفاً با پشتیبانی تماس بگیرید یا بعداً دوباره تلاش کنید.";

    private const string DuplicateRecordDetail = "امکان ثبت رکورد تکراری وجود ندارد.";
    private const string ConcurrencyDetail = "رکورد توسط کاربر دیگری تغییر یافته است. لطفاً صفحه را بارگذاری مجدد کنید.";
    private const string ConstraintViolationDetail = "نقض محدودیت داده رخ داده است. لطفاً اطلاعات را بررسی کنید.";
    private const string AccessDeniedDetail = "دسترسی مجاز نیست.";
    private const string NotFoundDetail = "منبع درخواستی یافت نشد.";
    private const string CancelledDetail = "درخواست لغو شد.";

    public static ExceptionMappingResult Map(Exception exception)
    {
        if (JsonBindingErrorMapper.TryGetBindingError(exception, out var statusCode, out var title, out var detail))
        {
            return new ExceptionMappingResult(statusCode, title, detail, ExceptionLogLevel.Warning, IsSecurityEvent: false);
        }

        if (exception is OperationCanceledException or TaskCanceledException)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status499ClientClosedRequest,
                "درخواست لغو شد",
                CancelledDetail,
                ExceptionLogLevel.Debug,
                IsSecurityEvent: false);
        }

        if (exception is UnauthorizedAccessException or SecurityException)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status403Forbidden,
                "دسترسی مجاز نیست",
                AccessDeniedDetail,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: true);
        }

        if (exception is KeyNotFoundException keyNotFound)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status404NotFound,
                "یافت نشد",
                GetUserFacingDetail(keyNotFound, NotFoundDetail),
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false);
        }

        if (exception is ArgumentException argumentException)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status400BadRequest,
                "درخواست نامعتبر",
                GetUserFacingDetail(argumentException, "درخواست نامعتبر است."),
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false);
        }

        if (exception is InvalidOperationException invalidOperation)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status400BadRequest,
                "درخواست نامعتبر",
                GetUserFacingDetail(invalidOperation, "عملیات در وضعیت فعلی مجاز نیست."),
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false);
        }

        if (exception is DbUpdateConcurrencyException)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status409Conflict,
                "تداخل داده",
                ConcurrencyDetail,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false);
        }

        var dbUpdateResult = TryMapDbUpdateException(exception);
        if (dbUpdateResult != null)
        {
            return dbUpdateResult;
        }

        if (IsSecurityRelatedByMessage(exception))
        {
            return new ExceptionMappingResult(
                StatusCodes.Status403Forbidden,
                "دسترسی مجاز نیست",
                AccessDeniedDetail,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: true);
        }

        var wrappedBusinessMessage = TryGetWrappedBusinessMessage(exception);
        if (wrappedBusinessMessage != null)
        {
            return new ExceptionMappingResult(
                StatusCodes.Status400BadRequest,
                "درخواست نامعتبر",
                wrappedBusinessMessage,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false);
        }

        return new ExceptionMappingResult(
            StatusCodes.Status500InternalServerError,
            "خطای سرور",
            GenericServerErrorDetail,
            ExceptionLogLevel.Error,
            IsSecurityEvent: false);
    }

    private static ExceptionMappingResult? TryMapDbUpdateException(Exception exception)
    {
        var dbUpdate = FindException<DbUpdateException>(exception);
        if (dbUpdate?.InnerException is not SqlException sqlException)
        {
            return null;
        }

        return sqlException.Number switch
        {
            2627 or 2601 => new ExceptionMappingResult(
                StatusCodes.Status409Conflict,
                "تداخل داده",
                DuplicateRecordDetail,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false),
            547 => new ExceptionMappingResult(
                StatusCodes.Status400BadRequest,
                "درخواست نامعتبر",
                ConstraintViolationDetail,
                ExceptionLogLevel.Warning,
                IsSecurityEvent: false),
            _ => null
        };
    }

    private static string? TryGetWrappedBusinessMessage(Exception exception)
    {
        var current = exception;
        while (current != null)
        {
            if (current is DbUpdateException or DbUpdateConcurrencyException)
            {
                break;
            }

            if (current is Exception { Message: var message } &&
                !string.IsNullOrWhiteSpace(message) &&
                !IsInternalMessage(message) &&
                current.GetType() == typeof(Exception))
            {
                return message.Trim();
            }

            current = current.InnerException;
        }

        return null;
    }

    private static bool IsSecurityRelatedByMessage(Exception exception)
    {
        var message = exception.Message;
        if (string.IsNullOrEmpty(message))
        {
            return exception.GetType().Name.Contains("Security", StringComparison.OrdinalIgnoreCase);
        }

        return message.Contains("authorization", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("authentication", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("permission", StringComparison.OrdinalIgnoreCase) ||
               message.Contains("access denied", StringComparison.OrdinalIgnoreCase) ||
               exception.GetType().Name.Contains("Security", StringComparison.OrdinalIgnoreCase);
    }

    private static string GetUserFacingDetail(Exception exception, string fallback)
    {
        var message = exception.Message;
        if (string.IsNullOrWhiteSpace(message) || IsInternalMessage(message))
        {
            return fallback;
        }

        return message.Trim();
    }

    private static bool IsInternalMessage(string message) =>
        message.Contains(" at ", StringComparison.Ordinal) ||
        message.StartsWith("System.", StringComparison.Ordinal) ||
        message.Contains("Exception:", StringComparison.OrdinalIgnoreCase);

    private static T? FindException<T>(Exception exception) where T : Exception
    {
        var current = exception;
        while (current != null)
        {
            if (current is T match)
            {
                return match;
            }

            current = current.InnerException;
        }

        return null;
    }
}
