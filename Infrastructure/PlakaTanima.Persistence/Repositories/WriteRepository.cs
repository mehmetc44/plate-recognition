using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories;
using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories;

public class WriteRepository<T> : IWriteRepository<T>
    where T : BaseEntity
{
    protected readonly AppDbContext _context;

    public WriteRepository(AppDbContext context)
    {
        _context = context;
    }

    protected DbSet<T> Table => _context.Set<T>();

    public async Task<bool> AddAsync(T entity)
    {
        var entry = await Table.AddAsync(entity);
        return entry.State == EntityState.Added;
    }

    public async Task<bool> AddRangeAsync(List<T> entities)
    {
        await Table.AddRangeAsync(entities);
        return true;
    }

    public bool Update(T entity)
    {
        var entry = Table.Update(entity);
        return entry.State == EntityState.Modified;
    }

    public bool Remove(T entity)
    {
        var entry = Table.Remove(entity);
        return entry.State == EntityState.Deleted;
    }

    public bool RemoveRange(List<T> entities)
    {
        Table.RemoveRange(entities);
        return true;
    }
}