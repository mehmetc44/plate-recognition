namespace PlakaTanima.Application.Abstract;

public interface ICameraManager
{
    Task StartAllCamerasAsync(CancellationToken cancellationToken);
    Task StopAllCamerasAsync();
}