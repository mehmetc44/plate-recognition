using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.CameraRepositories;

public interface ICameraRepository : IRepository<Camera>
{
    Task<List<Camera>> GetAllWithLocationAsync();
    Task<Camera?> GetCameraDetailAsync(Guid id);
    Task<bool> ExistsByIpAddressAsync(string ipAddress, Guid? excludeId = null);
}
