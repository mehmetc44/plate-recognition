using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Application.Repositories.VehicleRepositories;
using PlakaTanima.Application.Services;
using PlakaTanima.Persistence.Contexts;
using PlakaTanima.Persistence.Repositories.CameraRepositories;
using PlakaTanima.Persistence.Repositories.LocationRepositories;
using PlakaTanima.Persistence.Repositories.VehicleRepositories;
using PlakaTanima.Persistence.Services;

namespace PlakaTanima.Persistence
{
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
            services.AddScoped<IVehicleRepository, VehicleRepository>();

            services.AddScoped<IVehicleService, VehicleService>();
            services.AddScoped<ICameraService, CameraService>();
            services.AddScoped<ILocationService, LocationService>();

            return services;
        }
    }
}