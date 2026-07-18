using HR.Identity.Core.Entities;

namespace HRMS.API.Controllers.IdentityManager;

internal static class PermissionAccessHierarchyHelper
{
    public static Dictionary<Guid, List<Permission>> BuildChildrenLookup(IEnumerable<Permission> allPermissions)
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

    public static List<Permission> GetDescendantClaims(
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

    public static List<Permission> GetAncestorClaims(
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

    public static HashSet<string> ToNormalizedPermissionNameSet(IEnumerable<Permission> permissions) =>
        permissions.Select(p => p.Name.ToLower()).ToHashSet(StringComparer.OrdinalIgnoreCase);

    public static bool TryFindConflictingClaimType(
        HashSet<string> existingClaimTypes,
        HashSet<string> conflictingClaims,
        IReadOnlyDictionary<string, Permission> permissionByName,
        out string? conflictingDisplayName)
    {
        foreach (var claimType in existingClaimTypes)
        {
            if (!conflictingClaims.Contains(claimType))
            {
                continue;
            }

            conflictingDisplayName = permissionByName.TryGetValue(claimType, out var permission)
                ? permission.DisplayName
                : claimType;
            return true;
        }

        conflictingDisplayName = null;
        return false;
    }
}
