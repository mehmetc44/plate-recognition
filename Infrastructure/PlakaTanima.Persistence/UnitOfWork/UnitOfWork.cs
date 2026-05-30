using System;
using PlakaTanima.Application.Repositories;
using PlakaTanima.Application.Repositories.UnitOfWork;
using PlakaTanima.Domain.Entities.Common;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly AppDbContext _context;

    public UnitOfWork(AppDbContext context)
    {
        _context = context;
    }

    public IReadRepository<T> GetReadRepository<T>() where T : BaseEntity
        => new ReadRepository<T>(_context);

    public IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity
        => new WriteRepository<T>(_context);

    public async Task<int> SaveAsync()
        => await _context.SaveChangesAsync();
}
