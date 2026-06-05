using PlakaTanima.Application.Models.ANPR;

namespace PlakaTanima.Application.Abstractions.Services.ANPR.CameraManager;

public interface ICameraManager
{
    Task StartAsync(CancellationToken cancellationToken);
    Task StopAsync(CancellationToken cancellationToken);
    Task ReloadAsync(CancellationToken cancellationToken);
    IReadOnlyCollection<CameraConnectionInfo> GetConnections();
    CameraConnectionInfo? GetConnection(Guid cameraId);
}