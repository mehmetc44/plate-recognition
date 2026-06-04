using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.LocationRepositories;

public interface ILocationReadRepository
    : IReadRepository<Location>
{
    Task<List<Location>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name,Guid? excludeId = null);
    Task<bool> HasCameraAsync(Guid locationId);
}