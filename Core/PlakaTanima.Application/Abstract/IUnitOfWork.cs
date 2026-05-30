using System;
using PlakaTanima.Domain.Entities.Common;

namespace PlakaTanima.Application.Repositories.UnitOfWork;

public interface IUnitOfWork
{
    IReadRepository<T> GetReadRepository<T>() where T : BaseEntity;

    IWriteRepository<T> GetWriteRepository<T>() where T : BaseEntity;

    Task<int> SaveAsync();
}
