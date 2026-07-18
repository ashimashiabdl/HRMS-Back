using Hr.Employee.infrastructure.Services;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace HRMS.API.Cache;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class EmployeeAccessAttribute : Attribute, IAsyncActionFilter
{
    public string IdPropertyName { get; }

    public EmployeeAccessAttribute(string idPropertyName = "EmployeeId")
    {
        IdPropertyName = idPropertyName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            var userResolver = context.HttpContext.RequestServices.GetRequiredService<UserResolverService>();
            var employeeService = context.HttpContext.RequestServices.GetRequiredService<EmployeeService>();

            long currentUserId = userResolver.GetUserId();
            long employeeId = ResolveEmployeeId(context, IdPropertyName);

            if (employeeId > 0)
            {
                var hasAccess = employeeService.CheckAccess(currentUserId, employeeId);
                if (!hasAccess)
                {
                    context.Result = new ObjectResult(OperationResult.Failed("کاربر جاری به کارمند مورد نظر دسترسی ندارد"))
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return;
                }
            }
        }
        catch (Exception ex)
        {
            context.Result = new ObjectResult(OperationResult.Failed(ex.Message))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            return;
        }

        await next();
    }

    private static long ResolveEmployeeId(ActionExecutingContext context, string idPropertyName)
    {
        // 1) Try action arguments: look for property IdPropertyName on complex objects
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;

            var argType = arg.GetType();
            var prop = argType.GetProperty(idPropertyName, BindingFlags.Instance | BindingFlags.Public | BindingFlags.IgnoreCase);
            if (prop != null)
            {
                var value = prop.GetValue(arg);
                var coerced = CoerceToLong(value);
                if (coerced > 0) return coerced;
            }
        }

        // 2) Try route values
        if (context.RouteData.Values.TryGetValue(idPropertyName, out var routeVal))
        {
            if (routeVal != null && long.TryParse(routeVal.ToString(), out var l) && l > 0) return l;
        }

        // 3) Try query string
        if (context.HttpContext.Request.Query.TryGetValue(idPropertyName, out var qs) && qs.Count > 0)
        {
            if (long.TryParse(qs[0], out var l) && l > 0) return l;
        }

        return 0;
    }

    private static long CoerceToLong(object value)
    {
        if (value == null) return 0;
        if (value is long) return (long)value;
        if (value is int) return (int)value;
        if (value is short) return (short)value;
        if (value is byte) return (byte)value;
        if (value is long?) return ((long?)value) ?? 0;
        if (value is int?) return ((int?)value) ?? 0;
        if (value is short?) return ((short?)value) ?? 0;
        if (value is byte?) return ((byte?)value) ?? 0;
        if (value is string s && long.TryParse(s, out var p)) return p;
        return 0;
    }
}


