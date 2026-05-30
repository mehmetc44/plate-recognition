using System;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.CameraRepository;

public interface ICameraReadRepository : IReadRepository<Camera>
{
    Task<Camera?> GetByIpAsync(string ip);

    Task<List<Camera>> GetActiveCamerasAsync();

    Task<bool> ExistsByIpAsync(string ip);
}
