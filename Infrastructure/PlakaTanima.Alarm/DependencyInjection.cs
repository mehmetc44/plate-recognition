using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Alarm.Services;

namespace PlakaTanima.Alarm;

public static class DependencyInjection
{
    public static IServiceCollection AddAlarmDI(this IServiceCollection services)
    {
        services.AddSingleton<ICameraAlarmListener, HikvisionAlarmListener>();
        // Yarın bir gün Dahua gelirse: services.AddSingleton<ICameraListener, DahuaListener>();
        services.AddSingleton<ICameraManager, CameraManager>();
        services.AddSingleton<ICameraListenerFactory, CameraListenerFactory>();
        return services;
    }
}