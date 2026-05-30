using System;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories;

public interface IWriteRepository<T> where T : BaseEntity
{
    Task<bool> AddAsync(T entity);

    Task<bool> AddRangeAsync(List<T> entities);

    bool Update(T entity);

    bool Remove(T entity);

    bool RemoveRange(List<T> entities);
}
