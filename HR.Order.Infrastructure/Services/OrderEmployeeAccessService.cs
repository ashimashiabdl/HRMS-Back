using HR.Employee.Core.Entities;
using HR.Identity.Core.Entities;
using HR.Order.Core.Data;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using Hr.Employee.infrastructure.Data;
using HR.Identity.infrastructure.Data;
using HR.Order.Infrastructure.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.Order.Infrastructure.Services;

/// <summary>
/// سرویس بررسی دسترسی کاربر به کارمند برای صدور حکم
/// این سرویس منطق دسترسی را بر اساس حکم منع استخدام (InterdictOrder) یا محل پرداخت (PayLocation) چک می‌کند
/// </summary>
public class OrderEmployeeAccessService : IScopedServices
{
    private readonly OrderContext _orderContext;
    private readonly EmployeeContext _employeeContext;
    private readonly CustomIdentityContext _identityContext;
    private readonly UserResolverService _userResolverService;

    public OrderEmployeeAccessService(
        OrderContext orderContext,
        EmployeeContext employeeContext,
        CustomIdentityContext identityContext,
        UserResolverService userResolverService)
    {
        _orderContext = orderContext;
        _employeeContext = employeeContext;
        _identityContext = identityContext;
        _userResolverService = userResolverService;
    }

