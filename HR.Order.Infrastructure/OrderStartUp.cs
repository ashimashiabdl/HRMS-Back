using Hr.Employee.infrastructure.Services;
using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.infrastructure.Services;
using HR.Order.Infrastructure.Data;
using HR.Order.Infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HR.Order.Infrastructure;

public static class OrderStartUp
{
    public static IServiceCollection AddOrderInfo(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
    {
       
        services.AddDbContext<OrderContext>(options);
        services.AddScoped<DbContext, OrderContext>();

        // Register OrderEmployeeAccessService for access checking during order issuance
        services.AddScoped<OrderEmployeeAccessService>();

        return services;
    }
}
