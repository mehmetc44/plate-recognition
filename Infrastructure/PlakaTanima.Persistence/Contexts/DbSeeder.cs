using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Persistence.Contexts
{
    public static class DbSeeder
    {
        public static async Task SeedAsync(AppDbContext context)
        {
            // 1. Migrate database
            await context.Database.MigrateAsync();

            // 2. Seed Locations
            Location girisLoc = null!;
            Location cikisLoc = null!;

            var locationsTable = context.Set<Location>();
            if (!await locationsTable.AnyAsync())
            {
                girisLoc = new Location
                {
                    Id = Guid.NewGuid(),
                    Name = "30 Ağustos Giriş",
                    Description = "Ana Giriş Kapısı",
                    CreatedAt = DateTime.UtcNow
                };

                cikisLoc = new Location
                {
                    Id = Guid.NewGuid(),
                    Name = "30 Ağustos Çıkış",
                    Description = "Ana Çıkış Kapısı",
                    CreatedAt = DateTime.UtcNow
                };

                await locationsTable.AddRangeAsync(girisLoc, cikisLoc);
                await context.SaveChangesAsync();
            }
            else
            {
                girisLoc = await locationsTable.FirstOrDefaultAsync(x => x.Name.Contains("Giriş")) ?? await locationsTable.FirstAsync();
                cikisLoc = await locationsTable.FirstOrDefaultAsync(x => x.Name.Contains("Çıkış")) ?? await locationsTable.FirstAsync();
            }

            // 3. Seed Cameras
            if (!await context.Cameras.AnyAsync())
            {
                var cameras = new[]
                {
                    new Camera
                    {
                        Id = Guid.NewGuid(),
                        Name = "30Agustos_GIRIS",
                        LocationId = girisLoc.Id,
                        IpAddress = "10.179.0.81",
                        Port = 80,
                        Username = "admin",
                        Password = "kutup12.",
                        StreamChannel = 101,
                        Status = CameraStatus.Online,
                        CreatedAt = DateTime.UtcNow
                    },
                    new Camera
                    {
                        Id = Guid.NewGuid(),
                        Name = "30Agustos_CIKIS",
                        LocationId = cikisLoc.Id,
                        IpAddress = "10.179.0.80",
                        Port = 80,
                        Username = "admin",
                        Password = "kutup12.",
                        StreamChannel = 101,
                        Status = CameraStatus.Online,
                        CreatedAt = DateTime.UtcNow
                    }
                };

                await context.Cameras.AddRangeAsync(cameras);
                await context.SaveChangesAsync();
            }

            // 4. Seed Vehicles
            if (!await context.Vehicles.AnyAsync())
            {
                var vehicles = new[]
                {
                    new Vehicle { Id = Guid.NewGuid(), Plate = "34 ABC 123", Model = "Mercedes E200 Siyah", Owner = "Ahmet Yılmaz", Category = VehicleCategory.Vip, Note = "Yönetim kurulu üyesi", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "06 XYZ 987", Model = "BMW X5 Beyaz", Owner = "Mehmet Kara", Category = VehicleCategory.Blacklist, Note = "Güvenlik riski", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "35 VMS 404", Model = "Audi A6 Gri", Owner = "Ayşe Demir", Category = VehicleCategory.Vip, Note = "Özel misafir", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "16 NMG 772", Model = "Toyota Corolla Mavi", Owner = "Ali Yıldız", Category = VehicleCategory.Normal, Note = "", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "53 KRM 112", Model = "Ford Transit Beyaz", Owner = "Veli Can", Category = VehicleCategory.Blacklist, Note = "Hırsızlık şüphesi", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "21 AAA 001", Model = "Volkswagen Passat Siyah", Owner = "Zeynep Koç", Category = VehicleCategory.Staff, Note = "Personel aracı", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "38 AAA 543", Model = "Mercedes S400 Siyah", Owner = "Mustafa Öztürk", Category = VehicleCategory.Vip, Note = "CEO aracı", CreatedAt = DateTime.UtcNow },
                    new Vehicle { Id = Guid.NewGuid(), Plate = "07 BLL 234", Model = "Fiat Egea Kırmızı", Owner = "Hakan Usta", Category = VehicleCategory.Normal, Note = "", CreatedAt = DateTime.UtcNow }
                };

                await context.Vehicles.AddRangeAsync(vehicles);
                await context.SaveChangesAsync();
            }
        }
    }
}