    /// <summary>
    /// بررسی دسترسی کاربر به کارمند برای صدور حکم
    /// </summary>
    /// <param name="currentUserId">شناسه کاربر جاری</param>
    /// <param name="employeeId">شناسه کارمند مورد نظر</param>
    /// <returns>نتیجه عملیات شامل موفقیت یا عدم موفقیت</returns>
    public OperationResult CheckAccess(long currentUserId, long employeeId)
    {
        try
        {
            // Admin users have full access
            if (_userResolverService.IsAdmin())
            {
                return OperationResult.Succeeded();
            }

            // مرحله اول: بررسی وجود حکم منع استخدام با StatusId = 9
            var hasInterdictOrderWithStatus9 = _orderContext.InterdictOrders
                .AsNoTracking()
                .Any(io => io.StatusId == 9 &&
                          io.RecruitOrder != null &&
                          io.RecruitOrder.EmployeeId == employeeId);

            if (hasInterdictOrderWithStatus9)
            {
                // بررسی دسترسی بر اساس RecruitOrder متناظر
                var recruitOrder = _orderContext.InterdictOrders
                    .AsNoTracking()
                    .Where(io => io.StatusId == 9 &&
                                io.RecruitOrder != null &&
                                io.RecruitOrder.EmployeeId == employeeId)
                    .Select(io => io.RecruitOrder)
                    .FirstOrDefault();

                if (recruitOrder != null)
                {
                    // بررسی اینکه آیا کاربر به PayLocation این RecruitOrder دسترسی دارد
                    var hasAccessToPayLocation = _identityContext.UserPayLocations
                        .AsNoTracking()
                        .Any(upl => upl.UserId == currentUserId &&
                                   upl.PayLocationId == recruitOrder.PayLocationId);

                    if (hasAccessToPayLocation)
                    {
                        return OperationResult.Succeeded();
                    }
                    else
                    {
                        return OperationResult.Failed("کاربر جاری به محل پرداخت کارمند منع استخدام دسترسی ندارد");
                    }
                }
            }

            // مرحله دوم: بررسی دسترسی بر اساس BaseOrganisationId کارمند
            var employee = _employeeContext.Employees
                .AsNoTracking()
                .Where(e => e.Id == employeeId)
                .Select(e => new { e.BaseOrganisationId })
                .FirstOrDefault();

            if (employee == null)
            {
                return OperationResult.Failed("کارمند مورد نظر یافت نشد");
            }

            if (!employee.BaseOrganisationId.HasValue)
            {
                return OperationResult.Failed("محل پرداخت پایه برای کارمند تعریف نشده است");
            }

            // بررسی اینکه آیا BaseOrganisationId در فهرست UserPayLocation های کاربر جاری وجود دارد
            var userPayLocations = _identityContext.UserPayLocations
                .AsNoTracking()
                .Where(upl => upl.UserId == currentUserId)
                .Select(upl => upl.PayLocationId)
                .ToList();

            if (userPayLocations.Contains(employee.BaseOrganisationId.Value))
            {
                return OperationResult.Succeeded();
            }
            else
            {
                return OperationResult.Failed("کاربر جاری به محل پرداخت پایه کارمند دسترسی ندارد");
            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در بررسی دسترسی: {ex.Message}");
        }
    }

    /// <summary>
    /// بررسی دسترسی کاربر به کارمند برای صدور حکم (نسخه async)
    /// </summary>
    /// <param name="currentUserId">شناسه کاربر جاری</param>
    /// <param name="employeeId">شناسه کارمند مورد نظر</param>
    /// <returns>نتیجه عملیات شامل موفقیت یا عدم موفقیت</returns>
    public async Task<OperationResult> CheckAccessAsync(long currentUserId, long employeeId)
    {
        try
        {
            // Admin users have full access
            if (_userResolverService.IsAdmin())
            {
                return OperationResult.Succeeded();
            }

            // مرحله اول: بررسی وجود حکم منع استخدام با StatusId = 9
            var hasInterdictOrderWithStatus9 = await _orderContext.InterdictOrders
                .AsNoTracking()
                .AnyAsync(io => io.StatusId == 9 &&
                               io.RecruitOrder != null &&
                               io.RecruitOrder.EmployeeId == employeeId);

            if (hasInterdictOrderWithStatus9)
            {
                // بررسی دسترسی بر اساس RecruitOrder متناظر
                var recruitOrder = await _orderContext.InterdictOrders
                    .AsNoTracking()
                    .Where(io => io.StatusId == 9 &&
                                io.RecruitOrder != null &&
                                io.RecruitOrder.EmployeeId == employeeId)
                    .Select(io => io.RecruitOrder)
                    .FirstOrDefaultAsync();

                if (recruitOrder != null)
                {
                    // بررسی اینکه آیا کاربر به PayLocation این RecruitOrder دسترسی دارد
                    var hasAccessToPayLocation = await _identityContext.UserPayLocations
                        .AsNoTracking()
                        .AnyAsync(upl => upl.UserId == currentUserId &&
                                        upl.PayLocationId == recruitOrder.PayLocationId);

                    if (hasAccessToPayLocation)
                    {
                        return OperationResult.Succeeded();
                    }
                    else
                    {
                        return OperationResult.Failed("کاربر جاری به محل پرداخت کارمند منع استخدام دسترسی ندارد");
                    }
                }
            }

            // مرحله دوم: بررسی دسترسی بر اساس BaseOrganisationId کارمند
            var employee = await _employeeContext.Employees
                .AsNoTracking()
                .Where(e => e.Id == employeeId)
                .Select(e => new { e.BaseOrganisationId })
                .FirstOrDefaultAsync();

            if (employee == null)
            {
                return OperationResult.Failed("کارمند مورد نظر یافت نشد");
            }

            if (!employee.BaseOrganisationId.HasValue)
            {
                return OperationResult.Failed("محل پرداخت پایه برای کارمند تعریف نشده است");
            }

            // بررسی اینکه آیا BaseOrganisationId در فهرست UserPayLocation های کاربر جاری وجود دارد
            var userPayLocations = await _identityContext.UserPayLocations
                .AsNoTracking()
                .Where(upl => upl.UserId == currentUserId)
                .Select(upl => upl.PayLocationId)
                .ToListAsync();

            if (userPayLocations.Contains(employee.BaseOrganisationId.Value))
            {
                return OperationResult.Succeeded();
            }
            else
            {
                return OperationResult.Failed("کاربر جاری به محل پرداخت پایه کارمند دسترسی ندارد");
            }
        }
        catch (Exception ex)
        {
            return OperationResult.Failed($"خطا در بررسی دسترسی: {ex.Message}");
        }
    }
}

