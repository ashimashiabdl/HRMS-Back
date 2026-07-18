using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using System.Threading.Tasks;

namespace HR.SharedKernel.Data;

public interface IGenericRepository<T, C> where T : BaseEntity where C : IAppDbContext
{
    void Add(T objModel);
    IQueryable<T> All(bool IgnoreExpired = true, DateTime? ImpleDate = null);
    void AddRange(IQueryable<T> objModel);
    //T GetId(long id);
    Task<T> GetIdAsync(long id);
    Task<T> Get(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true);
    Task<T> GetAsync(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true);
    Task<List<T>> GetList(Expression<Func<T, bool>> predicate, bool IgnoreExpired = true);

    Task<List<T>> GetAll(bool IgnoreExpired = true);

    int Count();
    Task<int> CountAsync();
    void Update(T objModel);
    void LogicalRemove(long Id);
    void Dispose();
    Task<List<T>> GetListAsync(Expression<Func<T, bool>> predicate , bool IgnoreExpired = true);
    Task<List<T>> GetAllAsync(bool IgnoreExpired = true, string? SqlRaw = null);



    // int Savechange();
}
