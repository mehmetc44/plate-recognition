using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Vehicles;

namespace PlakaTanima.Application.Services
{
    public interface IVehicleService
    {
        Task<List<VehicleDto>> GetVehiclesAsync();
        Task<VehicleDetailResponseDto> GetVehicleByPlateAsync(string plate);
        Task<bool> ExistsByPlateAsync(string plate);
        Task<bool> ExistsByPlateAsync(string plate, Guid excludeId);
        Task<bool> AddVehicleAsync(CreateVehicleDto dto);
        Task<bool> UpdateVehicleAsync(UpdateVehicleDto dto);
        Task<bool> DeleteVehicleAsync(Guid id);
    }
}
