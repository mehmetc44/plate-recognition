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