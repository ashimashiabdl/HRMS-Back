using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.infrastructure.Data;
using HR.SharedKernel;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel;
using System.Collections.Generic;
using System.Linq;


namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserRole")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("نقش کاربران سیستم")]
public class UserRoleController(IdentityContext IdentityContext, UserManager<HR.Identity.Core.Entities.AspNetUsers> userManager, RoleManager<HR.Identity.Core.Entities.AspNetRoles> roleManager, IMapper Mapper, ILogger<UserRoleController> logger, IHttpContextAccessor accessor, IDapper dapper, UserResolverService UserResolverService) : AppBaseController(UserResolverService, logger, accessor, null, dapper)
{
    IMapper _mapper = Mapper;
    private IdentityContext _context = IdentityContext;
    private readonly UserManager<HR.Identity.Core.Entities.AspNetUsers> _userManager = userManager;
    private readonly RoleManager<HR.Identity.Core.Entities.AspNetRoles> _roleManager = roleManager;

    [HttpGet, Route("Get/{id}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult Get(int id)
    {
        return this.AppOk(1);
    }
    [HttpGet, Route("GetAsKeyValuePairByUserId/{userId}")]
    [CustomAccessKey(AccessKey: "view")]
    public async Task<IActionResult> GetAsKeyValuePairByUserId(long userId)
    {
        try
        {
            var user = await _userManager.FindByIdAsync(userId.ToString());
            if (user == null)
            {
                return this.AppBadRequest(OperationResult.Failed("کاربر یافت نشد"));
            }

            var userRoles = await _userManager.GetRolesAsync(user);
            var result = new List<HR.SharedKernel.Data.KeyValuePair>();

            foreach (var roleName in userRoles)
            {
                var role = await _roleManager.FindByNameAsync(roleName);
                if (role != null)
                {
                    result.Add(new HR.SharedKernel.Data.KeyValuePair
                    {
                        key = role.Id,
                        value = role.PersianName ?? role.Name
                    });
                }
            }

            return this.AppOk(OperationResult.Succeeded(payload: result));
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "GetAsKeyValuePairByUserId failed. UserId={UserId}", userId);
            return this.AppBadRequest(OperationResult.Failed($"خطا در دریافت نقش‌های کاربر: {ex.Message}"));
        }
    }
    [HttpGet, Route("GetPagedData/{currentPage}/{pageSize}/{filter?}/{activeSortColumn?}/{Sortdirection?}/{IgnoreExpired?}")]
    [CustomAccessKey(AccessKey: "view")]
    public IActionResult GetPagedData(int currentPage = 0, int pageSize = 10, [FromQuery] string filter = "", [FromQuery] string activeSortColumn = "", [FromQuery] string Sortdirection = "", [FromQuery] bool IgnoreExpired = true, [FromQuery] long? UserId = null, [FromQuery] long? RoleId = null)
    {
        if (UserId == 0)
        {
            UserId = null;
        }
        if (RoleId == 0)
        {
            RoleId = null;
        }

        var paged = _context.UserRoles
               .Where(i => (i.UserId == UserId || UserId == null) && (i.RoleId == RoleId || RoleId == null))
         ;
        int rowCount = 0;
        return this.AppOk(OperationResult.Succeeded(payload: _mapper.Map<List<UserRoleDTO>>(PagerUtility<HR.Identity.Core.Entities.UserRole>.GetPagedData(paged, out rowCount, currentPage: currentPage, pageSize: pageSize, filter, activeSortColumn, Sortdirection))));

    }
    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] UserRoleDTO body)
    {
        var user = await _userManager.FindByIdAsync(body.UserId.ToString());
        var role = await _roleManager.FindByIdAsync(body.RoleId.ToString());

        var resp = await _userManager.AddToRoleAsync(user, role.Name);
        if (resp.Succeeded)
        {
            // Ensure SecurityStamp is not null before UpdateSecurityStampAsync (fixes production environment issue)
            if (string.IsNullOrEmpty(user.SecurityStamp))
            {
                await _userManager.UpdateSecurityStampAsync(user);
                // Reload SecurityStamp to ensure it's updated in tracked entity
                var reloadedUser = await _userManager.FindByIdAsync(body.UserId.ToString());
                if (reloadedUser != null)
                {
                    user.SecurityStamp = reloadedUser.SecurityStamp;
                }
            }
            
            // Invalidate existing tokens by rotating user's security stamp
            await _userManager.UpdateSecurityStampAsync(user);
            try
            {
                var ip = UserResolverService.GetIP();
                var actorUserName = UserResolverService.GetUser();
                var actorFullName = UserResolverService.fullname();
                _logger.LogInformation(
                    "اعطای نقش - موفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP}",
                    UserResolverService.GetUserId(),
                    actorUserName,
                    actorFullName,
                    user?.Id,
                    user?.UserName,
                    role?.Id,
                    role?.Name,
                    ip);
            }
            catch { }
            return this.AppOk(OperationResult.Succeeded());
        }
        else
        {
            try
            {
                var ip = UserResolverService.GetIP();
                var actorUserName = UserResolverService.GetUser();
                var actorFullName = UserResolverService.fullname();
                var error = resp.Errors.FirstOrDefault()?.Description;
                _logger.LogWarning(
                    "اعطای نقش - ناموفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP} - خطا:{Error}",
                    UserResolverService.GetUserId(),
                    actorUserName,
                    actorFullName,
                    user?.Id,
                    user?.UserName,
                    role?.Id,
                    role?.Name,
                    ip,
                    error);
            }
            catch { }
            return this.AppBadRequest(resp.Errors.First().Description);
        }

    }

    [HttpPut("Put")]
    [CustomAccessKey(AccessKey: "update")]
    public async Task<IActionResult> Put([FromBody] UserRoleDTO body)
    {
        if (body.UserId == null || body.RoleId == null)
        {
            return this.AppBadRequest(OperationResult.Failed("ورودی نامعتبر است"));
        }

        try
        {
            var user = await _userManager.FindByIdAsync(body.UserId.Value.ToString());
            if (user == null)
            {
                return this.AppBadRequest(OperationResult.Failed("کاربر یافت نشد"));
            }

            var role = await _roleManager.FindByIdAsync(body.RoleId.Value.ToString());
            if (role == null)
            {
                return this.AppBadRequest(OperationResult.Failed("نقش یافت نشد"));
            }

            // Check if user already has this role
            var userRoles = await _userManager.GetRolesAsync(user);
            if (userRoles.Contains(role.Name))
            {
                // User already has this role, no need to update
                return this.AppOk(OperationResult.Succeeded());
            }

            // Remove all existing roles first
            if (userRoles.Count > 0)
            {
                var removeResult = await _userManager.RemoveFromRolesAsync(user, userRoles);
                if (!removeResult.Succeeded)
                {
                    var error = removeResult.Errors.FirstOrDefault()?.Description;
                    return this.AppBadRequest(OperationResult.Failed($"خطا در حذف نقش‌های موجود: {error}"));
                }
            }

            // Add the new role
            var addResult = await _userManager.AddToRoleAsync(user, role.Name);
            if (addResult.Succeeded)
            {
                // Ensure SecurityStamp is not null before UpdateSecurityStampAsync (fixes production environment issue)
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    // Reload SecurityStamp to ensure it's updated in tracked entity
                    var reloadedUser = await _userManager.FindByIdAsync(body.UserId.Value.ToString());
                    if (reloadedUser != null)
                    {
                        user.SecurityStamp = reloadedUser.SecurityStamp;
                    }
                }
                
                // Invalidate existing tokens by rotating user's security stamp
                await _userManager.UpdateSecurityStampAsync(user);
                try
                {
                    var ip = UserResolverService.GetIP();
                    var actorUserName = UserResolverService.GetUser();
                    var actorFullName = UserResolverService.fullname();
                    _logger.LogInformation(
                        "ویرایش نقش - موفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP}",
                        UserResolverService.GetUserId(),
                        actorUserName,
                        actorFullName,
                        user?.Id,
                        user?.UserName,
                        role?.Id,
                        role?.Name,
                        ip);
                }
                catch { }
                return this.AppOk(OperationResult.Succeeded());
            }
            else
            {
                var error = addResult.Errors.FirstOrDefault()?.Description;
                try
                {
                    var ip = UserResolverService.GetIP();
                    var actorUserName = UserResolverService.GetUser();
                    var actorFullName = UserResolverService.fullname();
                    _logger.LogWarning(
                        "ویرایش نقش - ناموفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP} - خطا:{Error}",
                        UserResolverService.GetUserId(),
                        actorUserName,
                        actorFullName,
                        user?.Id,
                        user?.UserName,
                        role?.Id,
                        role?.Name,
                        ip,
                        error);
                }
                catch { }
                return this.AppBadRequest(OperationResult.Failed(error ?? "خطا در ویرایش نقش"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Put failed. UserId={UserId}, RoleId={RoleId}", body.UserId, body.RoleId);
            return this.AppBadRequest(OperationResult.Failed($"خطا در ویرایش نقش: {ex.Message}"));
        }
    }

    [HttpPost("AssignMultiple")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> AssignMultiple([FromBody] UserRoleDTO body)
    {
        if (body.UserId == null || body.RoleIds == null || body.RoleIds.Count == 0)
        {
            return this.AppBadRequest(OperationResult.Failed("ورودی نامعتبر است"));
        }

        try
        {
            var user = await _userManager.FindByIdAsync(body.UserId.Value.ToString());
            if (user == null)
            {
                return this.AppBadRequest(OperationResult.Failed("کاربر یافت نشد"));
            }

            var ip = UserResolverService.GetIP();
            var actorUserName = UserResolverService.GetUser();
            var actorFullName = UserResolverService.fullname();

            // Get existing roles for the user
            var existingRoles = await _userManager.GetRolesAsync(user);
            _logger.LogInformation(
                "UserRole AssignMultiple replace start. UserId={UserId}, existingRolesCount={ExistingCount}, newRolesCount={NewCount}",
                body.UserId,
                existingRoles.Count,
                body.RoleIds.Count);

            // Remove all existing roles
            if (existingRoles.Count > 0)
            {
                _logger.LogInformation("Removing existing roles. UserId={UserId}, roles={Roles}", body.UserId, string.Join(",", existingRoles));
                var removeResult = await _userManager.RemoveFromRolesAsync(user, existingRoles);
                if (!removeResult.Succeeded)
                {
                    var error = removeResult.Errors.FirstOrDefault()?.Description;
                    _logger.LogWarning(
                        "حذف نقش‌های موجود - ناموفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - IP:{IP} - خطا:{Error}",
                        UserResolverService.GetUserId(),
                        actorUserName,
                        actorFullName,
                        user?.Id,
                        user?.UserName,
                        ip,
                        error);
                    return this.AppBadRequest(OperationResult.Failed($"خطا در حذف نقش‌های موجود: {error}"));
                }
            }

            // Get role names for new roles
            var roleNames = new List<string>();
            foreach (var roleId in body.RoleIds.Distinct())
            {
                var role = await _roleManager.FindByIdAsync(roleId.ToString());
                if (role == null)
                {
                    _logger.LogWarning("Role not found. RoleId={RoleId}", roleId);
                    continue;
                }
                roleNames.Add(role.Name);
            }

            if (roleNames.Count == 0)
            {
                return this.AppBadRequest(OperationResult.Failed("هیچ نقش معتبری یافت نشد"));
            }

            // Add new roles
            _logger.LogInformation("Adding new roles. UserId={UserId}, roleIds={RoleIds}", body.UserId, string.Join(",", body.RoleIds.Distinct()));
            var addResult = await _userManager.AddToRolesAsync(user, roleNames);
            if (addResult.Succeeded)
            {
                // Ensure SecurityStamp is not null before UpdateSecurityStampAsync (fixes production environment issue)
                if (string.IsNullOrEmpty(user.SecurityStamp))
                {
                    await _userManager.UpdateSecurityStampAsync(user);
                    // Reload SecurityStamp to ensure it's updated in tracked entity
                    var reloadedUser = await _userManager.FindByIdAsync(body.UserId.Value.ToString());
                    if (reloadedUser != null)
                    {
                        user.SecurityStamp = reloadedUser.SecurityStamp;
                    }
                }
                
                // Invalidate existing tokens by rotating user's security stamp
                await _userManager.UpdateSecurityStampAsync(user);
                _logger.LogInformation(
                    "اعطای چند نقش - موفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش‌ها:{RoleIds} - IP:{IP}",
                    UserResolverService.GetUserId(),
                    actorUserName,
                    actorFullName,
                    user?.Id,
                    user?.UserName,
                    string.Join(",", body.RoleIds.Distinct()),
                    ip);
                return this.AppOk(OperationResult.Succeeded(payload: roleNames.Count));
            }
            else
            {
                var error = addResult.Errors.FirstOrDefault()?.Description;
                _logger.LogWarning(
                    "اعطای چند نقش - ناموفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش‌ها:{RoleIds} - IP:{IP} - خطا:{Error}",
                    UserResolverService.GetUserId(),
                    actorUserName,
                    actorFullName,
                    user?.Id,
                    user?.UserName,
                    string.Join(",", body.RoleIds.Distinct()),
                    ip,
                    error);
                return this.AppBadRequest(OperationResult.Failed(error ?? "خطا در اعطای نقش‌ها"));
            }
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "AssignMultiple failed. UserId={UserId}", body.UserId);
            return this.AppBadRequest(OperationResult.Failed($"خطا در اعطای نقش‌ها: {ex.Message}"));
        }
    }

    [HttpDelete("Delete/{roleId}/{userId}")]
    [CustomAccessKey(AccessKey: "delete")]
    public async Task<IActionResult> Delete(long roleId, long userId)
    {
        var selectedUser = await _context.Users.FindAsync(userId);
        var role = await _context.Roles.FindAsync(roleId);

        if (role == null)
        {
            return this.AppNotFound("نقش ارسالی یافت نشد");
        }

        var result = await _userManager.RemoveFromRoleAsync(selectedUser, role.Name);


        var ip = UserResolverService.GetIP();
        var actorUserName = UserResolverService.GetUser();
        var actorFullName = UserResolverService.fullname();
        if (result.Succeeded)
        {
            // Ensure SecurityStamp is not null before UpdateSecurityStampAsync (fixes production environment issue)
            if (string.IsNullOrEmpty(selectedUser.SecurityStamp))
            {
                await _userManager.UpdateSecurityStampAsync(selectedUser);
                // Reload SecurityStamp to ensure it's updated in tracked entity
                var reloadedUser = await _userManager.FindByIdAsync(userId.ToString());
                if (reloadedUser != null)
                {
                    selectedUser.SecurityStamp = reloadedUser.SecurityStamp;
                }
            }
            
            // Invalidate existing tokens by rotating user's security stamp
            await _userManager.UpdateSecurityStampAsync(selectedUser);
            _logger.LogInformation(
                "حذف نقش - موفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP}",
                UserResolverService.GetUserId(),
                actorUserName,
                actorFullName,
                selectedUser?.Id,
                selectedUser?.UserName,
                role?.Id,
                role?.Name,
                ip);
        }
        else
        {
            var error = result.Errors.FirstOrDefault()?.Description;
            _logger.LogWarning(
                "حذف نقش - ناموفق - انجام‌دهنده:{ActorId}/{ActorUserName} ({ActorFullName}) - کاربر:{UserId}/{UserName} - نقش:{RoleId}/{RoleName} - IP:{IP} - خطا:{Error}",
                UserResolverService.GetUserId(),
                actorUserName,
                actorFullName,
                selectedUser?.Id,
                selectedUser?.UserName,
                role?.Id,
                role?.Name,
                ip,
                error);
        }




        return this.AppOk(result);
    }
}



