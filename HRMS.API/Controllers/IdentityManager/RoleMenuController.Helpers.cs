using HR.Identity.Core.Entities;
using HRMS.API.Controllers.IdentityManager.Model;

namespace HRMS.API.Controllers.IdentityManager;

public partial class RoleMenuController
{
    private static void MarkCheckedPermissions(List<Permission> permissions, HashSet<string> checkedClaims)
    {
        foreach (var permission in permissions)
        {
            if (checkedClaims.Contains(permission.Name.ToLower()))
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

    private static bool TryFindConflictingRoleMenu(
        IEnumerable<RoleMenu> selectedRoleMenus,
        HashSet<string> conflictingClaims,
        IReadOnlyDictionary<string, Permission> permissionByName,
        out string? conflictingDisplayName)
    {
        foreach (var roleMenu in selectedRoleMenus)
        {
            if (string.IsNullOrEmpty(roleMenu.Claim))
            {
                continue;
            }

            if (!conflictingClaims.Contains(roleMenu.Claim.ToLower()))
            {
                continue;
            }

            conflictingDisplayName = permissionByName.TryGetValue(roleMenu.Claim, out var permission)
                ? permission.DisplayName
                : roleMenu.Claim;
            return true;
        }

        conflictingDisplayName = null;
        return false;
    }
}
