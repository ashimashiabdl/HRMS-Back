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

[Route("api/RoleMenu")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("منو های نقش")]
public partial class RoleMenuController : AppBaseController
{
    private readonly RoleMenuService _roleMenuService;
    private readonly PermissionsService _permissionsService;

    public RoleMenuController(
        RoleMenuService roleMenuService,
        PermissionsService permissionsService,
        ILogger<RoleMenuController> logger,
        IHttpContextAccessor accessor,
        IMapper mapper,
        IDapper dapper,
        UserResolverService userResolverService)
        : base(userResolverService, logger, accessor, mapper, dapper)
    {
        _roleMenuService = roleMenuService;
        _roleMenuService._currentUserDefaultOrganId = currentUserDefaultOrganId;
        _permissionsService = permissionsService;
    }

    [HttpGet, Route("GetAllForSelectedRole/{id}")]
    [CustomAccessKey(AccessKey: "GetAllForSelectedRole")]
    public IActionResult GetAllForSelectedRole(long id)
    {
        var allPermissions = _permissionsService.GetAll();

        var checkedClaims = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var claim in _roleMenuService.All()
            .AsNoTracking()
            .Where(i => i.RoleId == id && !string.IsNullOrEmpty(i.Claim))
            .Select(i => i.Claim))
        {
            checkedClaims.Add(claim.ToLower());
        }

        MarkCheckedPermissions(allPermissions, checkedClaims);

        var tree = TreeUtil.BuildTree(MapToNodes(allPermissions));
        return this.AppOk(tree);
    }

    [HttpPost("Post")]
    [CustomAccessKey(AccessKey: "Post")]
    public async Task<IActionResult> Post([FromBody] UserAccessDTO body)
    {
        var selectedRoleMenus = _roleMenuService.All()
            .AsNoTracking()
            .Where(i => i.RoleId == body.RoleId && !string.IsNullOrEmpty(i.Claim))
            .ToList();

        if (selectedRoleMenus.Any(c => c.Claim == body.claimType))
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

        var descendantClaims = PermissionAccessHierarchyHelper.GetDescendantClaims(targetPermission, childrenByParentId);
        var descendantClaimsSet = PermissionAccessHierarchyHelper.ToNormalizedPermissionNameSet(descendantClaims);
        if (TryFindConflictingRoleMenu(selectedRoleMenus, descendantClaimsSet, permissionByName, out var descendantWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"لطفا ابتدا دسترسی زیر مجموعه '{descendantWithAccess}' را بگیرید سپس اجازه بدهید که دسترسی اعطا بشود"));
        }

        var ancestorClaims = PermissionAccessHierarchyHelper.GetAncestorClaims(targetPermission, permissionById);
        var ancestorClaimsSet = PermissionAccessHierarchyHelper.ToNormalizedPermissionNameSet(ancestorClaims);
        if (TryFindConflictingRoleMenu(selectedRoleMenus, ancestorClaimsSet, permissionByName, out var ancestorWithAccess))
        {
            return this.AppOk(OperationResult.Failed(
                $"به علت داشتن دسترسی در سطح بالاتر '{ancestorWithAccess}'، دادن دسترسی در سطح پایین‌تر بی‌معنی است و مجاز نیست"));
        }

        _roleMenuService.Add(new RoleMenu
        {
            Claim = body.claimType,
            RoleId = body.RoleId!.Value,
            CreateDate = DateTime.Now,
            title = string.Empty,
        });
        await _roleMenuService._unitOfWork.Save();

        return this.AppOk(OperationResult.Succeeded());
    }

    [HttpPost("RevokeAccess")]
    [CustomAccessKey(AccessKey: "RevokeAccess")]
    public async Task<IActionResult> RevokeAccess([FromBody] UserAccessDTO body)
    {
        var matchingAccess = _roleMenuService.All()
            .AsNoTracking()
            .FirstOrDefault(i => i.RoleId == body.RoleId && i.Claim == body.claimType);

        if (matchingAccess == null)
        {
            return this.AppNotFound("دسترسی با مشخصات مورد نظر ارسالی یافت نشد");
        }

        _roleMenuService.DeleteRecord(matchingAccess.Id);
        await _roleMenuService._unitOfWork.Save();
        return this.AppOk(OperationResult.Succeeded("دسترسی منو مورد نظر حذف شد "));
    }
}
