using System;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.CameraRepository;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
using PlakaTanima.Persistence.Contexts;

namespace PlakaTanima.Persistence.Repositories.CameraRepository;

public class CameraWriteRepository : WriteRepository<Camera>, ICameraWriteRepository
{
    public CameraWriteRepository(AppDbContext context) : base(context)
    {
    }

    public async Task SetStatusAsync(Guid cameraId, CameraStatus status)
    {
        var camera = await _context.Set<Camera>()
            .FirstOrDefaultAsync(x => x.Id == cameraId);

        if (camera == null)
            return;

        camera.Status = status;
    }

    public async Task UpdateLastCheckedAsync(Guid cameraId)
    {
        var camera = await _context.Set<Camera>()
            .FirstOrDefaultAsync(x => x.Id == cameraId);

        if (camera == null)
            return;

        camera.LastCheckedAt = DateTime.UtcNow;
    }

    public async Task UpdateCredentialsAsync(Guid cameraId, string username, string password)
    {
        var camera = await _context.Set<Camera>()
            .FirstOrDefaultAsync(x => x.Id == cameraId);

        if (camera == null)
            return;

        camera.Username = username;
        camera.Password = password;
    }

    public async Task UpdateStreamConfigAsync(Guid cameraId, int streamChannel, int port)
    {
        var camera = await _context.Set<Camera>()
            .FirstOrDefaultAsync(x => x.Id == cameraId);

        if (camera == null)
            return;

        camera.StreamChannel = streamChannel;
        camera.Port = port;
    }
}
