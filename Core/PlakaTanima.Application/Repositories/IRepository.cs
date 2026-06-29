using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories;

public interface IRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll(bool tracking = false);

    IQueryable<T> Where(
        Expression<Func<T, bool>> predicate,
        bool tracking = false);

    Task<T?> GetByIdAsync(
        Guid id,
        bool tracking = false);

    Task<T?> FirstOrDefaultAsync(
        Expression<Func<T, bool>> predicate,
        bool tracking = false);

    Task AddAsync(T entity);

    Task AddRangeAsync(IEnumerable<T> entities);

    void Update(T entity);

    void Remove(T entity);

    Task<int> SaveAsync();
}
