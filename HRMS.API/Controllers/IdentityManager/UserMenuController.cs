using AutoMapper;
using HR.Identity.Core.DTOs;
using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.DTOs;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Controllers.IdentityManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/UserMenu")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("منو های کاربر")]
public class UserMenuController : AppBaseController
{
    private readonly UserMenuService _userMenuService;
    private readonly PermissionsService _permissionsService;

    public UserMenuController(
        UserMenuService userMenuService,
        PermissionsService permissionsService,
        ILogger<UserMenuController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _userMenuService = userMenuService;
        _userMenuService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _permissionsService = permissionsService;
    }

    [HttpGet, Route("GetAllForSelectedUser/{id}")]
    [CustomAccessKey(AccessKey: "GetAllForSelectedUser")]
    public IActionResult GetAllForSelectedUser(long id)
    {
        var allPermissions = _permissionsService.GetAll();

        var checkedClaims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var claim in _userMenuService.All()
            .AsNoTracking()
            .Where(i => i.UserId == id && !string.IsNullOrEmpty(i.Claim))
            .Select(i => i.Claim))
        {
            checkedClaims.Add(claim.Trim());
        }

        MarkCheckedPermissions(allPermissions, checkedClaims);

        var tree = TreeUtil.BuildTree(MapToNodes(allPermissions));
        return this.AppOk(tree);
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "Post")]
    public async Task<IActionResult> Post([FromBody] UserAccessDTO body)
    {
        var validationError = ValidateAccessRequest(body);
        if (validationError != null)
        {
            return validationError;
        }

        var claimTypeNormalized = body!.claimType.Trim();

        var selectedUserMenus = _userMenuService.All()
            .AsNoTracking()
            .Where(i => i.UserId == body.UserId && !string.IsNullOrEmpty(i.Claim))
            .ToList();

        var selectedClaimSet = BuildNormalizedClaimSet(selectedUserMenus);
        if (selectedClaimSet.Contains(NormalizeClaim(claimTypeNormalized)))
        {
            return this.AppOk(OperationResult.Failed("این دسترسی قبلا برای این کاربر ثبت شده است"));
        }

        var allPermissions = _permissionsService.GetAll();
        var permissionByName = allPermissions.ToDictionary(p => p.Name, p => p, StringComparer.OrdinalIgnoreCase);
        var permissionById = allPermissions.ToDictionary(p => p.Id);
        var childrenByParentId = BuildChildrenLookup(allPermissions);

        if (!permissionByName.TryGetValue(claimTypeNormalized, out var targetPermission))
        {
            return this.AppOk(OperationResult.Failed("دسترسی مورد نظر یافت نشد"));
        }

        var descendantClaims = GetDescendantClaims(targetPermission, childrenByParentId);
        var descendantClaimsSet = ToNormalizedPermissionNameSet(descendantClaims);
        if (TryFindConflictingClaim(selectedUserMenus, descendantClaimsSet, permissionByName, out var descendantWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"لطفا ابتدا دسترسی زیر مجموعه '{descendantWithAccess}' را بگیرید سپس اجازه بدهید که دسترسی اعطا بشود"));
        }

        var ancestorClaims = GetAncestorClaims(targetPermission, permissionById);
        var ancestorClaimsSet = ToNormalizedPermissionNameSet(ancestorClaims);
        if (TryFindConflictingClaim(selectedUserMenus, ancestorClaimsSet, permissionByName, out var ancestorWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"به علت داشتن دسترسی در سطح بالاتر '{ancestorWithAccess}'، دادن دسترسی در سطح پایین‌تر بی‌معنی است و مجاز نیست"));
        }

        _userMenuService.Add(new UserMenu
        {
            Claim = claimTypeNormalized,
            UserId = body.UserId,
            CreateDate = DateTime.Now,
            title = string.Empty,
        });
        await _userMenuService._unitOfWork.Save();

        return this.AppOk(OperationResult.Succeeded());
    }

    [HttpPost("RevokeAccess")]
    [CustomAccessKey(AccessKey: "RevokeAccess")]
    public async Task<IActionResult> RevokeAccess([FromBody] UserAccessDTO body)
    {
        var validationError = ValidateAccessRequest(body);
        if (validationError != null)
        {
            return validationError;
        }

        var claimTypeNormalized = body!.claimType.Trim();

        var matchingAccess = _userMenuService.All()
            .AsNoTracking()
            .Where(i => i.UserId == body.UserId && !string.IsNullOrEmpty(i.Claim))
            .AsEnumerable()
            .FirstOrDefault(c => c.Claim != null &&
                string.Equals(c.Claim.Trim(), claimTypeNormalized, StringComparison.OrdinalIgnoreCase));

        if (matchingAccess == null)
        {
            return this.AppNotFound("دسترسی با مشخصات مورد نظر ارسالی یافت نشد");
        }

        _userMenuService.DeleteRecord(matchingAccess.Id);
        await _userMenuService._unitOfWork.Save();
        return this.AppOk(OperationResult.Succeeded("دسترسی منو مورد نظر حذف شد "));
    }

    private IActionResult? ValidateAccessRequest(UserAccessDTO? body)
    {
        if (body == null)
        {
            return this.AppOk(OperationResult.Failed("اطلاعات ارسالی معتبر نیست"));
        }

        if (body.UserId <= 0)
        {
            return this.AppOk(OperationResult.Failed("شناسه کاربر معتبر نیست"));
        }

        if (string.IsNullOrWhiteSpace(body.claimType))
        {
            return this.AppOk(OperationResult.Failed("نوع دسترسی نمی‌تواند خالی باشد"));
        }

        return null;
    }

    private static void MarkCheckedPermissions(List<Permission> permissions, HashSet<string> checkedClaims)
    {
        foreach (var permission in permissions)
        {
            if (checkedClaims.Contains(permission.Name))
            {
                permission.Checked = true;
            }
        }
    }

    private static List<Node> MapToNodes(IReadOnlyList<Permission> permissions)
    {
        var nodes = new List<Node>(permissions.Count);
        foreach (var permission in permissions)
        {
            nodes.Add(new Node
            {
                Id = permission.Id,
                Name = permission.DisplayName,
                Key = permission.Name,
                ParentId = permission.ParentId,
                Checked = permission.Checked
            });
        }

        return nodes;
    }

    private static HashSet<string> BuildNormalizedClaimSet(IEnumerable<UserMenu> userMenus)
    {
        var claims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var menu in userMenus)
        {
            if (!string.IsNullOrEmpty(menu.Claim))
            {
                claims.Add(NormalizeClaim(menu.Claim));
            }
        }

        return claims;
    }

    private static HashSet<string> ToNormalizedPermissionNameSet(IEnumerable<Permission> permissions) =>
        permissions.Select(p => NormalizeClaim(p.Name)).ToHashSet(StringComparer.OrdinalIgnoreCase);

    private static string NormalizeClaim(string claim) => claim.Trim().ToLower();

    private static Dictionary<Guid, List<Permission>> BuildChildrenLookup(IEnumerable<Permission> allPermissions)
    {
        var lookup = new Dictionary<Guid, List<Permission>>();
        foreach (var permission in allPermissions)
        {
            if (!permission.ParentId.HasValue)
            {
                continue;
            }

            if (!lookup.TryGetValue(permission.ParentId.Value, out var children))
            {
                children = [];
                lookup[permission.ParentId.Value] = children;
            }

            children.Add(permission);
        }

        return lookup;
    }

    private static List<Permission> GetDescendantClaims(
        Permission permission,
        IReadOnlyDictionary<Guid, List<Permission>> childrenByParentId)
    {
        var descendants = new List<Permission>();
        if (!childrenByParentId.TryGetValue(permission.Id, out var directChildren))
        {
            return descendants;
        }

        var stack = new Stack<Permission>(directChildren);
        while (stack.Count > 0)
        {
            var current = stack.Pop();
            descendants.Add(current);

            if (childrenByParentId.TryGetValue(current.Id, out var children))
            {
                foreach (var child in children)
                {
                    stack.Push(child);
                }
            }
        }

        return descendants;
    }

    private static List<Permission> GetAncestorClaims(
        Permission permission,
        IReadOnlyDictionary<Guid, Permission> permissionById)
    {
        var ancestors = new List<Permission>();
        var current = permission;

        while (current.ParentId is Guid parentId && permissionById.TryGetValue(parentId, out var parent))
        {
            ancestors.Add(parent);
            current = parent;
        }

        return ancestors;
    }

    private static bool TryFindConflictingClaim(
        IEnumerable<UserMenu> selectedUserMenus,
        HashSet<string> conflictingClaims,
        IReadOnlyDictionary<string, Permission> permissionByName,
        out string? conflictingDisplayName)
    {
        foreach (var userMenu in selectedUserMenus)
        {
            if (string.IsNullOrEmpty(userMenu.Claim))
            {
                continue;
            }

            var normalizedClaim = NormalizeClaim(userMenu.Claim);
            if (!conflictingClaims.Contains(normalizedClaim))
            {
                continue;
            }

            conflictingDisplayName = permissionByName.TryGetValue(userMenu.Claim.Trim(), out var permission)
                ? permission.DisplayName
                : userMenu.Claim;
            return true;
        }

        conflictingDisplayName = null;
        return false;
    }
}
