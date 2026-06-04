using System.Linq.Expressions;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories;

public interface IReadRepository<T>
    where T : BaseEntity
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
}