namespace HRMS.API.Cache;

/// <summary>
/// Shared HttpContext.Items keys used across JWT validation and HR authorization.
/// </summary>
public static class AuthorizationContextKeys
{
    /// <summary>
    /// Set to the validated user id (long) after JwtBearer confirms the security stamp for the current request.
    /// </summary>
    public const string StampValidatedUserId = "AuthorizeHR:StampValidatedUserId";
}
