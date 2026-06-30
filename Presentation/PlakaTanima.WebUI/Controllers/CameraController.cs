using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.DTOs.Cameras;
using PlakaTanima.Application.DTOs.Locations;
using PlakaTanima.Application.Services;

namespace PlakaTanima.WebUI.Controllers
{
    public class CameraController : Controller
    {
        private readonly ICameraService _cameraService;
        private readonly ILocationService _locationService;

        public CameraController(
            ICameraService cameraService,
            ILocationService locationService)
        {
            _cameraService = cameraService;
            _locationService = locationService;
        }

        // ========== JSON API: Tree verisi ==========
        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            try
            {
                var result = await _locationService.GetTreeAsync();
                return Json(result);
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========== JSON API: Kamera detay ==========
        [HttpGet]
        public async Task<IActionResult> GetCameraDetail(Guid id)
        {
            var camera = await _cameraService.GetCameraByIdAsync(id);
            if (camera == null)
                return NotFound();

            return Json(camera);
        }

        // ========== CAMERA CRUD (JSON) ==========
        [HttpPost]
        public async Task<IActionResult> AddCameraApi([FromBody] CreateCameraDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var locationExists = await _locationService.ExistsByIdAsync(command.LocationId);
                if (!locationExists)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                var ipExists = await _cameraService.ExistsByIpAddressAsync(command.IpAddress);
                if (ipExists)
                    return Json(new { success = false, message = "Bu IP adresi ile kayıtlı kamera mevcut." });

                var id = await _cameraService.AddCameraAsync(command);
                return Json(new { success = true, id = id, name = command.Name });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCameraApi([FromBody] UpdateCameraDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var locationExists = await _locationService.ExistsByIdAsync(command.LocationId);
                if (!locationExists)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                var ipExists = await _cameraService.ExistsByIpAddressAsync(command.IpAddress, command.Id);
                if (ipExists)
                    return Json(new { success = false, message = "Bu IP adresi başka bir kamerada kullanılıyor." });

                var success = await _cameraService.UpdateCameraAsync(command);
                if (!success)
                    return Json(new { success = false, message = "Kamera bulunamadı." });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCameraApi([FromBody] DeleteRequest request)
        {
            try
            {
                await _cameraService.DeleteCameraAsync(request.Id);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========== LOCATION CRUD (JSON API) ==========
        [HttpPost]
        public async Task<IActionResult> AddLocationApi([FromBody] CreateLocationDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var id = await _locationService.AddLocationAsync(command);
                return Json(new { success = true, id = id, name = command.Name });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocationApi([FromBody] UpdateLocationDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var success = await _locationService.UpdateLocationAsync(command);
                if (!success)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocationApi([FromBody] DeleteRequest request)
        {
            try
            {
                var hasCameras = await _locationService.HasCameraAsync(request.Id);
                if (hasCameras)
                    return Json(new { success = false, message = "Kamerası bulunan bir lokasyon silinemez." });

                var success = await _locationService.DeleteLocationAsync(request.Id);
                return Json(new { success = success });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========== TRADITIONAL POST ACTIONS (for form fallback) ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(CreateLocationDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            await _locationService.AddLocationAsync(command);
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLocation(UpdateLocationDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            await _locationService.UpdateLocationAsync(command);
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocation(DeleteRequest command)
        {
            await _locationService.DeleteLocationAsync(command.Id);
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCamera(CreateCameraDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            await _cameraService.AddCameraAsync(command);
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCamera(UpdateCameraDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            await _cameraService.UpdateCameraAsync(command);
            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCamera(DeleteRequest command)
        {
            await _cameraService.DeleteCameraAsync(command.Id);
            return RedirectToAction("Index", "Settings");
        }
    }

    public class DeleteRequest
    {
        public Guid Id { get; set; }
    }
}