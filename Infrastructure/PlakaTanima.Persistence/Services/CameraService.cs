using AutoMapper;
using PlakaTanima.Application.DTOs.Cameras;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Services;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
using System;
using System.Threading.Tasks;

namespace PlakaTanima.Persistence.Services
{
    public class CameraService : ICameraService
    {
        private readonly ICameraRepository _cameraRepository;
        private readonly IMapper _mapper;

        public CameraService(ICameraRepository cameraRepository, IMapper mapper)
        {
            _cameraRepository = cameraRepository;
            _mapper = mapper;
        }

        public async Task<CameraDto?> GetCameraByIdAsync(Guid id)
        {
            var camera = await _cameraRepository.GetByIdAsync(id);
            if (camera == null) return null;

            return _mapper.Map<CameraDto>(camera);
        }

        public async Task<bool> ExistsByIpAddressAsync(string ipAddress)
        {
            return await _cameraRepository.ExistsByIpAddressAsync(ipAddress);
        }

        public async Task<bool> ExistsByIpAddressAsync(string ipAddress, Guid excludeId)
        {
            return await _cameraRepository.ExistsByIpAddressAsync(ipAddress, excludeId);
        }

        public async Task<Guid> AddCameraAsync(CreateCameraDto dto)
        {
            var camera = _mapper.Map<Camera>(dto);
            await _cameraRepository.AddAsync(camera);
            await _cameraRepository.SaveAsync();
            return camera.Id;
        }

        public async Task<bool> UpdateCameraAsync(UpdateCameraDto dto)
        {
            var camera = await _cameraRepository.GetByIdAsync(dto.Id, tracking: true);
            if (camera == null) return false;

            _mapper.Map(dto, camera);
            camera.UpdatedAt = DateTime.UtcNow;

            _cameraRepository.Update(camera);
            await _cameraRepository.SaveAsync();
            return true;
        }

        public async Task<bool> DeleteCameraAsync(Guid id)
        {
            var camera = await _cameraRepository.GetByIdAsync(id);
            if (camera == null) return false;

            _cameraRepository.Remove(camera);
            await _cameraRepository.SaveAsync();
            return true;
        }
    }
}
