
using HR.BaseInfo.Core.Interfaces;
using HR.SharedKernel.Service;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using static System.Runtime.InteropServices.JavaScript.JSType;
using HR.BaseInfo.infrastructure.Data;
using HR.SharedKernel.Data;
using System.Data.Entity;

namespace HR.BaseInfo.infrastructure
{
    public static class BaseInfoStartup
    {
        public static IServiceCollection AddBaseInfo(this IServiceCollection services, Action<DbContextOptionsBuilder> options, ConfigurationManager Configuration)
        {
            services.AddDbContext<BaseInfoContext>();


            services.AddScoped<IAppDbContext, BaseInfoContext>();
         //   services.AddScoped<IAppDbContext, HrDbContext>(); // یا هرکدوم لازمه

            return services;
        }
    }
}
