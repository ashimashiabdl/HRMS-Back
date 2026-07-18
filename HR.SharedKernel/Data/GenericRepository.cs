

using HR.SharedKernel.Extensions;
using HR.SharedKernel.Service;
using LinqKit;
using Microsoft.EntityFrameworkCore;

using System.Linq.Expressions;
using System.Reflection;


namespace HR.SharedKernel.Data;

public class GenericRepository<T, C> : IGenericRepository<T, C>
    where T : BaseEntity
    where C : IAppDbContext, IDisposable
{
    private bool _isDisposed;
    private readonly UserResolverService _userService;


    public GenericRepository(C context, UserResolverService userService)
    {
        _db = context ?? throw new ArgumentNullException(nameof(context));
        _userService = userService ?? throw new ArgumentNullException(nameof(userService));
    }

    public IAppDbContext _db;

    public void Dispose()
    {
        if (_db != null)
            _db.Dispose();
        _isDisposed = true;
    }

    public virtual void Add(T entity)
    {
        try
        {
            EnsureContextAvailable();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            entity.CreateDate = DateTime.Now;
            entity.IPAddress = _userService.GetIP();
            _db.Set<T>().Add(entity);
        }
        catch (Exception dbEx)
        {
            HandleUnitOfWorkException(dbEx);
        }
    }

    public virtual void AddRange(IQueryable<T> objModel)
    {
        try
        {
            EnsureContextAvailable();
            if (objModel == null)
            {
                throw new ArgumentNullException(nameof(objModel));
            }

            var items = objModel.ToList();
            foreach (var item in items)
            {
                item.IPAddress = _userService.GetIP();
                item.CreateDate = DateTime.Now;
            }

            _db.Set<T>().AddRange(items);
        }
        catch (Exception dbEx)
        {
            HandleUnitOfWorkException(dbEx);
        }
    }

    public async Task<T> GetIdAsync(long id)
    {
        EnsureContextAvailable();
        return await _db.Set<T>().FindAsync(id);
    }

    public IQueryable<T> All(bool IgnoreExpired = true, DateTime? ImpleDate = null)
    {
        EnsureContextAvailable();
        return _db.Set<T>().Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired, ImpleDate));
    }

    public async Task<T> Get(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true)
    {
        return await GetAsync(predicate, IgnoreExpired);
    }

    public async Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true)
    {
        EnsureContextAvailable();
        return await _db.Set<T>()
            .Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired))
            .FirstOrDefaultAsync(predicate);
    }

    public async Task<List<T>> GetList(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true)
    {
        return await GetListAsync(predicate, IgnoreExpired);
    }

    public async Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true)
    {
        EnsureContextAvailable();
        return await _db.Set<T>()
            .Where(predicate)
            .Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired).And(i => i.IsDeleted != true))
            .ToListAsync();
    }

    public async Task<List<T>> GetAll(bool IgnoreExpired = true)
    {
        EnsureContextAvailable();
        return await _db.Set<T>()
            .Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired).And(i => i.IsDeleted != true))
            .ToListAsync();
    }

    public async Task<List<T>> GetAllAsync(bool IgnoreExpired = true, string? SqlRaw = null)
    {
        EnsureContextAvailable();
        if (!string.IsNullOrEmpty(SqlRaw))
        {
            return await _db.Set<T>()
                .FromSqlRaw(SqlRaw)
                .Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired))
                .ToListAsync();
        }

        return await _db.Set<T>()
            .Where(DateValidityExtension<T>.GetDateValidationPredicate(IgnoreExpired))
            .ToListAsync();
    }

    public int Count()
    {
        EnsureContextAvailable();
        return _db.Set<T>().Count();
    }

    public async Task<int> CountAsync()
    {
        EnsureContextAvailable();
        return await _db.Set<T>().CountAsync();
    }

    public void Update(T entity)
    {
        try
        {
            EnsureContextAvailable();
            if (entity == null)
            {
                throw new ArgumentNullException(nameof(entity));
            }

            if (entity.StartDate.HasValue && entity.EndDate.HasValue && entity.StartDate > entity.EndDate)
            {
                throw new ArgumentException("تاریخ اعتبار صحیح وارد نشده");
            }

            entity.LastModifiedDate = DateTime.Now;
            entity.IPAddress = _userService.GetIP();

            var entry = _db.Entry(entity);
            var dbValues = entry.GetDatabaseValues();

            if (dbValues == null)
            {
                if (entity.CreateDate == null)
                {
                    entity.CreateDate = DateTime.Now;
                }

                // Added/Unchanged/Modified are already tracked; Detached persisted rows must be attached.
                if (entry.State == EntityState.Detached && entity.Id > 0)
                {
                    _db.Set<T>().Attach(entity);
                    entry.State = EntityState.Modified;
                }

                return;
            }

            if (dbValues.GetValue<object>("CreateDate") == null)
            {
                entity.CreateDate = DateTime.Now;
            }
            else
            {
                entity.CreateDate = Convert.ToDateTime(dbValues.GetValue<object>("CreateDate"));
            }

            if (typeof(T).GetInterfaces().Contains(typeof(IOrganisationChartId)))
            {
                var organisationChartId = dbValues.GetValue<long>("OrganisationChartId");
                var organisationChartProperty = entity.GetType().GetProperty("OrganisationChartId");
                organisationChartProperty?.SetValue(entity, organisationChartId, null);
            }

            var trackedDuplicate = _db.Set<T>()
                .Local
                .FirstOrDefault(localEntity => localEntity.Id.Equals(entity.Id) && !ReferenceEquals(localEntity, entity));

            if (trackedDuplicate != null)
            {
                _db.Entry(trackedDuplicate).State = EntityState.Detached;
            }

            entry.State = EntityState.Modified;
        }
        catch (Exception dbEx)
        {
            HandleUnitOfWorkException(dbEx);
        }
    }

    public void LogicalRemove(long Id)
    {
        try
        {
            EnsureContextAvailable();
            if (Id == 0)
            {
                throw new ArgumentNullException(nameof(Id));
            }

            var objModel = _db.Set<T>().Find(Id);
            if (objModel == null)
            {
                throw new KeyNotFoundException($"Entity with id {Id} was not found.");
            }

            objModel.IsDeleted = true;
            objModel.LastModifiedDate = DateTime.Now;
            objModel.IPAddress = _userService.GetIP();
            _db.Entry(objModel).State = EntityState.Modified;
        }
        catch (Exception dbEx)
        {
            HandleUnitOfWorkException(dbEx);
        }
    }

    private void EnsureContextAvailable()
    {
        if (_db == null || _isDisposed)
        {
            throw new ObjectDisposedException(nameof(GenericRepository<T, C>));
        }
    }

    private static void HandleUnitOfWorkException(Exception dbEx)
    {
        throw dbEx;
    }
}
