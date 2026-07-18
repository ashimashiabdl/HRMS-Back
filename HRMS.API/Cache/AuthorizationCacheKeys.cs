namespace HRMS.API.Cache;

internal static class AuthorizationCacheKeys
{
    internal const string PermissionGraph = "AuthorizeHR:PermissionGraph";
    internal const string UserAccessPrefix = "AuthorizeHR:UserAccess:";

    internal static string UserAccess(long userId, string securityStamp)
        => $"{UserAccessPrefix}{userId}:{securityStamp}";
}
