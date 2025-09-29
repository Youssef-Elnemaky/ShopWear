using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using ShopWear.DataAccess.Data;
using ShopWear.DataAccess.Interfaces.Repositories;

namespace ShopWear.DataAccess.Repositories;

public class Repository<T> : IRepository<T> where T : class
{
    private readonly AppDbContext _db;
    private readonly DbSet<T> _dbSet;
    public Repository(AppDbContext db)
    {
        _db = db;
        _dbSet = _db.Set<T>();
    }

    private IQueryable<T> BuildQuery(
            Expression<Func<T, bool>>? filter = null,
            Func<IQueryable<T>, IQueryable<T>>? include = null,
            Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
            bool asNoTracking = true,
            int? skip = null,
            int? take = null)
    {
        IQueryable<T> query = _dbSet;

        if (asNoTracking) query = query.AsNoTracking();
        if (filter is not null) query = query.Where(filter);
        if (include is not null) query = include(query);
        if (orderBy is not null) query = orderBy(query);
        if (skip.HasValue) query = query.Skip(skip.Value);
        if (take.HasValue) query = query.Take(take.Value);

        return query;
    }

    public async Task<T> AddAsync(T entity)
    {
        await _dbSet.AddAsync(entity);
        return entity;
    }

    public Task AddRangeAsync(IEnumerable<T> entities)
    {
        return _dbSet.AddRangeAsync(entities);
    }

    public Task<int> CountAsync(Expression<Func<T, bool>>? filter = null)
    {
        return BuildQuery(filter).CountAsync();
    }

    public Task DeleteAsync(T entity)
    {
        _dbSet.Remove(entity);
        return Task.CompletedTask;
    }

    public Task DeleteRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.RemoveRange(entities);
        return Task.CompletedTask;
    }

    public async Task<IReadOnlyList<T>> GetAllAsync(
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        int? skip = null,
        int? take = null)
    {
        IQueryable<T> query = BuildQuery(filter, include, orderBy, asNoTracking, skip, take);

        return await query.ToListAsync();
    }

    public async Task<IReadOnlyList<TResult>> GetAllAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>>? filter = null,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        Func<IQueryable<T>, IOrderedQueryable<T>>? orderBy = null,
        bool asNoTracking = true,
        int? skip = null,
        int? take = null)
    {
        IQueryable<T> query = BuildQuery(filter, include, orderBy, asNoTracking, skip, take);

        return await query.Select(selector).ToListAsync();
    }

    public async Task<T?> GetAsync(
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = true)
    {
        IQueryable<T> query = BuildQuery(filter, include, null, asNoTracking);

        return await query.FirstOrDefaultAsync();
    }

    public async Task<TResult?> GetAsync<TResult>(
        Expression<Func<T, TResult>> selector,
        Expression<Func<T, bool>> filter,
        Func<IQueryable<T>, IQueryable<T>>? include = null,
        bool asNoTracking = true)
    {
        IQueryable<T> query = BuildQuery(filter, include, null, asNoTracking);

        return await query.Select(selector).FirstOrDefaultAsync();
    }

    public Task UpdateAsync(T entity)
    {
        _dbSet.Update(entity);
        return Task.CompletedTask;
    }

    public Task UpdateRangeAsync(IEnumerable<T> entities)
    {
        _dbSet.UpdateRange(entities);
        return Task.CompletedTask;
    }
}