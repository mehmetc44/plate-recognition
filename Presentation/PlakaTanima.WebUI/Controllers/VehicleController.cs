using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.Repositories.VehicleRepositories;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.WebUI.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleRepository _vehicleRepository;

        public VehicleController(IVehicleRepository vehicleRepository)
        {
            _vehicleRepository = vehicleRepository;
        }

        // GET: /Vehicle/GetVehiclesApi
        [HttpGet]
        public async Task<IActionResult> GetVehiclesApi()
        {
            try
            {
                var vehicles = await _vehicleRepository.GetAll(tracking: false)
                    .OrderByDescending(x => x.CreatedAt)
                    .ToListAsync();

                var dtos = vehicles.Select(v => new VehicleDto
                {
                    Id = v.Id,
                    Plate = v.Plate,
                    Model = v.Model,
                    Owner = v.Owner,
                    Category = MapCategoryToString(v.Category),
                    Note = v.Note ?? "",
                    Date = v.CreatedAt.ToString("dd.MM.yyyy")
                }).ToList();

                return Json(dtos);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // GET: /Vehicle/GetVehicleByPlateApi
        [HttpGet]
        public async Task<IActionResult> GetVehicleByPlateApi([FromQuery] string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
                return Json(new { success = false, message = "Plaka belirtilmedi." });

            try
            {
                var normalizedPlate = plate.Replace(" ", "").ToUpper();
                var vehicle = await _vehicleRepository.GetAll(tracking: false)
                    .FirstOrDefaultAsync(v => v.Plate.Replace(" ", "").ToUpper() == normalizedPlate);

                if (vehicle == null)
                {
                    return Json(new { 
                        success = true,
                        exists = false,
                        plate = plate.Trim().ToUpper(),
                        model = "Bilinmeyen Araç",
                        owner = "Bilinmeyen Sürücü",
                        category = "normal",
                        note = "Sistemde kayıtlı değil.",
                        date = "-"
                    });
                }

                return Json(new {
                    success = true,
                    exists = true,
                    id = vehicle.Id,
                    plate = vehicle.Plate,
                    model = vehicle.Model,
                    owner = vehicle.Owner,
                    category = MapCategoryToString(vehicle.Category),
                    note = vehicle.Note ?? "",
                    date = vehicle.CreatedAt.ToString("dd.MM.yyyy")
                });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Vehicle/AddVehicleApi
        [HttpPost]
        public async Task<IActionResult> AddVehicleApi([FromBody] CreateVehicleDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });

            try
            {
                var upperPlate = command.Plate.Trim().ToUpper();
                var exists = await _vehicleRepository.ExistsByPlateAsync(upperPlate);
                if (exists)
                    return Json(new { success = false, message = "Bu plaka ile kayıtlı araç zaten mevcut." });

                var vehicle = new Vehicle
                {
                    Plate = upperPlate,
                    Model = command.Model.Trim(),
                    Owner = command.Owner.Trim(),
                    Category = MapCategory(command.Category),
                    Note = command.Note?.Trim()
                };

                await _vehicleRepository.AddAsync(vehicle);
                await _vehicleRepository.SaveAsync();

                return Json(new { success = true, message = "Araç başarıyla eklendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Vehicle/UpdateVehicleApi
        [HttpPost]
        public async Task<IActionResult> UpdateVehicleApi([FromBody] UpdateVehicleDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });

            try
            {
                var upperPlate = command.Plate.Trim().ToUpper();
                var exists = await _vehicleRepository.ExistsByPlateAsync(upperPlate, command.Id);
                if (exists)
                    return Json(new { success = false, message = "Bu plaka ile başka bir kayıt mevcut." });

                var vehicle = await _vehicleRepository.GetByIdAsync(command.Id, tracking: true);
                if (vehicle == null)
                    return Json(new { success = false, message = "Araç kaydı bulunamadı." });

                vehicle.Plate = upperPlate;
                vehicle.Model = command.Model.Trim();
                vehicle.Owner = command.Owner.Trim();
                vehicle.Category = MapCategory(command.Category);
                vehicle.Note = command.Note?.Trim();
                vehicle.UpdatedAt = DateTime.UtcNow;

                _vehicleRepository.Update(vehicle);
                await _vehicleRepository.SaveAsync();

                return Json(new { success = true, message = "Araç başarıyla güncellendi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // POST: /Vehicle/DeleteVehicleApi
        [HttpPost]
        public async Task<IActionResult> DeleteVehicleApi(Guid id)
        {
            try
            {
                var vehicle = await _vehicleRepository.GetByIdAsync(id, tracking: true);
                if (vehicle == null)
                    return Json(new { success = false, message = "Araç bulunamadı." });

                _vehicleRepository.Remove(vehicle);
                await _vehicleRepository.SaveAsync();

                return Json(new { success = true, message = "Araç başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private VehicleCategory MapCategory(string category)
        {
            return category.ToLower() switch
            {
                "vip" => VehicleCategory.Vip,
                "blacklist" => VehicleCategory.Blacklist,
                "staff" => VehicleCategory.Staff,
                _ => VehicleCategory.Normal
            };
        }

        private string MapCategoryToString(VehicleCategory category)
        {
            return category switch
            {
                VehicleCategory.Vip => "vip",
                VehicleCategory.Blacklist => "blacklist",
                VehicleCategory.Staff => "staff",
                _ => "normal"
            };
        }
    }

    public class VehicleDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string Note { get; set; } = "";
        public string Date { get; set; } = null!;
    }

    public class CreateVehicleDto
    {
        public string Plate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Note { get; set; }
    }

    public class UpdateVehicleDto
    {
        public Guid Id { get; set; }
        public string Plate { get; set; } = null!;
        public string Model { get; set; } = null!;
        public string Owner { get; set; } = null!;
        public string Category { get; set; } = null!;
        public string? Note { get; set; }
    }
}
