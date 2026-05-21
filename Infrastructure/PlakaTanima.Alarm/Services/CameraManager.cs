using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Application.Models;

namespace PlakaTanima.Alarm.Services;

public class CameraManager : ICameraManager
{
    private readonly ILogger<CameraManager> _logger;
    private readonly CameraOptions _cameraOptions;
    private readonly ICameraListenerFactory _listenerFactory;
    private readonly List<Task> _runningTasks = new();

    public CameraManager(
        ILogger<CameraManager> logger,
        IOptions<CameraOptions> cameraOptions,
        ICameraListenerFactory listenerFactory)
    {
        _logger = logger;
        _cameraOptions = cameraOptions.Value;
        _listenerFactory = listenerFactory;
    }

    public async Task StartAllCamerasAsync(CancellationToken cancellationToken)
    {
        _logger.LogInformation("Kamera Yönetim Merkezi (Manager) başlatılıyor... Toplam Kamera Sayısı: {Count}", _cameraOptions.Cameras.Count);

        foreach (var camera in _cameraOptions.Cameras)
        {
            try
            {
                _logger.LogInformation("[{CameraName}] İçin dinleyici hazırlanıyor. Marka: {Brand}, IP: {Ip}", camera.Name, camera.Brand, camera.Ip);
                
                var listener = _listenerFactory.GetListener(camera.Brand);

                // Her kamerayı thread'i bloklamayacak şekilde arka planda bağımsız bir Task olarak uçuruyoruz
                var task = Task.Run(async () =>
                {
                    try
                    {
                        await listener.StartListeningAsync(camera, cancellationToken);
                    }
                    catch (Exception ex)
                    {
                        _logger.LogError(ex, "[{CameraName}] Çalışma esnasında kritik bir hata verdi!", camera.Name);
                    }
                }, cancellationToken);

                _runningTasks.Add(task);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "[{CameraName}] Başlatılamadı! Fabrika veya konfigürasyon hatası.", camera.Name);
            }
        }

        // Başlatılan tüm kameraların arka planda paralel akmasını sağlıyoruz
        await Task.WhenAll(_runningTasks);
    }

    public Task StopAllCamerasAsync()
    {
        _logger.LogWarning("Tüm kamera görevleri sonlandırılıyor...");
        _runningTasks.Clear();
        return Task.CompletedTask;
    }
}