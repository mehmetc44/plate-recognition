using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using PlakaTanima.Application.Abstract;

namespace PlakaTanima.WebUI.HostedServices;

public class CameraAlarmHostedService : BackgroundService
{
    private readonly ILogger<CameraAlarmHostedService> _logger;
    private readonly ICameraManager _cameraManager;

    public CameraAlarmHostedService(ILogger<CameraAlarmHostedService> logger, ICameraManager cameraManager)
    {
        _logger = logger;
        _cameraManager = cameraManager;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        _logger.LogInformation("CameraAlarmHostService tetiklendi, yönetim manager'a devrediliyor...");
        
        // Uygulama ayağa kalktığında manager'a tüm kameraları başlat emri veriyoruz
        await _cameraManager.StartAllCamerasAsync(stoppingToken);
    }

    public override async Task StopAsync(CancellationToken cancellationToken)
    {
        _logger.LogWarning("Uygulama kapatılıyor, CameraAlarmHostService kameraları durduruyor...");
        await _cameraManager.StopAllCamerasAsync();
        await base.StopAsync(cancellationToken);
    }
}