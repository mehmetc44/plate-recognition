using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Persistence.Contexts;
using PlakaTanima.Persistence.Repositories.CameraRepositories;
using PlakaTanima.Persistence.Repositories.LocationRepositories;

namespace PlakaTanima.Persistence;

public static class DependencyInjection
{
    public static IServiceCollection AddPersistenceDI(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        services.AddDbContext<AppDbContext>(options =>
            options.UseNpgsql(
                configuration.GetConnectionString("DefaultConnection")));

        services.AddScoped<ILocationRepository, LocationRepository>();
        services.AddScoped<ICameraRepository, CameraRepository>();
        return services;
    }
}