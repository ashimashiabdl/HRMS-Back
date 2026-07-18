using Hr.Employee.infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Employee.infrastructure
{
    public static class EmployeeStartUp
    {
        public static IServiceCollection AddEmployee(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            //    services.AddDbContext<EmployeeContext>(options);

            services.AddDbContext<EmployeeContext>();
            services.AddScoped<DbContext, EmployeeContext>();

            return services;
        }
    }
}
