using System;
using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Cameras;

namespace PlakaTanima.Application.Services
{
    public interface ICameraService
    {
        Task<CameraDto?> GetCameraByIdAsync(Guid id);
        Task<bool> ExistsByIpAddressAsync(string ipAddress);
        Task<bool> ExistsByIpAddressAsync(string ipAddress, Guid excludeId);
        Task<Guid> AddCameraAsync(CreateCameraDto dto);
        Task<bool> UpdateCameraAsync(UpdateCameraDto dto);
        Task<bool> DeleteCameraAsync(Guid id);
    }
}
