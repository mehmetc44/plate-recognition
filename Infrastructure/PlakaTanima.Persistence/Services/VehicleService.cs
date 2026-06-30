using AutoMapper;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.DTOs.Vehicles;
using PlakaTanima.Application.Repositories.VehicleRepositories;
using PlakaTanima.Application.Services;
using PlakaTanima.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlakaTanima.Persistence.Services
{
    public class VehicleService : IVehicleService
    {
        private readonly IVehicleRepository _vehicleRepository;
        private readonly IMapper _mapper;

        public VehicleService(IVehicleRepository vehicleRepository, IMapper mapper)
        {
            _vehicleRepository = vehicleRepository;
            _mapper = mapper;
        }

        public async Task<List<VehicleDto>> GetVehiclesAsync()
        {
            var vehicles = await _vehicleRepository.GetAll(tracking: false)
                .OrderByDescending(x => x.CreatedAt)
                .ToListAsync();

            return _mapper.Map<List<VehicleDto>>(vehicles);
        }

        public async Task<VehicleDetailResponseDto> GetVehicleByPlateAsync(string plate)
        {
            var normalizedPlate = plate.Replace(" ", "").ToUpper();
            var vehicle = await _vehicleRepository.GetAll(tracking: false)
                .FirstOrDefaultAsync(v => v.Plate.Replace(" ", "").ToUpper() == normalizedPlate);

            if (vehicle == null)
            {
                return new VehicleDetailResponseDto
                {
                    Exists = false,
                    Plate = plate.Trim().ToUpper(),
                    Model = "Bilinmeyen Araç",
                    Owner = "Bilinmeyen Sürücü",
                    Category = "normal",
                    Note = "Sistemde kayıtlı değil.",
                    Date = "-"
                };
            }

            var dto = _mapper.Map<VehicleDto>(vehicle);
            return new VehicleDetailResponseDto
            {
                Exists = true,
                Id = dto.Id,
                Plate = dto.Plate,
                Model = dto.Model,
                Owner = dto.Owner,
                Category = dto.Category,
                Note = dto.Note,
                Date = dto.Date
            };
        }

        public async Task<bool> ExistsByPlateAsync(string plate)
        {
            return await _vehicleRepository.ExistsByPlateAsync(plate);
        }

        public async Task<bool> ExistsByPlateAsync(string plate, Guid excludeId)
        {
            return await _vehicleRepository.ExistsByPlateAsync(plate, excludeId);
        }

        public async Task<bool> AddVehicleAsync(CreateVehicleDto dto)
        {
            var vehicle = _mapper.Map<Vehicle>(dto);
            await _vehicleRepository.AddAsync(vehicle);
            await _vehicleRepository.SaveAsync();
            return true;
        }

        public async Task<bool> UpdateVehicleAsync(UpdateVehicleDto dto)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(dto.Id, tracking: true);
            if (vehicle == null) return false;

            _mapper.Map(dto, vehicle);
            vehicle.UpdatedAt = DateTime.UtcNow;

            _vehicleRepository.Update(vehicle);
            await _vehicleRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteVehicleAsync(Guid id)
        {
            var vehicle = await _vehicleRepository.GetByIdAsync(id, tracking: true);
            if (vehicle == null) return false;

            _vehicleRepository.Remove(vehicle);
            await _vehicleRepository.SaveAsync();
            return true;
        }
    }
}
