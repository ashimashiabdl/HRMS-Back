using HR.BaseInfo.infrastructure.Services;
using HR.Organisation.Infrastructure.Services;
using Hr.SystemSetting.Infrastructure.Services;
using HR.Payroll.Infrastructure.Data;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;

using Hr.Employee.infrastructure.Services;
using HR.SharedKernel.Dapper;
using HR.Order.Infrastructure.Services;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;

namespace HR.Payroll.Infrastructure
{
    public static class PayRollStartup
    {
        public static IServiceCollection AddPayRoll(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            services.AddDbContext<PayrollContext>(ServiceLifetime.Scoped);
            services.AddScoped<DbContext, PayrollContext>();   
            
            
            services.AddDbContext<IdentityContext>(ServiceLifetime.Scoped);
            services.AddScoped<DbContext, IdentityContext>();


            var type = typeof(IScopedServices);
            var basservtypes = typeof(EmployeeTypeService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in basservtypes)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }

            var types = typeof(OrganisationEmployeeTypeOrderTypeCheckService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in types)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }   
            
            
            
            var UserCostCenterService = typeof(UserCostCenterService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in UserCostCenterService)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }
            var Formulatypes = typeof(FormulaEngine.Infrastructure.Services.FormulaService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in Formulatypes)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }

            var Organizationtypes = typeof(OrganizationJobService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in Organizationtypes)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }

            var employeeServices = typeof(EmployeeService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in employeeServices)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }

                        var OrderServices = typeof(OrderService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in OrderServices)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }




            var WageItemService = typeof(HR.BaseInfo.infrastructure.Services.WageItemService).GetTypeInfo().Assembly.GetTypes()

                .Where(type.IsAssignableFrom);

            foreach (var item in WageItemService)
            {
                if (item.Name != "IScopedServices")
                {
                    services.AddScoped(item);
                }
            }
            return services;
        }
    }
}
