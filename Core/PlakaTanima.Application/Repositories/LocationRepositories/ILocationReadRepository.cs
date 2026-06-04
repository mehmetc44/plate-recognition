using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.LocationRepositories;

public interface ILocationReadRepository
    : IReadRepository<Location>
{
    Task<List<Location>> GetAllAsync();
}