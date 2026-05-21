using PlakaTanima.Application.Models;

namespace PlakaTanima.Application.Abstract;

public interface ICameraAlarmListener
{
    string Brand { get; }
    Task StartListeningAsync(CameraConfig config, CancellationToken cancellationToken);
}