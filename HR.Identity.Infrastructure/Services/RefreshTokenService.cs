using System;
using System.Linq;
using System.Security.Cryptography;
using System.Threading.Tasks;
using HR.Identity.Core.Entities;
using HR.Identity.Core.DTOs;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Security;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace HR.Identity.infrastructure.Services;

/// <summary>
/// Service for managing refresh tokens
/// </summary>
public class RefreshTokenService
{
    private readonly IdentityContext _context;
    private readonly CustomIdentityContext _customContext;
    private readonly UserResolverService _userResolverService;
    private readonly ILogger<RefreshTokenService> _logger;
    private readonly Identitysetting _settings;

    // Defaults used when configuration is missing/invalid
    private const int DefaultRefreshTokenExpirationDays = 7;
    private const int DefaultMaxActiveRefreshTokensPerUser = 5;

    /// <summary>طول عمر توکن تازه‌سازی به روز (از تنظیمات JWT)</summary>
    public int RefreshTokenDays =>
        _settings != null && _settings.RefreshTokenDays > 0 ? _settings.RefreshTokenDays : DefaultRefreshTokenExpirationDays;

    private int RefreshTokenExpirationDays => RefreshTokenDays;

    private int MaxActiveRefreshTokensPerUser =>
        _settings != null && _settings.MaxActiveRefreshTokensPerUser > 0 ? _settings.MaxActiveRefreshTokensPerUser : DefaultMaxActiveRefreshTokensPerUser;

    public RefreshTokenService(
        IdentityContext context,
        CustomIdentityContext customContext,
        UserResolverService userResolverService,
        IOptions<Identitysetting> settings,
        ILogger<RefreshTokenService> logger)
    {
        _context = context;
        _customContext = customContext;
        _userResolverService = userResolverService;
        _settings = settings?.Value;
        _logger = logger;
    }

    /// <summary>
    /// Generate a new cryptographically secure refresh token
    /// </summary>
    public string GenerateRefreshToken()
    {
        var randomBytes = new byte[64];
        using var rng = RandomNumberGenerator.Create();
        rng.GetBytes(randomBytes);
        return Convert.ToBase64String(randomBytes);
    }

    /// <summary>
    /// Create and store a new refresh token for a user
    /// </summary>
    public async Task<OperationResult> CreateRefreshTokenAsync(long userId, string ipAddress)
    {
        try
        {
            var user = await _context.AspNetUsers.FindAsync(userId);
            if (user == null)
            {
                return OperationResult.NotFound("کاربر یافت نشد");
            }

            // Generate unique token
            var token = GenerateRefreshToken();
            
            // Hash the token before storing (security best practice)
            var hashedToken = HashToken(token);

            var refreshToken = new RefreshToken
            {
                Token = hashedToken,
                UserId = userId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
                CreatedByIp = ipAddress,
                SecurityStamp = user.SecurityStamp
            };

            _context.RefreshTokens.Add(refreshToken);

            // Clean up old/expired tokens for this user
            await CleanupExpiredTokensAsync(userId);

            // Enforce maximum active tokens limit
            await EnforceTokenLimitAsync(userId);

            await _context.SaveChangesAsync();

            _logger.LogInformation("Created refresh token for user {UserId} from IP {IP}", userId, ipAddress);

            // Return the plain token (not hashed) to send to client
            return OperationResult.Succeeded(payload: token);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error creating refresh token for user {UserId}", userId);
            return OperationResult.Failed("خطا در ایجاد توکن تازه‌سازی");
        }
    }

