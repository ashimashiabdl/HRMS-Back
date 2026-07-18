using HR.Identity.Core.Interfaces;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;

using HR.Identity.infrastructure.Data;

namespace HR.Identity.infrastructure;

public static class HRIdentityStartup
{
    public static IServiceCollection AddHRIdentity(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
    {
        services.AddDbContext<IdentityContext>(options =>
    options.UseSqlServer(Configuration.GetConnectionString("HRMSConnection")));
        services.AddScoped<DbContext, IdentityContext>();



        services.AddDbContext<CustomIdentityContext>(options =>
   options.UseSqlServer(Configuration.GetConnectionString("HRMSConnection")));
        services.AddScoped<DbContext, CustomIdentityContext>();
        
        return services;
    }
}
