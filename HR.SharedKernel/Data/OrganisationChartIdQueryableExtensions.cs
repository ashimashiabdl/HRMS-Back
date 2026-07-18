using System.Linq.Expressions;

namespace HR.SharedKernel.Data;

public static class OrganisationChartIdQueryableExtensions
{
    private static readonly System.Collections.Concurrent.ConcurrentDictionary<Type, bool> OrganScopedCache = new();

    public static bool IsOrganisationChartScoped(Type entityType) =>
        OrganScopedCache.GetOrAdd(entityType, t => t.GetInterfaces().Contains(typeof(IOrganisationChartId)));

    public static IQueryable<T> WhereDefaultOrganisationChartId<T>(
        this IQueryable<T> query,
        long organisationChartId,
        bool ignoreDefaultOrganId = false)
        where T : class
    {
        if (ignoreDefaultOrganId || organisationChartId <= 0 || !IsOrganisationChartScoped(typeof(T)))
            return query;

        var xParam = Expression.Parameter(typeof(T), "x");
        var colExpr = Expression.Property(xParam, nameof(IOrganisationChartId.OrganisationChartId));
        var constExpr = Expression.Constant(organisationChartId);
        var lambdaBody = Expression.MakeBinary(ExpressionType.Equal, colExpr, constExpr);
        var lambda = Expression.Lambda<Func<T, bool>>(lambdaBody, xParam);
        return query.Where(lambda);
    }
}
