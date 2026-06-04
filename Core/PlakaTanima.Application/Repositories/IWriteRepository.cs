using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    void Remove(T entity);

    Task<int> SaveAsync();
}