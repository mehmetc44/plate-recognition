using System;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.DTOs.Cameras;
using PlakaTanima.Application.DTOs.Locations;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.WebUI.Controllers
{
    public class CameraController : Controller
    {
        private readonly ICameraRepository _cameraRepository;
        private readonly ILocationRepository _locationRepository;

        public CameraController(
            ICameraRepository cameraRepository,
            ILocationRepository locationRepository)
        {
            _cameraRepository = cameraRepository;
            _locationRepository = locationRepository;
        }

        // ========== JSON API: Tree verisi ==========
        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            try
            {
                var locations = await _locationRepository.GetAllWithCamerasAsync();
                var result = locations.Select(loc => new LocationDto
                {
                    Id = loc.Id,
                    Name = loc.Name,
                    Description = loc.Description,
                    Cameras = loc.Cameras.Select(c => new CameraDto
                    {
                        Id = c.Id,
                        Name = c.Name,
                        LocationId = c.LocationId,
                        IpAddress = c.IpAddress,
                        Port = c.Port,
                        Username = c.Username,
                        Password = c.Password,
                        StreamChannel = c.StreamChannel,
                        Status = c.Status.ToString().ToLower()
                    }).ToList()
                }).ToList();

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
            var camera = await _cameraRepository.GetByIdAsync(id);
            if (camera == null)
                return NotFound();

            var dto = new CameraDto
            {
                Id = camera.Id,
                Name = camera.Name,
                LocationId = camera.LocationId,
                IpAddress = camera.IpAddress,
                Port = camera.Port,
                Username = camera.Username,
                Password = camera.Password,
                StreamChannel = camera.StreamChannel,
                Status = camera.Status.ToString().ToLower()
            };

            return Json(dto);
        }

        // ========== CAMERA CRUD (JSON) ==========
        [HttpPost]
        public async Task<IActionResult> AddCameraApi([FromBody] CreateCameraDto command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var location = await _locationRepository.GetByIdAsync(command.LocationId);
                if (location == null)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                var ipExists = await _cameraRepository.ExistsByIpAddressAsync(command.IpAddress);
                if (ipExists)
                    return Json(new { success = false, message = "Bu IP adresi ile kayıtlı kamera mevcut." });

                var camera = new Camera
                {
                    Name = command.Name,
                    LocationId = command.LocationId,
                    IpAddress = command.IpAddress,
                    Port = command.Port,
                    Username = command.Username,
                    Password = command.Password,
                    StreamChannel = command.StreamChannel,
                    Status = CameraStatus.Offline
                };

                await _cameraRepository.AddAsync(camera);
                await _cameraRepository.SaveAsync();

                return Json(new { success = true, id = camera.Id, name = camera.Name });
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
                var camera = await _cameraRepository.GetByIdAsync(command.Id, true);
                if (camera == null)
                    return Json(new { success = false, message = "Kamera bulunamadı." });

                var location = await _locationRepository.GetByIdAsync(command.LocationId);
                if (location == null)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                var ipExists = await _cameraRepository.ExistsByIpAddressAsync(command.IpAddress, command.Id);
                if (ipExists)
                    return Json(new { success = false, message = "Bu IP adresi başka bir kamerada kullanılıyor." });

                camera.Name = command.Name;
                camera.LocationId = command.LocationId;
                camera.IpAddress = command.IpAddress;
                camera.Port = command.Port;
                camera.Username = command.Username;
                camera.Password = command.Password;
                camera.StreamChannel = command.StreamChannel;
                camera.UpdatedAt = DateTime.UtcNow;

                _cameraRepository.Update(camera);
                await _cameraRepository.SaveAsync();

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
                var camera = await _cameraRepository.GetByIdAsync(request.Id);
                if (camera != null)
                {
                    _cameraRepository.Remove(camera);
                    await _cameraRepository.SaveAsync();
                }
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
                var location = new Location
                {
                    Name = command.Name,
                    Description = command.Description
                };

                await _locationRepository.AddAsync(location);
                await _locationRepository.SaveAsync();

                return Json(new { success = true, id = location.Id, name = location.Name });
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
                var location = await _locationRepository.GetByIdAsync(command.Id, true);
                if (location == null)
                    return Json(new { success = false, message = "Lokasyon bulunamadı." });

                location.Name = command.Name;
                location.Description = command.Description;
                location.UpdatedAt = DateTime.UtcNow;

                _locationRepository.Update(location);
                await _locationRepository.SaveAsync();

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
                var location = await _locationRepository.GetByIdAsync(request.Id);
                if (location != null)
                {
                    // Check if has cameras
                    var hasCameras = await _locationRepository.HasCameraAsync(request.Id);
                    if (hasCameras)
                        return Json(new { success = false, message = "Kamerası bulunan bir lokasyon silinemez." });

                    _locationRepository.Remove(location);
                    await _locationRepository.SaveAsync();
                }
                return Json(new { success = true });
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

            var location = new Location { Name = command.Name, Description = command.Description };
            await _locationRepository.AddAsync(location);
            await _locationRepository.SaveAsync();

            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLocation(UpdateLocationDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            var location = await _locationRepository.GetByIdAsync(command.Id, true);
            if (location != null)
            {
                location.Name = command.Name;
                location.Description = command.Description;
                _locationRepository.Update(location);
                await _locationRepository.SaveAsync();
            }

            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocation(DeleteRequest command)
        {
            var location = await _locationRepository.GetByIdAsync(command.Id);
            if (location != null)
            {
                _locationRepository.Remove(location);
                await _locationRepository.SaveAsync();
            }
            return Ok();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCamera(CreateCameraDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            var camera = new Camera
            {
                Name = command.Name,
                LocationId = command.LocationId,
                IpAddress = command.IpAddress,
                Port = command.Port,
                Username = command.Username,
                Password = command.Password,
                StreamChannel = command.StreamChannel,
                Status = CameraStatus.Offline
            };
            await _cameraRepository.AddAsync(camera);
            await _cameraRepository.SaveAsync();

            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCamera(UpdateCameraDto command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction("Index", "Settings");

            var camera = await _cameraRepository.GetByIdAsync(command.Id, true);
            if (camera != null)
            {
                camera.Name = command.Name;
                camera.LocationId = command.LocationId;
                camera.IpAddress = command.IpAddress;
                camera.Port = command.Port;
                camera.Username = command.Username;
                camera.Password = command.Password;
                camera.StreamChannel = command.StreamChannel;
                _cameraRepository.Update(camera);
                await _cameraRepository.SaveAsync();
            }

            return RedirectToAction("Index", "Settings");
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCamera(DeleteRequest command)
        {
            var camera = await _cameraRepository.GetByIdAsync(command.Id);
            if (camera != null)
            {
                _cameraRepository.Remove(camera);
                await _cameraRepository.SaveAsync();
            }
            return RedirectToAction("Index", "Settings");
        }
    }

    public class DeleteRequest
    {
        public Guid Id { get; set; }
    }
}