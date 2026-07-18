using Microsoft.AspNetCore.Http;

namespace HRMS.API.Infrastructure.Security;

/// <summary>
/// Centralizes JWT / refresh-token HttpOnly cookie options so set and delete use identical attributes.
/// Mismatched Path / Secure / SameSite on delete leaves stale cookies in the browser.
/// </summary>
public static class AuthCookieHelper
{
    public const string JwtCookieName = "jwt";
    public const string RefreshTokenCookieName = "refreshToken";

    public static CookieOptions CreateAuthCookieOptions(HttpRequest request, DateTime expiresUtc) =>
        new()
        {
            Path = "/",
            HttpOnly = true,
            Secure = request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = expiresUtc
        };

    public static void AppendJwtCookie(HttpRequest request, HttpResponse response, string token, DateTime expiresUtc) =>
        response.Cookies.Append(JwtCookieName, token, CreateAuthCookieOptions(request, expiresUtc));

    public static void AppendRefreshTokenCookie(HttpRequest request, HttpResponse response, string refreshToken, DateTime expiresUtc) =>
        response.Cookies.Append(RefreshTokenCookieName, refreshToken, CreateAuthCookieOptions(request, expiresUtc));

    public static void ClearAuthCookies(HttpRequest request, HttpResponse response)
    {
        var deleteOptions = new CookieOptions
        {
            Path = "/",
            HttpOnly = true,
            Secure = request.IsHttps,
            SameSite = SameSiteMode.Strict,
            Expires = DateTime.UnixEpoch
        };

        response.Cookies.Delete(JwtCookieName, deleteOptions);
        response.Cookies.Delete(RefreshTokenCookieName, deleteOptions);
    }
}
