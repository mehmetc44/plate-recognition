using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories;
using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Persistence.Contexts;
using System.Linq.Expressions;

namespace PlakaTanima.Persistence.Repositories;

public class ReadRepository<T> : IReadRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _context;

    public ReadRepository(AppDbContext context)
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
}