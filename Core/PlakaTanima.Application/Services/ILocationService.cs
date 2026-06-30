using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Locations;

namespace PlakaTanima.Application.Services
{
    public interface ILocationService
    {
        Task<List<LocationDto>> GetTreeAsync();
        Task<Guid> AddLocationAsync(CreateLocationDto dto);
        Task<bool> UpdateLocationAsync(UpdateLocationDto dto);
        Task<bool> DeleteLocationAsync(Guid id);
        Task<bool> HasCameraAsync(Guid id);
        Task<bool> ExistsByIdAsync(Guid id);
    }
}
