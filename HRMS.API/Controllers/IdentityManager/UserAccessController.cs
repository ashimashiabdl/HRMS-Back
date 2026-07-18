using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;
using System.Security.Claims;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserAccess")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("دسترسی های کاربران")]
public class UserAccessController(
    UserManager<AspNetUsers> userManager,
    ILogger<UserAccessController> logger,
    IHttpContextAccessor accessor,
    IDapper dapper,
    UserResolverService userResolverService,
    PermissionsService permissionsService,
    IdentityContext identityContext)
    : AppBaseController(userResolverService, logger, accessor, null, dapper)
{
    private readonly UserManager<AspNetUsers> _userManager = userManager;
    private readonly PermissionsService _permissionsService = permissionsService;
    private readonly IdentityContext _identityContext = identityContext;

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "create")]
    public async Task<IActionResult> Post([FromBody] UserAccessDTO body)
    {
        var user = await _userManager.FindByIdAsync(body.UserId.ToString());

        var existingClaims = await _userManager.GetClaimsAsync(user);
        var claimValue = body.claimValue.ToString();
        if (existingClaims.Any(c => c.Type == body.claimType && c.Value == claimValue))
        {
            return this.AppOk(OperationResult.Failed("این دسترسی قبلا داده شده است"));
        }

        var allPermissions = _permissionsService.GetAll();
        var permissionByName = allPermissions.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
        var permissionById = allPermissions.ToDictionary(p => p.Id);
        var childrenByParentId = PermissionAccessHierarchyHelper.BuildChildrenLookup(allPermissions);

        if (!permissionByName.TryGetValue(body.claimType, out var targetPermission))
        {
            return this.AppOk(OperationResult.Failed("دسترسی مورد نظر یافت نشد"));
        }

        var existingClaimTypes = await _identityContext.UserClaims
            .AsNoTracking()
            .Where(i => i.UserId == body.UserId)
            .Select(uc => uc.ClaimType.ToLower())
            .ToListAsync();

        var existingClaimTypeSet = existingClaimTypes.ToHashSet(StringComparer.OrdinalIgnoreCase);

        var descendantClaims = PermissionAccessHierarchyHelper.GetDescendantClaims(targetPermission, childrenByParentId);
        var descendantClaimsSet = PermissionAccessHierarchyHelper.ToNormalizedPermissionNameSet(descendantClaims);
        if (PermissionAccessHierarchyHelper.TryFindConflictingClaimType(existingClaimTypeSet, descendantClaimsSet, permissionByName, out var descendantWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"لطفا ابتدا دسترسی زیر مجموعه '{descendantWithAccess}' را بگیرید سپس اجازه بدهید که دسترسی اعطا بشود"));
        }

        var ancestorClaims = PermissionAccessHierarchyHelper.GetAncestorClaims(targetPermission, permissionById);
        var ancestorClaimsSet = PermissionAccessHierarchyHelper.ToNormalizedPermissionNameSet(ancestorClaims);
        if (PermissionAccessHierarchyHelper.TryFindConflictingClaimType(existingClaimTypeSet, ancestorClaimsSet, permissionByName, out var ancestorWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"به علت داشتن دسترسی در سطح بالاتر '{ancestorWithAccess}'، دادن دسترسی در سطح پایین‌تر بی‌معنی است و مجاز نیست"));
        }

        var result = await _userManager.AddClaimAsync(user, new Claim(body.claimType, claimValue));
        if (result.Succeeded)
        {
            await _userManager.UpdateSecurityStampAsync(user);
            return this.AppOk(OperationResult.Succeeded());
        }

        return this.AppOk(OperationResult.Failed(result.Errors.FirstOrDefault()?.Description ?? "خطا در ثبت دسترسی"));
    }

    [HttpPost("RevokeAccess")]
    [CustomAccessKey(AccessKey: "RevokeAccess")]
    public async Task<IActionResult> RevokeAccess([FromBody] UserAccessDTO body)
    {
        var user = await _userManager.FindByIdAsync(body.UserId.ToString());
        var existingClaims = await _userManager.GetClaimsAsync(user);

        var revokeValue = (!body.claimValue).ToString();
        var claimExists = existingClaims.Any(c =>
            string.Equals(c.Type, body.claimType, StringComparison.OrdinalIgnoreCase) &&
            c.Value == revokeValue);

        if (!claimExists)
        {
            return this.AppOk(OperationResult.Failed("دسترسی مورد نظر یافت نشد"));
        }

        var result = await _userManager.RemoveClaimAsync(user, new Claim(body.claimType, revokeValue));
        if (result.Succeeded)
        {
            await _userManager.UpdateSecurityStampAsync(user);
            return this.AppOk(OperationResult.Succeeded());
        }

        return this.AppOk(OperationResult.Failed(result.Errors.First().Description));
    }

}
