using HR.Identity.Core.Entities;
using HR.Identity.infrastructure.Data;
using HR.Identity.infrastructure.Services;
using HR.SharedKernel.API;
using HR.SharedKernel.Attribute;
using HR.SharedKernel.Dapper;
using HR.SharedKernel.Service;
using HRMS.API.Cache;
using HRMS.API.Controllers.IdentityManager.Model;
using HRMS.API.Scanner;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System.ComponentModel;

namespace HRMS.API.Controllers.IdentityManager;

[Route("api/[controller]")]
[ControllerGroup("IdentityManager", "احراز هویت")]
[DisplayName("کلید های دسترسی")]
public partial class PermissionsController(
    ILogger<PermissionsController> logger,
    IHttpContextAccessor accessor,
    PermissionsService permissionsService,
    IdentityContext identityContext,
    PermissionScanner permissionScanner,
    IDapper dapper,
    UserResolverService userResolverService)
    : AppBaseController(userResolverService, logger, accessor, null, dapper)
{
    private readonly PermissionsService _permissionsService = permissionsService;
    private readonly PermissionScanner _permissionScanner = permissionScanner;
    private readonly IdentityContext _identityContext = identityContext;

    [HttpGet, Route("InitializeDefaultPermissions")]
    [CustomAccessKey(AccessKey: "Initialize")]
    public async Task<IActionResult> InitializeDefaultPermissions()
    {
        var initialized = await _permissionScanner.InitializeDefaultPermissions();
        return this.AppOk(initialized);
    }

    [HttpGet, Route("GetAll")]
    [CustomAccessKey(AccessKey: "GetAll")]
    public IActionResult GetAll()
    {
        var permissions = _permissionsService.GetAll();
        var routeClaims = BuildRouteClaimSet(_identityContext.PermissionRoutes.AsNoTracking());
        return this.AppOk(TreeUtil.BuildTree(MapPermissionNodes(permissions, routeClaims)));
    }

    [HttpGet, Route("GetAllForSelectedRole/{id}")]
    [CustomAccessKey(AccessKey: "GetAllForSelectedRole")]
    public IActionResult GetAllForSelectedRole(long id)
    {
        var checkedClaims = LoadRoleClaimKeys(id);
        return BuildOrderedPermissionTreeResponse(_permissionsService.GetAll(), checkedClaims);
    }

    [HttpGet, Route("GetAllForSelectedUser/{id}")]
    [CustomAccessKey(AccessKey: "GetAllForSelectedUser")]
    public IActionResult GetAllForSelectedUser(long id)
    {
        var checkedClaims = LoadUserClaimKeys(id);
        return BuildOrderedPermissionTreeResponse(_permissionsService.GetAll(), checkedClaims);
    }
}
