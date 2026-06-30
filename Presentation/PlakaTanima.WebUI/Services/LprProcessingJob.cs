using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using PlakaTanima.Application.Services;
using PlakaTanima.Persistence.Contexts;
using PlakaTanima.SignalR.Hubs;

namespace PlakaTanima.WebUI.Services
{
    public class LprProcessingJob : ILprProcessingJob
    {
        private readonly IServiceScopeFactory _scopeFactory;
        private readonly IMinioStorageService _minioService;
        private readonly IHubContext<PlateHub> _hubContext;

        public LprProcessingJob(
            IServiceScopeFactory scopeFactory,
            IMinioStorageService minioService,
            IHubContext<PlateHub> hubContext)
        {
            _scopeFactory = scopeFactory;
            _minioService = minioService;
            _hubContext = hubContext;
        }

        public async Task ProcessEventAsync(Guid eventId, string plate, string cameraName, string timestamp)
        {
            using var scope = _scopeFactory.CreateScope();
            var context = scope.ServiceProvider.GetRequiredService<AppDbContext>();

            // 1. Fetch event from DB with retry logic (protects against database write delays)
            PlakaTanima.Domain.Entities.AnprEvent? eventRecord = null;
            for (int i = 0; i < 5; i++)
            {
                eventRecord = await context.AnprEvents.FirstOrDefaultAsync(x => x.Id == eventId);
                if (eventRecord != null) break;
                await Task.Delay(500); // Delay 500ms and try again
            }

            if (eventRecord == null)
            {
                Console.WriteLine($"[JOB] Error: Event with ID {eventId} not found in database. Aborting processing.");
                return;
            }

            // 2. Query plate from Vehicles table to check list categorizations
            var normalizedPlate = plate.Replace(" ", "").ToUpper();
            PlakaTanima.Domain.Entities.Vehicle? vehicle = await context.Vehicles.FirstOrDefaultAsync(v => 
                v.Plate.Replace(" ", "").ToUpper() == normalizedPlate);

            var category = "normal";
            var owner = "Bilinmeyen Sürücü";
            var model = "Bilinmeyen Araç";

            if (vehicle == null)
            {
                // If it doesn't exist, create and save it automatically as a Normal vehicle
                vehicle = new PlakaTanima.Domain.Entities.Vehicle
                {
                    Id = Guid.NewGuid(),
                    Plate = plate.Trim().ToUpper(),
                    Model = model,
                    Owner = owner,
                    Category = PlakaTanima.Domain.Entities.VehicleCategory.Normal,
                    Note = "Geçiş esnasında otomatik kaydedildi",
                    CreatedAt = DateTime.UtcNow
                };
                await context.Vehicles.AddAsync(vehicle);
                await context.SaveChangesAsync();
            }
            else
            {
                category = vehicle.Category.ToString().ToLower(); // "normal", "vip", "blacklist", "staff"
                owner = vehicle.Owner;
                model = vehicle.Model;
            }

            // 3. Resolve the MinIO image presigned URL
            var vehicleImageUrl = "/img/no-car.png";
            if (!string.IsNullOrEmpty(eventRecord.VehicleImagePath))
            {
                if (eventRecord.VehicleImagePath.StartsWith("platar-bucket/"))
                {
                    vehicleImageUrl = await _minioService.GetPresignedUrlAsync(eventRecord.VehicleImagePath);
                }
                else
                {
                    // Fallback to local files
                    vehicleImageUrl = "/" + eventRecord.VehicleImagePath.Replace("\\", "/");
                }
            }

            // 4. Format timestamp and trigger SignalR Hub event
            var formattedTime = eventRecord.EventTimestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss");
            
            await _hubContext.Clients.All.SendAsync("NewPlateDetected", new
            {
                plate = eventRecord.Plate,
                camera = eventRecord.CameraName,
                fullTime = formattedTime,
                imgVehicle = vehicleImageUrl,
                category = category,
                owner = owner,
                model = model
            });

            Console.WriteLine($"[JOB] Successfully processed and broadcast plate: {eventRecord.Plate} (Category: {category})");
        }
    }
}
