using Hangfire;
using Hangfire.PostgreSql;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Services;
using PlakaTanima.Infrastructure.Services;

namespace PlakaTanima.Infrastructure
{
    public static class DependencyInjection
    {
        public static IServiceCollection AddInfrastructureDI(this IServiceCollection services, IConfiguration configuration)
        {
            // 1. Configure Hangfire Database Storage (PostgreSQL) and Server
            services.AddHangfire(config => config
                .UsePostgreSqlStorage(c => c.UseNpgsqlConnection(configuration.GetConnectionString("DefaultConnection"))));

            services.AddHangfireServer();

            // 2. Register Application Storage (Singleton Minio Storage Service)
            services.AddSingleton<IMinioStorageService, MinioStorageService>();

            return services;
        }
    }
}
