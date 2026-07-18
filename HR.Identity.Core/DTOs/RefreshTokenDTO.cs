using System;

namespace HR.Identity.Core.DTOs;

/// <summary>
/// DTO for refresh token operations
/// </summary>
public class RefreshTokenDTO
{
    public long Id { get; set; }
    public string Token { get; set; }
    public long UserId { get; set; }
    public DateTime CreatedAt { get; set; }
    public DateTime ExpiresAt { get; set; }
    public DateTime? RevokedAt { get; set; }
    public string? CreatedByIp { get; set; }
    public string? RevokedByIp { get; set; }
    public string? ReplacedByToken { get; set; }
    public string? RevocationReason { get; set; }
    public bool IsActive { get; set; }
    public bool IsExpired { get; set; }
}

/// <summary>
/// Request DTO for refreshing access token
/// </summary>
public class RefreshTokenRequestDTO
{
    /// <summary>
    /// The refresh token (sent as HttpOnly cookie or in body)
    /// </summary>
    public string? RefreshToken { get; set; }
}

/// <summary>
/// Response DTO after successful token refresh
/// </summary>
public class RefreshTokenResponseDTO
{
    public string AccessToken { get; set; }
    public string RefreshToken { get; set; }
    public DateTime AccessTokenExpiresAt { get; set; }
    public DateTime RefreshTokenExpiresAt { get; set; }
    public string UserFullPersianName { get; set; }
}

/// <summary>
/// Request DTO for revoking a refresh token
/// </summary>
public class RevokeTokenRequestDTO
{
    /// <summary>
    /// The refresh token to revoke (optional if sent as cookie)
    /// </summary>
    public string? RefreshToken { get; set; }
}

