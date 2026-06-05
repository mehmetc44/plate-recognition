using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.Abstractions.Services.ANPR.CameraConnection;

public interface ICameraConnection
{
    Guid CameraId { get; }

    CameraStatus Status { get; }

    Task StartAsync(Camera camera,CancellationToken cancellationToken);

    Task StopAsync(CancellationToken cancellationToken);
}