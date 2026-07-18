using Hr.SystemSetting.Infrastructure.Services;
using HR.BaseInfo.infrastructure.Services;
using HR.FormulaEngine.Infrastructure.Data;
using HR.Organisation.Infrastructure.Services;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;

namespace HR.FormulaEngine.Infrastructure
{
    public static class FormulaStartup
    {
        public static IServiceCollection AddFormula(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            services.AddDbContext<FormulaEngineContext>();
            services.AddScoped<DbContext, FormulaEngineContext>();


            return services;
        }
    }
}