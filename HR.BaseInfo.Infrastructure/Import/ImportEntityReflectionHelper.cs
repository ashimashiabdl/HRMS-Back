using System.Linq.Expressions;
using HR.SharedKernel.Data;
using Microsoft.EntityFrameworkCore;

namespace HR.BaseInfo.infrastructure.Import;

public static class ImportEntityReflectionHelper
{
    public static async Task<List<BaseEntity>> LoadActiveEntitiesAsync(DbContext context, Type entityType)
    {
        if (!typeof(BaseEntity).IsAssignableFrom(entityType))
            return [];

        var queryable = GetDbSet(context, entityType);
        var filtered = ApplyIsDeletedFilter(queryable, entityType);
        return await ToListAsync(filtered, entityType);
    }

    public static async Task<List<BaseEntity>> LoadEntitiesByIdsAsync(DbContext context, Type entityType, IReadOnlyList<long> ids)
    {
        if (ids.Count == 0 || !typeof(BaseEntity).IsAssignableFrom(entityType))
            return [];

        var queryable = GetDbSet(context, entityType);
        var param = Expression.Parameter(entityType, "e");
        var idProp = Expression.Property(param, nameof(BaseEntity.Id));
        var containsMethod = typeof(Enumerable).GetMethods()
            .First(m => m.Name == nameof(Enumerable.Contains) && m.GetParameters().Length == 2)
            .MakeGenericMethod(typeof(long));
        var idsConst = Expression.Constant(ids);
        var containsCall = Expression.Call(containsMethod, idsConst, idProp);
        var lambda = Expression.Lambda(containsCall, param);

        var whereMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        var filtered = (IQueryable)whereMethod.Invoke(null, new object[] { queryable, lambda })!;
        return await ToListAsync(filtered, entityType);
    }

    public static void AddEntity(DbContext context, Type entityType, object entity)
    {
        var addMethod = typeof(DbContext).GetMethod(nameof(DbContext.Add), new[] { typeof(object) })!;
        addMethod.Invoke(context, new[] { entity });
    }

    private static IQueryable GetDbSet(DbContext context, Type entityType)
    {
        var setMethod = typeof(DbContext).GetMethod(nameof(DbContext.Set), Type.EmptyTypes)!
            .MakeGenericMethod(entityType);
        return (IQueryable)setMethod.Invoke(context, null)!;
    }

    private static IQueryable ApplyIsDeletedFilter(IQueryable queryable, Type entityType)
    {
        var param = Expression.Parameter(entityType, "e");
        var isDeleted = Expression.Property(param, nameof(BaseEntity.IsDeleted));
        var body = Expression.Equal(isDeleted, Expression.Constant(false));
        var lambda = Expression.Lambda(body, param);

        var whereMethod = typeof(Queryable).GetMethods()
            .First(m => m.Name == nameof(Queryable.Where) && m.GetParameters().Length == 2)
            .MakeGenericMethod(entityType);

        return (IQueryable)whereMethod.Invoke(null, new object[] { queryable, lambda })!;
    }

    private static async Task<List<BaseEntity>> ToListAsync(IQueryable queryable, Type entityType)
    {
        var toListMethod = typeof(EntityFrameworkQueryableExtensions)
            .GetMethods()
            .First(m => m.Name == nameof(EntityFrameworkQueryableExtensions.ToListAsync)
                        && m.GetParameters().Length == 2
                        && m.GetParameters()[1].ParameterType == typeof(CancellationToken))
            .MakeGenericMethod(entityType);

        var task = (Task)toListMethod.Invoke(null, new object[] { queryable, CancellationToken.None })!;
        await task.ConfigureAwait(false);
        var result = task.GetType().GetProperty("Result")!.GetValue(task)!;
        return ((System.Collections.IEnumerable)result).Cast<BaseEntity>().ToList();
    }
}
