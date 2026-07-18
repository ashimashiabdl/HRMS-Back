using HR.Organisation.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace HR.Organisation.Infrastructure
{
    public static class OrganisationStartup
    {
        public static IServiceCollection AddOrganisation(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            //services.AddDbContext<OrganisationContext>();
            //services.AddScoped<DbContext, OrganisationContext>();
            services.AddDbContext<OrganisationContext>(ServiceLifetime.Transient);
            return services;
        }
    }
}
