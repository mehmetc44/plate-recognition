using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Models;
using PlakaTanima.WebUI.HostedServices;


namespace PlakaTanima.WebUI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebUIDI(this IServiceCollection services ,IConfiguration configuration  )
    {
        // "CamerasSection" yerine direkt kökteki "Cameras" dizisini hedef gösteriyoruz
        services.Configure<CameraOptions>(configuration.GetSection("CamerasSection"));
        services.AddHttpClient();
        services.AddHostedService<CameraAlarmHostedService>();
        return services;
    }
}