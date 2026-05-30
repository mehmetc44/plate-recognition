using Microsoft.Extensions.DependencyInjection;

namespace PlakaTanima.SignalR;

public static class DependencyInjection
{
    public static IServiceCollection AddSignalRDI(
        this IServiceCollection services)
    {
        services.AddSignalR();


        return services;
    }
}