using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Repositories.CameraRepositories;

public interface ICameraReadRepository
    : IReadRepository<Camera>
{
    Task<List<Camera>> GetAllWithLocationAsync();
    Task<Camera?> GetCameraDetailAsync(Guid id);
    Task<bool> ExistsByIpAddressAsync(
    string ipAddress,
    Guid? excludeId = null);

}