namespace HRMS.API.Cache;

public interface IAuthorizationCacheService
{
    Task<long[]> GetUserRolesAsync(long userId);
}
