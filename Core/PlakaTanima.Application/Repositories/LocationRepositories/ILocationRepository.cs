using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.LocationRepositories;

public interface ILocationRepository : IRepository<Location>
{
    Task<List<Location>> GetAllAsync();
    Task<bool> ExistsByNameAsync(string name, Guid? excludeId = null);
    Task<bool> HasCameraAsync(Guid locationId);
    Task<List<Location>> GetAllWithCamerasAsync();
}
