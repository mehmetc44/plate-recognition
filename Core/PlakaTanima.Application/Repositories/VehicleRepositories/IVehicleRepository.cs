using System;
using System.Threading.Tasks;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.VehicleRepositories;

public interface IVehicleRepository : IRepository<Vehicle>
{
    Task<bool> ExistsByPlateAsync(string plate, Guid? excludeId = null);
}
