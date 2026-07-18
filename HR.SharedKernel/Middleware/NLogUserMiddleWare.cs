using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.Logging;

namespace HR.SharedKernel.Middleware;

public class NLogUserMiddleWare(RequestDelegate next, ILogger<NLogUserMiddleWare> logger)
{
    public const string UserNameItemKey = "UserName";

    public async Task InvokeAsync(HttpContext context)
    {
        context.Items[UserNameItemKey] = ResolveUserName(context);
        await next(context);
    }

    private string ResolveUserName(HttpContext context)
    {
        try
        {
            var user = context.User;
            if (user == null)
            {
                return "Anonymous";
            }

            var fullNameClaim = user.Claims.FirstOrDefault(claim =>
                string.Equals(claim.Type, "fullname", StringComparison.OrdinalIgnoreCase));
            if (fullNameClaim != null && !string.IsNullOrWhiteSpace(fullNameClaim.Value))
            {
                return fullNameClaim.Value;
            }

            if (user.Identity?.IsAuthenticated == true)
            {
                return !string.IsNullOrWhiteSpace(user.Identity.Name)
                    ? user.Identity.Name
                    : "Authenticated";
            }

            return "Anonymous";
        }
        catch (Exception ex)
        {
            logger.LogWarning(ex, "Failed to resolve user name for NLog context.");
            return "Anonymous";
        }
    }
}
