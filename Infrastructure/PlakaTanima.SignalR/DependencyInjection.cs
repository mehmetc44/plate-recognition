using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Abstract.Services;
using PlakaTanima.SignalR.Services;

namespace PlakaTanima.SignalR;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRDI(
        this IServiceCollection services)
    {
        services.AddSignalR();

        services.AddScoped<
            IPlateNotificationService,
            SignalRPlateNotificationService>();

        return services;
    }
}