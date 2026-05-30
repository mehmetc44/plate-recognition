using System;
using System.Linq.Expressions;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories;

public interface IReadRepository<T> where T : BaseEntity
{
    IQueryable<T> GetAll(bool tracking = true);

    IQueryable<T> GetWhere(Expression<Func<T, bool>> predicate, bool tracking = true);

    Task<T?> GetSingleAsync(Expression<Func<T, bool>> predicate, bool tracking = true);

    Task<T?> GetByIdAsync(Guid id, bool tracking = true);
}