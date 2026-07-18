using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.ChangeTracking;
using Microsoft.EntityFrameworkCore.Infrastructure;
using System.Linq.Expressions;

namespace HR.SharedKernel.Data;

public interface IAppDbContext : IDisposable
{
    DbSet<TEntity> Set<TEntity>() where TEntity : class;

    void Add<TEntity>(TEntity objModel) where TEntity : class;
    void Remove<TEntity>(TEntity objModel) where TEntity : class;
    void AddRange<TEntity>(IQueryable<TEntity> objModel) where TEntity : class;

    IQueryable<TEntity> All<TEntity>(bool IgnoreExpired = true, DateTime? ImpleDate = null) where TEntity : class;

    Task<TEntity> GetIdAsync<TEntity>(long id) where TEntity : class;
    Task<TEntity> Get<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class;
    Task<TEntity> GetAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class;

    Task<List<TEntity>> GetList<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class;
    Task<List<TEntity>> GetListAsync<TEntity>(Expression<Func<TEntity, bool>> predicate, bool IgnoreExpired = true) where TEntity : class;

    Task<List<TEntity>> GetAllAsync<TEntity>(bool IgnoreExpired = true, string? SqlRaw = null) where TEntity : class;
    Task<List<TEntity>> GetAll<TEntity>(bool IgnoreExpired = true) where TEntity : class;

    int Count<TEntity>() where TEntity : class;
    Task<int> CountAsync<TEntity>() where TEntity : class;

    void Update<TEntity>(TEntity objModel) where TEntity : class;
    void LogicalRemove<TEntity>(long Id) where TEntity : class;

    EntityEntry<TEntity> Entry<TEntity>(TEntity entity) where TEntity : class;

    Task<int> SaveChangesAsync(CancellationToken cancellationToken = default);
    DatabaseFacade Database { get; }
}
