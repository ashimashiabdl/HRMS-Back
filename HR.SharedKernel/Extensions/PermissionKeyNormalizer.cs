namespace HR.SharedKernel.Extensions;

public static class PermissionKeyNormalizer
{
    /// <summary>
    /// Keeps the last two segments of a permission key (controller.action semantics).
    /// Mirrors authorization graph normalization on the API.
    /// </summary>
    public static string Normalize(string? input)
    {
        if (string.IsNullOrWhiteSpace(input))
            return string.Empty;

        var parts = input.Split('.', StringSplitOptions.RemoveEmptyEntries | StringSplitOptions.TrimEntries);
        if (parts.Length == 0)
            return string.Empty;

        var take = Math.Min(2, parts.Length);
        var lastSegments = parts.Skip(parts.Length - take);
        return string.Join('.', lastSegments).ToLowerInvariant();
    }
}
