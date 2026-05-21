using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Alarm.Services;
using PlakaTanima.Alarm.Parsers;

namespace PlakaTanima.Alarm;

public static class DependencyInjection
{
    public static IServiceCollection AddAlarmDI(this IServiceCollection services)
    {
        // Parser kaydı
        services.AddSingleton<IAlarmParser, HikvisionXmlParser>();


        //Alarm Listener Kaydı. Farklı Kamera Eklersen Buraya DI ekleyebilirsin.
        services.AddSingleton<ICameraAlarmListener, HikvisionAlarmListener>();

        services.AddSingleton<ICameraManager, CameraManager>();
        services.AddSingleton<ICameraListenerFactory, CameraListenerFactory>();
        return services;
    }
}