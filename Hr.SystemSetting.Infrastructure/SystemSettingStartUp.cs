using Hr.SystemSetting.Infrastructure.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.SystemSetting.Infrastructure;

public static class SystemSettingStartUp
{
    public static IServiceCollection AddSystemSetting(this IServiceCollection services, Action<DbContextOptionsBuilder> options, List<Type> autoMapperProfileAssemblies = null, string ConnectionString = null)
    {
        services.AddDbContext<SystemSettingContext>(options);

        services.AddScoped<DbContext, SystemSettingContext>();

    

        return services;
    }
}