    /// <summary>
    /// Validate a refresh token and return the user ID if valid
    /// </summary>
    public async Task<OperationResult> ValidateRefreshTokenAsync(string token, string ipAddress)
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return OperationResult.Failed("توکن تازه‌سازی معتبر نیست");
            }

            var hashedToken = HashToken(token);

            var refreshToken = await _context.RefreshTokens
                .Include(rt => rt.User)
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

            if (refreshToken == null)
            {
                _logger.LogWarning("Refresh token not found - IP: {IP}", ipAddress);
                return OperationResult.Failed("توکن تازه‌سازی معتبر نیست");
            }

            // Check if token is revoked
            if (refreshToken.RevokedAt != null)
            {
                // Token reuse detection: an already-rotated (replaced) token is being presented again.
                // This strongly indicates token theft/replay -> revoke the entire token family for safety.
                if (!string.IsNullOrEmpty(refreshToken.ReplacedByToken))
                {
                    _logger.LogWarning("SECURITY - Refresh token REUSE detected, revoking all user tokens - UserId: {UserId}, IP: {IP}",
                        refreshToken.UserId, ipAddress);
                    await RevokeAllUserTokensAsync(refreshToken.UserId, ipAddress, "Reuse detected - possible token theft");
                }
                else
                {
                    _logger.LogWarning("Attempted use of revoked refresh token - UserId: {UserId}, IP: {IP}",
                        refreshToken.UserId, ipAddress);
                }
                return OperationResult.Failed("توکن تازه‌سازی باطل شده است");
            }

            // Check if token is expired
            if (refreshToken.IsExpired)
            {
                _logger.LogWarning("Attempted use of expired refresh token - UserId: {UserId}, IP: {IP}", 
                    refreshToken.UserId, ipAddress);
                return OperationResult.Failed("توکن تازه‌سازی منقضی شده است");
            }

            // Check if user still exists and is active
            if (refreshToken.User == null || refreshToken.User.LockoutEnabled)
            {
                _logger.LogWarning("Refresh token user not found or locked - UserId: {UserId}, IP: {IP}", 
                    refreshToken.UserId, ipAddress);
                return OperationResult.Failed("کاربر یافت نشد یا غیرفعال است");
            }

            // Security stamp must match — rejects refresh after login/password change on another device
            var currentSecurityStamp = await _context.AspNetUsers.AsNoTracking()
                .Where(u => u.Id == refreshToken.UserId)
                .Select(u => u.SecurityStamp)
                .FirstOrDefaultAsync();

            if (!string.IsNullOrEmpty(refreshToken.SecurityStamp)
                && !string.Equals(currentSecurityStamp, refreshToken.SecurityStamp, StringComparison.Ordinal))
            {
                _logger.LogWarning(
                    "SECURITY - Refresh token SecurityStamp mismatch - UserId: {UserId}, IP: {IP}",
                    refreshToken.UserId, ipAddress);
                await RevokeRefreshTokenAsync(token, ipAddress, "SecurityStamp mismatch - session superseded");
                return OperationResult.Failed("نشست شما منقضی شده است. لطفاً مجدداً وارد شوید");
            }

            // Validate IP address - refresh token should only work from the IP it was created from
            if (!string.IsNullOrEmpty(refreshToken.CreatedByIp) && refreshToken.CreatedByIp != "unknown")
            {
                if (refreshToken.CreatedByIp != ipAddress)
                {
                    _logger.LogWarning("Refresh token IP mismatch - Token IP: {TokenIp}, Current IP: {CurrentIp}, UserId: {UserId}", 
                        refreshToken.CreatedByIp, ipAddress, refreshToken.UserId);
                  //  return OperationResult.Failed("توکن تازه‌سازی فقط از IP اصلی قابل استفاده است");
                }
            }

            _logger.LogInformation("Refresh token validated successfully - UserId: {UserId}, IP: {IP}", 
                refreshToken.UserId, ipAddress);

            return OperationResult.Succeeded(payload: refreshToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error validating refresh token from IP {IP}", ipAddress);
            return OperationResult.Failed("خطا در اعتبارسنجی توکن تازه‌سازی");
        }
    }

    /// <summary>
    /// Revoke a refresh token
    /// </summary>
    public async Task<OperationResult> RevokeRefreshTokenAsync(string token, string ipAddress, string reason = "Revoked by user")
    {
        try
        {
            if (string.IsNullOrWhiteSpace(token))
            {
                return OperationResult.Failed("توکن تازه‌سازی معتبر نیست");
            }

            var hashedToken = HashToken(token);

            var refreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == hashedToken);

            if (refreshToken == null)
            {
                return OperationResult.NotFound("توکن تازه‌سازی یافت نشد");
            }

            if (refreshToken.RevokedAt != null)
            {
                return OperationResult.Failed("توکن تازه‌سازی قبلاً باطل شده است");
            }

            // Revoke the token
            refreshToken.RevokedAt = DateTime.UtcNow;
            refreshToken.RevokedByIp = ipAddress;
            refreshToken.RevocationReason = reason;

            _context.RefreshTokens.Update(refreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Revoked refresh token for user {UserId} from IP {IP}, Reason: {Reason}", 
                refreshToken.UserId, ipAddress, reason);

            return OperationResult.Succeeded();
        }
        catch (DbUpdateConcurrencyException ex)
        {
            // Token was modified or deleted since load (e.g. concurrent logout, cleanup, or token limit). Treat as success.
            _logger.LogWarning(ex, "Refresh token concurrency on revoke - token may already be revoked/deleted. IP: {IP}", ipAddress);
            return OperationResult.Succeeded();
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking refresh token from IP {IP}", ipAddress);
            return OperationResult.Failed("خطا در باطل کردن توکن تازه‌سازی");
        }
    }

    /// <summary>
    /// Revoke all refresh tokens for a user
    /// </summary>
    public async Task<OperationResult> RevokeAllUserTokensAsync(long userId, string ipAddress, string reason = "Revoked all tokens")
    {
        try
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null)
                .ToListAsync();

            if (!activeTokens.Any())
            {
                return OperationResult.Succeeded( "هیچ توکن فعالی برای باطل کردن وجود ندارد");
            }

            foreach (var token in activeTokens)
            {
                token.RevokedAt = DateTime.UtcNow;
                token.RevokedByIp = ipAddress;
                token.RevocationReason = reason;
            }

            _context.RefreshTokens.UpdateRange(activeTokens);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Revoked all refresh tokens for user {UserId} from IP {IP}, Count: {Count}", 
                userId, ipAddress, activeTokens.Count);

            return OperationResult.Succeeded( $"{activeTokens.Count} توکن باطل شد");
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error revoking all tokens for user {UserId}", userId);
            return OperationResult.Failed("خطا در باطل کردن توکن‌ها");
        }
    }

    /// <summary>
    /// Replace an old refresh token with a new one (used during token refresh)
    /// </summary>
    public async Task<OperationResult> RotateRefreshTokenAsync(string oldToken, string ipAddress)
    {
        try
        {
            var hashedOldToken = HashToken(oldToken);

            var oldRefreshToken = await _context.RefreshTokens
                .FirstOrDefaultAsync(rt => rt.Token == hashedOldToken);

            if (oldRefreshToken == null)
            {
                return OperationResult.Failed("توکن تازه‌سازی معتبر نیست");
            }

            var currentSecurityStamp = await _context.AspNetUsers.AsNoTracking()
                .Where(u => u.Id == oldRefreshToken.UserId)
                .Select(u => u.SecurityStamp)
                .FirstOrDefaultAsync();

            // Generate new token
            var newToken = GenerateRefreshToken();
            var hashedNewToken = HashToken(newToken);

            // Revoke old token and link to new one
            oldRefreshToken.RevokedAt = DateTime.UtcNow;
            oldRefreshToken.RevokedByIp = ipAddress;
            oldRefreshToken.ReplacedByToken = hashedNewToken;
            oldRefreshToken.RevocationReason = "Replaced by new token";

            // Create new refresh token
            var newRefreshToken = new RefreshToken
            {
                Token = hashedNewToken,
                UserId = oldRefreshToken.UserId,
                CreatedAt = DateTime.UtcNow,
                ExpiresAt = DateTime.UtcNow.AddDays(RefreshTokenExpirationDays),
                CreatedByIp = ipAddress,
                SecurityStamp = currentSecurityStamp
            };

            _context.RefreshTokens.Update(oldRefreshToken);
            _context.RefreshTokens.Add(newRefreshToken);
            await _context.SaveChangesAsync();

            _logger.LogInformation("Rotated refresh token for user {UserId} from IP {IP}", 
                oldRefreshToken.UserId, ipAddress);

            // Return the plain new token (not hashed)
            return OperationResult.Succeeded(payload: newToken);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error rotating refresh token from IP {IP}", ipAddress);
            return OperationResult.Failed("خطا در تازه‌سازی توکن");
        }
    }

    /// <summary>
    /// Clean up expired refresh tokens for a user
    /// </summary>
    private async Task CleanupExpiredTokensAsync(long userId)
    {
        var expiredTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.ExpiresAt < DateTime.UtcNow)
            .ToListAsync();

        if (expiredTokens.Any())
        {
            _context.RefreshTokens.RemoveRange(expiredTokens);
            _logger.LogInformation("Cleaned up {Count} expired tokens for user {UserId}", 
                expiredTokens.Count, userId);
        }
    }

    /// <summary>
    /// Enforce maximum active tokens limit per user
    /// </summary>
    private async Task EnforceTokenLimitAsync(long userId)
    {
        var activeTokens = await _context.RefreshTokens
            .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
            .OrderBy(rt => rt.CreatedAt)
            .ToListAsync();

        if (activeTokens.Count >= MaxActiveRefreshTokensPerUser)
        {
            // Remove oldest tokens to stay within limit
            var tokensToRemove = activeTokens.Take(activeTokens.Count - MaxActiveRefreshTokensPerUser + 1).ToList();
            _context.RefreshTokens.RemoveRange(tokensToRemove);
            _logger.LogInformation("Removed {Count} old tokens for user {UserId} to enforce limit", 
                tokensToRemove.Count, userId);
        }
    }

    /// <summary>
    /// Hash a token using SHA256 for secure storage
    /// </summary>
    private string HashToken(string token)
    {
        using var sha256 = SHA256.Create();
        var hashBytes = sha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(token));
        return Convert.ToBase64String(hashBytes);
    }

    /// <summary>
    /// Get all active refresh tokens for a user (for admin purposes)
    /// </summary>
    public async Task<OperationResult> GetUserActiveTokensAsync(long userId)
    {
        try
        {
            var activeTokens = await _context.RefreshTokens
                .Where(rt => rt.UserId == userId && rt.RevokedAt == null && rt.ExpiresAt > DateTime.UtcNow)
                .OrderByDescending(rt => rt.CreatedAt)
                .Select(rt => new
                {
                    rt.Id,
                    rt.CreatedAt,
                    rt.ExpiresAt,
                    rt.CreatedByIp
                })
                .ToListAsync();

            return OperationResult.Succeeded(payload: activeTokens);
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error getting active tokens for user {UserId}", userId);
            return OperationResult.Failed("خطا در دریافت توکن‌های فعال");
        }
    }
}

