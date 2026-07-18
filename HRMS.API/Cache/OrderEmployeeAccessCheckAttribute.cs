using HR.Order.Infrastructure.Services;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Reflection;

namespace HRMS.API.Cache;

/// <summary>
/// Attribute برای بررسی دسترسی کاربر به کارمند هنگام صدور احکام
/// این Attribute مشابه EmployeeAccessCheckAttribute عمل می‌کند اما با منطق خاص صدور حکم
/// </summary>
[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class)]
public sealed class OrderEmployeeAccessCheckAttribute : Attribute, IAsyncActionFilter
{
    /// <summary>
    /// نام property که شامل EmployeeId است (پیش‌فرض: "EmployeeId")
    /// </summary>
    public string EmployeeIdPropertyName { get; }

    /// <summary>
    /// ایجاد یک instance از OrderEmployeeAccessCheckAttribute
    /// </summary>
    /// <param name="employeeIdPropertyName">نام property که شامل EmployeeId است</param>
    public OrderEmployeeAccessCheckAttribute(string employeeIdPropertyName = "EmployeeId")
    {
        EmployeeIdPropertyName = employeeIdPropertyName;
    }

    public async Task OnActionExecutionAsync(ActionExecutingContext context, ActionExecutionDelegate next)
    {
        try
        {
            // دریافت سرویس‌های مورد نیاز از DI
            var userResolver = context.HttpContext.RequestServices.GetRequiredService<UserResolverService>();
            var orderEmployeeAccessService = context.HttpContext.RequestServices.GetRequiredService<OrderEmployeeAccessService>();

            long currentUserId = userResolver.GetUserId();
            long employeeId = ResolveEmployeeId(context, EmployeeIdPropertyName);

            if (employeeId > 0)
            {
                // بررسی دسترسی با استفاده از OrderEmployeeAccessService
                var accessResult = await orderEmployeeAccessService.CheckAccessAsync(currentUserId, employeeId);
                
                if (!accessResult.Success)
                {
                    context.Result = new ObjectResult(accessResult)
                    {
                        StatusCode = StatusCodes.Status400BadRequest
                    };
                    return;
                }
            }
            else
            {
                // اگر EmployeeId معتبر نیست، خطا برگردانیم
                context.Result = new ObjectResult(OperationResult.Failed("شناسه کارمند معتبر نیست"))
                {
                    StatusCode = StatusCodes.Status400BadRequest
                };
                return;
            }
        }
        catch (Exception ex)
        {
            context.Result = new ObjectResult(OperationResult.Failed($"خطا در بررسی دسترسی: {ex.Message}"))
            {
                StatusCode = StatusCodes.Status400BadRequest
            };
            return;
        }

        // در صورت موفقیت، به اکشن بعدی بروید
        await next();
    }

    /// <summary>
    /// استخراج EmployeeId از پارامترهای اکشن
    /// </summary>
    /// <param name="context">Context اکشن</param>
    /// <param name="propertyName">نام property که شامل EmployeeId است</param>
    /// <returns>مقدار EmployeeId یا 0 در صورت عدم یافتن</returns>
    private static long ResolveEmployeeId(ActionExecutingContext context, string propertyName)
    {
        if (context.ActionArguments == null || !context.ActionArguments.Any())
        {
            return 0;
        }

        // 1) ابتدا به دنبال پارامتر با نام دقیق (EmployeeId) بگردیم
        if (context.ActionArguments.TryGetValue(propertyName, out var directValue))
        {
            return CoerceToLong(directValue);
        }

        // 2) اگر پیدا نشد، در object های پارامتر جستجو کنیم
        foreach (var arg in context.ActionArguments.Values)
        {
            if (arg == null) continue;

            // بررسی اینکه آیا این object یک property با نام مشخص شده دارد
            var argType = arg.GetType();
            var propInfo = argType.GetProperty(propertyName, BindingFlags.Public | BindingFlags.Instance | BindingFlags.IgnoreCase);
            
            if (propInfo != null)
            {
                var value = propInfo.GetValue(arg);
                var employeeId = CoerceToLong(value);
                if (employeeId > 0)
                {
                    return employeeId;
                }
            }
        }

        return 0;
    }

    /// <summary>
    /// تبدیل مقدار به long
    /// </summary>
    /// <param name="value">مقدار ورودی</param>
    /// <returns>مقدار long یا 0</returns>
    private static long CoerceToLong(object? value)
    {
        if (value == null) return 0;

        if (value is long longVal)
        {
            return longVal;
        }

        if (value is int intVal)
        {
            return intVal;
        }

        if (value is string strVal && long.TryParse(strVal, out long parsed))
        {
            return parsed;
        }

        // تلاش برای تبدیل انواع دیگر
        try
        {
            return Convert.ToInt64(value);
        }
        catch
        {
            return 0;
        }
    }
}

