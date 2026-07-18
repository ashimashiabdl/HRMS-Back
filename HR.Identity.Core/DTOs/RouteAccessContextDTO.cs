namespace HR.Identity.Core.DTOs;

public class RouteAccessContextDTO
{
    public List<RouteClaimEntryDTO> Routes { get; set; } = [];
    public List<string> GrantedClaims { get; set; } = [];
    public List<PermissionGraphNodeDTO> PermissionNodes { get; set; } = [];
    public bool IsAdmin { get; set; }
}

public class PermissionGraphNodeDTO
{
    public Guid Id { get; set; }
    public Guid? ParentId { get; set; }
    public string Key { get; set; } = string.Empty;
}

public class RouteClaimEntryDTO
{
    public string Route { get; set; } = string.Empty;
    public string Claim { get; set; } = string.Empty;
}
