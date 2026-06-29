using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories;
using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories;

public class Repository<T> : IRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _context;

    public Repository(AppDbContext context)
    {
        _context = context;
    }

    protected DbSet<T> Table => _context.Set<T>();

    public IQueryable<T> GetAll(bool tracking = false)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return query;
    }

    public IQueryable<T> Where(
        Expression<Func<T, bool>> predicate,
        bool tracking = false)
    {
        var query = Table.Where(predicate);

        if (!tracking)
            query = query.AsNoTracking();

        return query;
    }

    public async Task<T?> GetByIdAsync(
        Guid id,
        bool tracking = false)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(x => x.Id == id);
    }

    public async Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false)
    {
        var query = Table.AsQueryable();

        if (!tracking)
            query = query.AsNoTracking();

        return await query.FirstOrDefaultAsync(predicate);
    }

    public async Task AddAsync(T entity)
    {
        await Table.AddAsync(entity);
    }

    public async Task AddRangeAsync(IEnumerable<T> entities)
    {
        await Table.AddRangeAsync(entities);
    }

    public void Update(T entity)
    {
        Table.Update(entity);
    }

    public void Remove(T entity)
    {
        Table.Remove(entity);
    }

    public async Task<int> SaveAsync()
    {
        return await _context.SaveChangesAsync();
    }
}
