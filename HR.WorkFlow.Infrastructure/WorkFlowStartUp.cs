
using HR.SharedKernel.Service;
using HR.WorkFlow.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System.Reflection;

namespace HR.WorkFlow.Infrastructure
{
    public static class WorkFlowStartUp
    {
        public static IServiceCollection AddWorkFlow(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            services.AddDbContext<WorkFlowContext>(options);
            services.AddScoped<DbContext, WorkFlowContext>();

            return services;
        }
    }
}
