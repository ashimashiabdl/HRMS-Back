namespace HRMS.API.Controllers.IdentityManager.Model;

public class Node
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }  // اگر null باشد یعنی نود ریشه است
    public HRMS.API.Controllers.IdentityManager.Model.Node? Parent { get; set; }  // اگر null باشد یعنی نود ریشه است
    public string Name { get; set; }
    public bool Disabled { get; set; }
    public string Key { get; set; }
    public bool Checked { get; set; }

    /// <summary>
    /// آیا برای این دسترسی رکورد مسیر منو (PermissionRoute) تعریف شده است.
    /// </summary>
    public bool HasRoute { get; set; }

    public List<Node> Children { get; set; } = new List<Node>();
}
public static class TreeUtil {
    public static List<Node> BuildTree(List<Node> flatList)
    {
        var lookup = flatList.ToDictionary(x => x.Id);
        List<Node> roots = [];

        foreach (var node in flatList)
        {
            if (node.ParentId.HasValue)
            {
                if (lookup.TryGetValue(node.ParentId.Value, out var parent))
                {
                    //node.Parent = parent;
                    parent.Children.Add(node);
                }
            }
            else
            {

                node.Name = "سامانه سرمایه انسانی";
                node.Disabled = true;
                roots.Add(node); // نودهای ریشه
            }
        }

        foreach (var root in roots)
        {
            root.Children = root.Children
                .OrderBy(c => c.Name, StringComparer.CurrentCulture)
                .ToList();
        }

        return roots;
    }

}