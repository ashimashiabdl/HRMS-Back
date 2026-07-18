using System;
using System.Security.Cryptography;
using System.Text;
using HR.Identity.Core.Entities;
using Microsoft.AspNetCore.Identity;

namespace HR.Identity.infrastructure.Services;

public class CustomPasswordHasher : IPasswordHasher<AspNetUsers>
{
    private const int KeySize = 64;
    private const int Iterations = 350000;
    private static readonly HashAlgorithmName HashAlgorithm = HashAlgorithmName.SHA512;

    private readonly PasswordHasher<AspNetUsers> _fallback = new();

  /// <summary>
    /// هر بار که رمز عبور تنظیم می‌شود (ایجاد کاربر، تغییر توسط خود کاربر، یا ریست توسط ادمین)
    /// یک salt تصادفی یکتا تولید می‌شود تا حتی اگر دو کاربر رمز یکسان داشته باشند،
    /// مقدار ذخیره‌شده در PasswordHash متفاوت باشد.
    /// </summary>
    public string HashPassword(AspNetUsers user, string password)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (password == null) throw new ArgumentNullException(nameof(password));

        user.salt = RandomNumberGenerator.GetBytes(KeySize);

        var hash = Rfc2898DeriveBytes.Pbkdf2(
            Encoding.UTF8.GetBytes(password),
            user.salt,
            Iterations,
            HashAlgorithm,
            KeySize);

        return Convert.ToHexString(hash);
    }

    /// <summary>
    /// تأیید رمز عبور با salt مشخص (برای تاریخچه رمزهای قبلی).
    /// </summary>
    public PasswordVerificationResult VerifyPasswordWithSalt(string hashedPassword, string providedPassword, byte[]? salt)
    {
        if (string.IsNullOrEmpty(hashedPassword) || string.IsNullOrEmpty(providedPassword))
        {
            return PasswordVerificationResult.Failed;
        }

        if (salt != null && salt.Length > 0)
        {
            try
            {
                var computed = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(providedPassword),
                    salt,
                    Iterations,
                    HashAlgorithm,
                    KeySize);

                var ok = CryptographicOperations.FixedTimeEquals(computed, Convert.FromHexString(hashedPassword));
                return ok ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                // hash قدیمی Identity — salt جداگانه ندارد
            }
        }

        return _fallback.VerifyHashedPassword(new AspNetUsers(), hashedPassword, providedPassword);
    }

    public PasswordVerificationResult VerifyHashedPassword(AspNetUsers user, string hashedPassword, string providedPassword)
    {
        if (user == null) throw new ArgumentNullException(nameof(user));
        if (hashedPassword == null) return PasswordVerificationResult.Failed;

        // Verify using custom scheme when salt exists
        if (user.salt != null && user.salt.Length > 0 && !string.IsNullOrEmpty(user.PasswordHash))
        {
            try
            {
                var computed = Rfc2898DeriveBytes.Pbkdf2(
                    Encoding.UTF8.GetBytes(providedPassword),
                    user.salt,
                    Iterations,
                    HashAlgorithm,
                    KeySize);

                var ok = CryptographicOperations.FixedTimeEquals(computed, Convert.FromHexString(hashedPassword));
                return ok ? PasswordVerificationResult.Success : PasswordVerificationResult.Failed;
            }
            catch (FormatException)
            {
                // If hex conversion fails, the hash is in old format - fallback to default hasher
                var result = _fallback.VerifyHashedPassword(user, hashedPassword, providedPassword);
                return result;
            }
        }

        // Fallback to default Identity hasher for legacy records (no salt column filled)
        var result2 = _fallback.VerifyHashedPassword(user, hashedPassword, providedPassword);
        return result2;
    }
}


