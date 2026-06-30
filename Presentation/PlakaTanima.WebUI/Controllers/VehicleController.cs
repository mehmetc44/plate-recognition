using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.DTOs.Vehicles;
using PlakaTanima.Application.Services;

namespace PlakaTanima.WebUI.Controllers
{
    public class VehicleController : Controller
    {
        private readonly IVehicleService _vehicleService;

        public VehicleController(IVehicleService vehicleService)
        {
            _vehicleService = vehicleService;
        }

        // GET: /Vehicle/GetVehiclesApi
        [HttpGet]
        public async Task<IActionResult> GetVehiclesApi()
        {
            try
            {
                var dtos = await _vehicleService.GetVehiclesAsync();
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
                var details = await _vehicleService.GetVehicleByPlateAsync(plate);
                return Json(new
                {
                    success = true,
                    exists = details.Exists,
                    id = details.Id,
                    plate = details.Plate,
                    model = details.Model,
                    owner = details.Owner,
                    category = details.Category,
                    note = details.Note,
                    date = details.Date
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
                var exists = await _vehicleService.ExistsByPlateAsync(command.Plate);
                if (exists)
                    return Json(new { success = false, message = "Bu plaka ile kayıtlı araç zaten mevcut." });

                await _vehicleService.AddVehicleAsync(command);
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
                var exists = await _vehicleService.ExistsByPlateAsync(command.Plate, command.Id);
                if (exists)
                    return Json(new { success = false, message = "Bu plaka ile başka bir kayıt mevcut." });

                var success = await _vehicleService.UpdateVehicleAsync(command);
                if (!success)
                    return Json(new { success = false, message = "Araç kaydı bulunamadı." });

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
                var success = await _vehicleService.DeleteVehicleAsync(id);
                if (!success)
                    return Json(new { success = false, message = "Araç bulunamadı." });

                return Json(new { success = true, message = "Araç başarıyla silindi!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
    }
}
