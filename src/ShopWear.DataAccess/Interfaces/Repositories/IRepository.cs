using System.Linq.Expressions;

namespace ShopWear.DataAccess.Interfaces.Repositories;

public interface IRepository<T> where T : class
{
    Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        int? skip = null,
        int? take = null);

    Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        int? skip = null,
        int? take = null);

    Task<TResult?> GetAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = true);

    Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = true);

    Task<int> CountAsync(Expression<Func<T, bool>>? filter = null);

    Task<T> AddAsync(T entity);
    Task AddRangeAsync(IEnumerable<T> entities);
    Task UpdateAsync(T entity);
    Task UpdateRangeAsync(IEnumerable<T> entities);

    Task DeleteAsync(T entity);
    Task DeleteRangeAsync(IEnumerable<T> entities);
}