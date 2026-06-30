using AutoMapper;
using PlakaTanima.Application.DTOs.Locations;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Application.Services;
using PlakaTanima.Domain.Entities;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlakaTanima.Persistence.Services
{
    public class LocationService : ILocationService
    {
        private readonly ILocationRepository _locationRepository;
        private readonly IMapper _mapper;

        public LocationService(ILocationRepository locationRepository, IMapper mapper)
        {
            _locationRepository = locationRepository;
            _mapper = mapper;
        }

        public async Task<List<LocationDto>> GetTreeAsync()
        {
            var locations = await _locationRepository.GetAllWithCamerasAsync();
            return _mapper.Map<List<LocationDto>>(locations);
        }

        public async Task<Guid> AddLocationAsync(CreateLocationDto dto)
        {
            var location = _mapper.Map<Location>(dto);
            await _locationRepository.AddAsync(location);
            await _locationRepository.SaveAsync();
            return location.Id;
        }

        public async Task<bool> UpdateLocationAsync(UpdateLocationDto dto)
        {
            var location = await _locationRepository.GetByIdAsync(dto.Id, tracking: true);
            if (location == null) return false;

            _mapper.Map(dto, location);
            location.UpdatedAt = DateTime.UtcNow;

            _locationRepository.Update(location);
            await _locationRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteLocationAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            if (location == null) return false;

            _locationRepository.Remove(location);
            await _locationRepository.SaveAsync();
            return true;
        }

        public async Task<bool> HasCameraAsync(Guid id)
        {
            return await _locationRepository.HasCameraAsync(id);
        }

        public async Task<bool> ExistsByIdAsync(Guid id)
        {
            var location = await _locationRepository.GetByIdAsync(id);
            return location != null;
        }
    }
}
