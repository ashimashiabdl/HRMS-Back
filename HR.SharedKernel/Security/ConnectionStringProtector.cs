using System;
using System.Security.Cryptography;
using System.Text;
using Microsoft.Extensions.Logging;

namespace HR.SharedKernel.Security
{
    public static class ConnectionStringProtector
    {
        private const string Prefix = "enc::";
        private static ILogger? _logger;

        public static void SetLogger(ILogger logger)
        {
            _logger = logger;
        }

        public static bool IsEncrypted(string? value)
        {
            return !string.IsNullOrEmpty(value) && value.StartsWith(Prefix, StringComparison.Ordinal);
        }

        // Format: enc::<scheme>::<base64>
        public static string? TryUnprotect(string? value)
        {
            if (string.IsNullOrWhiteSpace(value)) return null;
            if (!IsEncrypted(value)) return null;

            try
            {
                var parts = value.Split(new[] {"::"}, StringSplitOptions.None);
                if (parts.Length != 3)
                {
                    _logger?.LogWarning("TryUnprotect: فرمت نامعتبر - تعداد بخش‌ها: {Count}", parts.Length);
                    return null;
                }

                var scheme = parts[1];
                var payloadB64 = parts[2];
                
                byte[] cipher;
                try
                {
                    cipher = Convert.FromBase64String(payloadB64);
                }
                catch (FormatException ex)
                {
                    _logger?.LogWarning(ex, "TryUnprotect: خطا در تبدیل Base64 - طول: {Length}", payloadB64?.Length ?? 0);
                    return null;
                }

                if (string.Equals(scheme, "dpapi-lm", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var plain = ProtectedData.Unprotect(cipher, optionalEntropy: null, scope: DataProtectionScope.LocalMachine);
                        return Encoding.UTF8.GetString(plain);
                    }
                    catch (CryptographicException ex)
                    {
                        _logger?.LogError(ex, "TryUnprotect: خطای رمزگشایی LocalMachine - HResult: 0x{HResult:X8}", ex.HResult);
                        throw; // Re-throw to preserve original behavior for critical errors
                    }
                }
                if (string.Equals(scheme, "dpapi-cu", StringComparison.OrdinalIgnoreCase))
                {
                    try
                    {
                        var plain = ProtectedData.Unprotect(cipher, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
                        return Encoding.UTF8.GetString(plain);
                    }
                    catch (CryptographicException ex)
                    {
                        _logger?.LogError(ex, "TryUnprotect: خطای رمزگشایی CurrentUser - HResult: 0x{HResult:X8}", ex.HResult);
                        throw; // Re-throw to preserve original behavior for critical errors
                    }
                }

                _logger?.LogWarning("TryUnprotect: scheme ناشناخته: {Scheme}", scheme);
                return null;
            }
            catch (CryptographicException)
            {
                // Already logged above, just return null
                return null;
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "TryUnprotect: خطای غیرمنتظره - Type: {Type}", ex.GetType().Name);
                return null;
            }
        }

        // Helper for admins to produce encrypted values (not used at runtime)
        public static string ProtectWithLocalMachine(string plain)
        {
            if (string.IsNullOrEmpty(plain))
            {
                _logger?.LogWarning("ProtectWithLocalMachine: ورودی خالی");
                throw new ArgumentException("Plain text cannot be null or empty", nameof(plain));
            }

            try
            {
                _logger?.LogDebug("ProtectWithLocalMachine: شروع رمزنگاری - طول ورودی: {Length}", plain.Length);
                
                var bytes = Encoding.UTF8.GetBytes(plain);
                _logger?.LogDebug("ProtectWithLocalMachine: تبدیل به bytes انجام شد - تعداد bytes: {Count}", bytes.Length);
                
                var cipher = ProtectedData.Protect(bytes, optionalEntropy: null, scope: DataProtectionScope.LocalMachine);
                _logger?.LogDebug("ProtectWithLocalMachine: رمزنگاری موفق - تعداد bytes رمزشده: {Count}", cipher.Length);
                
                var result = $"{Prefix}dpapi-lm::{Convert.ToBase64String(cipher)}";
                _logger?.LogInformation("ProtectWithLocalMachine: موفق - طول نهایی: {Length}", result.Length);
                
                return result;
            }
            catch (CryptographicException ex)
            {
                _logger?.LogError(ex, "ProtectWithLocalMachine: خطای رمزنگاری - HResult: 0x{HResult:X8} | Message: {Message}", 
                    ex.HResult, ex.Message);
                throw new CryptographicException(
                    $"خطا در رمزنگاری با LocalMachine DPAPI. HResult: 0x{ex.HResult:X8}. " +
                    $"احتمالاً مشکل دسترسی یا تفاوت در پروفایل ماشین است.", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ProtectWithLocalMachine: خطای غیرمنتظره - Type: {Type}", ex.GetType().FullName);
                throw;
            }
        }

        public static string ProtectWithCurrentUser(string plain)
        {
            if (string.IsNullOrEmpty(plain))
            {
                _logger?.LogWarning("ProtectWithCurrentUser: ورودی خالی");
                throw new ArgumentException("Plain text cannot be null or empty", nameof(plain));
            }

            try
            {
                _logger?.LogDebug("ProtectWithCurrentUser: شروع رمزنگاری - طول ورودی: {Length}", plain.Length);
                
                var bytes = Encoding.UTF8.GetBytes(plain);
                _logger?.LogDebug("ProtectWithCurrentUser: تبدیل به bytes انجام شد - تعداد bytes: {Count}", bytes.Length);
                
                var cipher = ProtectedData.Protect(bytes, optionalEntropy: null, scope: DataProtectionScope.CurrentUser);
                _logger?.LogDebug("ProtectWithCurrentUser: رمزنگاری موفق - تعداد bytes رمزشده: {Count}", cipher.Length);
                
                var result = $"{Prefix}dpapi-cu::{Convert.ToBase64String(cipher)}";
                _logger?.LogInformation("ProtectWithCurrentUser: موفق - طول نهایی: {Length}", result.Length);
                
                return result;
            }
            catch (CryptographicException ex)
            {
                _logger?.LogError(ex, "ProtectWithCurrentUser: خطای رمزنگاری - HResult: 0x{HResult:X8} | Message: {Message}", 
                    ex.HResult, ex.Message);
                throw new CryptographicException(
                    $"خطا در رمزنگاری با CurrentUser DPAPI. HResult: 0x{ex.HResult:X8}. " +
                    $"احتمالاً مشکل دسترسی به User Profile یا تفاوت در کاربر اجرا کننده است.", ex);
            }
            catch (Exception ex)
            {
                _logger?.LogError(ex, "ProtectWithCurrentUser: خطای غیرمنتظره - Type: {Type}", ex.GetType().FullName);
                throw;
            }
        }
    }
}


