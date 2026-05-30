using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.Repositories.CameraRepository
{
    public interface ICameraWriteRepository : IWriteRepository<Camera>
    {

        Task SetStatusAsync(Guid cameraId, CameraStatus status);

        Task UpdateLastCheckedAsync(Guid cameraId);
    }
}