using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.References;
using PlakaTanima.Application.Models;


namespace PlakaTanima.WebUI;

public static class DependencyInjection
{
    public static IServiceCollection AddWebUIDI(this IServiceCollection services, IConfiguration configuration)
    {
        // "CamerasSection" yerine direkt kökteki "Cameras" dizisini hedef gösteriyoruz
        services.Configure<CameraOptions>(configuration.GetSection("CamerasSection"));
        services.AddHttpClient();
        services.AddMediatR(cfg =>
{
    cfg.RegisterServicesFromAssembly(
        typeof(ApplicationAssemblyReference).Assembly);
});
        return services;
    }
}