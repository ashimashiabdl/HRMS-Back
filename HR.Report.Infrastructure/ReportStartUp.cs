using HR.Report.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Report.Infrastructure
{
    public static class ReportStartUp
    {
        public static IServiceCollection AddReport(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            services.AddDbContext<ReportContext>(options, ServiceLifetime.Scoped);
            services.AddScoped<DbContext, ReportContext>();

            return services;
        }
    }
}
