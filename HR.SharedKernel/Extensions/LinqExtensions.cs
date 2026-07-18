using System.Linq.Expressions;

namespace HR.SharedKernel.Extensions;

public static class LinqExtensions
{
    /// <summary>
    /// Applies dynamic sorting on an <see cref="IQueryable{T}"/> using a property name (supports dotted paths).
    /// Returns the original query when the property path is invalid.
    /// </summary>
    public static IQueryable<T> OrderBy<T>(this IQueryable<T> source, string orderByProperty, bool descending)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(orderByProperty))
        {
            return source;
        }

        if (!PropertyPathExpressionBuilder.TryBuildMemberAccess(
                typeof(T),
                orderByProperty,
                out var parameter,
                out var propertyAccess,
                out var keyType))
        {
            return source;
        }

        var command = descending ? "OrderByDescending" : "OrderBy";
        var orderByExpression = Expression.Lambda(propertyAccess, parameter);
        var resultExpression = Expression.Call(
            typeof(Queryable),
            command,
            [typeof(T), keyType],
            source.Expression,
            Expression.Quote(orderByExpression));

        return source.Provider.CreateQuery<T>(resultExpression);
    }

    /// <summary>
    /// Applies in-memory generic search on properties marked with <see cref="IsEffectiveInGenericSearch"/>.
    /// Intended for use after materialization (<c>AsEnumerable</c>) when EF translation is not required.
    /// </summary>
    public static IEnumerable<T> Filter<T>(this IEnumerable<T> source, string filter)
    {
        ArgumentNullException.ThrowIfNull(source);

        if (string.IsNullOrWhiteSpace(filter))
        {
            return source;
        }

        var predicate = GenericSearchExpressionBuilder.BuildPredicate<T>(filter.Trim());
        if (predicate == null)
        {
            return source.Where(_ => false);
        }

        return source.Where(predicate.Compile());
    }
}
