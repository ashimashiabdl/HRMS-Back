using HR.Identity.Core.Entities;
using HR.SharedKernel.API;
using HRMS.API.Controllers.IdentityManager.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace HRMS.API.Controllers.IdentityManager;

public partial class PermissionsController
{
    private HashSet<string> LoadRoleClaimKeys(long roleId)
    {
        return _identityContext.RoleClaims
            .AsNoTracking()
            .Where(i => i.RoleId == roleId)
            .Select(i => i.ClaimType)
            .AsEnumerable()
            .Select(NormalizePermissionKey)
            .ToHashSet(StringComparer.Ordinal);
    }

    private HashSet<string> LoadUserClaimKeys(long userId)
    {
        return _identityContext.UserClaims
            .AsNoTracking()
            .Where(i => i.UserId == userId)
            .Select(i => i.ClaimType)
            .AsEnumerable()
            .Select(NormalizePermissionKey)
            .ToHashSet(StringComparer.Ordinal);
    }

    private IActionResult BuildOrderedPermissionTreeResponse(
        List<Permission> permissions,
        HashSet<string> checkedClaims)
    {
        var routeDefinitions = _identityContext.PermissionRoutes.AsNoTracking().ToList();
        var priorityByClaim = BuildPriorityByClaimLookup(routeDefinitions);
        var routeClaims = BuildRouteClaimSet(routeDefinitions);

        var orderedPermissions = OrderPermissions(permissions, priorityByClaim);
        MarkCheckedPermissions(orderedPermissions, checkedClaims);

        return this.AppOk(TreeUtil.BuildTree(MapPermissionNodes(orderedPermissions, routeClaims)));
    }

    private static HashSet<string> BuildRouteClaimSet(IEnumerable<PermissionRoute> routes) =>
        routes
            .Where(r => !string.IsNullOrWhiteSpace(r.Claim))
            .Select(r => NormalizeRouteClaim(r.Claim))
            .ToHashSet(StringComparer.Ordinal);

    private static Dictionary<string, int> BuildPriorityByClaimLookup(IEnumerable<PermissionRoute> routes) =>
        routes
            .Where(r => !string.IsNullOrWhiteSpace(r.Claim))
            .GroupBy(r => NormalizeRouteClaim(r.Claim))
            .ToDictionary(g => g.Key, g => g.Min(r => r.Priority));

    private static List<Permission> OrderPermissions(
        List<Permission> permissions,
        IReadOnlyDictionary<string, int> priorityByClaim) =>
        permissions
            .OrderBy(p => priorityByClaim.TryGetValue(NormalizeRouteClaim(p.Name), out var priority)
                ? priority
                : int.MaxValue)
            .ThenBy(p => p.DisplayName)
            .ToList();

    private static void MarkCheckedPermissions(List<Permission> permissions, HashSet<string> checkedClaims)
    {
        foreach (var permission in permissions)
        {
            if (checkedClaims.Contains(NormalizePermissionKey(permission.Name)))
            {
                permission.Checked = true;
            }
        }
    }

    private static List<Node> MapPermissionNodes(
        IReadOnlyList<Permission> permissions,
        HashSet<string> routeClaims)
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
                Checked = permission.Checked,
                HasRoute = routeClaims.Contains(NormalizeRouteClaim(permission.Name))
            });
        }

        return nodes;
    }

    private static string NormalizePermissionKey(string? claim) =>
        (claim ?? string.Empty).ToLower();

    private static string NormalizeRouteClaim(string? claim) =>
        (claim ?? string.Empty).Trim().ToLower();
}
