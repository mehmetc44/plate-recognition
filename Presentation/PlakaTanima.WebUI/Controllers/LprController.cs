using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Hangfire;
using PlakaTanima.Application.Services;
using PlakaTanima.Persistence.Contexts;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/lpr")]
    public class LprController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly IMinioStorageService _minioService;

        public LprController(AppDbContext context, IMinioStorageService minioService)
        {
            _context = context;
            _minioService = minioService;
        }

        [HttpPost("event")]
        public IActionResult ReceiveEvent([FromBody] LprEventWebhookDto dto)
        {
            if (dto == null || dto.EventId == Guid.Empty || string.IsNullOrEmpty(dto.Plate))
            {
                return BadRequest(new { success = false, message = "Geçersiz webhook verisi." });
            }

            // Enqueue the heavy database queries and SignalR broadcast to Hangfire background workers
            BackgroundJob.Enqueue<ILprProcessingJob>(x => x.ProcessEventAsync(
                dto.EventId,
                dto.Plate,
                dto.CameraName,
                dto.Timestamp
            ));

            return Ok(new { success = true });
        }

        [HttpGet("query")]
        public async Task<IActionResult> QueryEvents(
            [FromQuery] string? search,
            [FromQuery] string? categories, // comma-separated: e.g. "vip,blacklist"
            [FromQuery] string? direction,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate)
        {
            try
            {
                var query = from e in _context.AnprEvents
                            join v in _context.Vehicles on e.Plate.Replace(" ", "").ToUpper() equals v.Plate.Replace(" ", "").ToUpper() into vehGroup
                            from v in vehGroup.DefaultIfEmpty()
                            select new
                            {
                                Id = e.Id,
                                Plate = e.Plate,
                                CameraName = e.CameraName,
                                EventTimestamp = e.EventTimestamp,
                                Confidence = e.Confidence,
                                VehicleType = e.VehicleType,
                                VehicleColor = e.VehicleColor,
                                VehicleBrand = e.VehicleBrand,
                                Direction = e.Direction,
                                Country = e.Country,
                                PlateImagePath = e.PlateImagePath,
                                VehicleImagePath = e.VehicleImagePath,
                                FullImagePath = e.FullImagePath,
                                Category = v != null ? v.Category : VehicleCategory.Normal,
                                Owner = v != null ? v.Owner : "Bilinmeyen Sürücü",
                                Model = v != null ? v.Model : "Bilinmeyen Araç",
                                Note = v != null ? v.Note : ""
                            };

                // Apply filters
                if (!string.IsNullOrWhiteSpace(search))
                {
                    var s = search.Replace(" ", "").ToUpper();
                    query = query.Where(x => x.Plate.Replace(" ", "").ToUpper().Contains(s) || 
                                             x.CameraName.ToUpper().Contains(s) || 
                                             x.Owner.ToUpper().Contains(s) || 
                                             x.Model.ToUpper().Contains(s));
                }

                if (!string.IsNullOrWhiteSpace(categories) && categories != "all")
                {
                    var catList = categories.Split(',')
                        .Select(c => c.Trim().ToLower())
                        .ToList();
                    
                    query = query.Where(x => catList.Contains(x.Category == VehicleCategory.Vip ? "vip" :
                                             x.Category == VehicleCategory.Blacklist ? "blacklist" :
                                             x.Category == VehicleCategory.Staff ? "staff" : "normal"));
                }

                if (!string.IsNullOrWhiteSpace(direction) && direction != "all")
                {
                    var dir = direction.ToLower();
                    if (dir == "entry" || dir == "giriş")
                    {
                        query = query.Where(x => x.Direction.ToLower() == "forward" || x.Direction.ToLower() == "entry" || x.Direction.ToLower() == "giriş");
                    }
                    else if (dir == "exit" || dir == "çıkış")
                    {
                        query = query.Where(x => x.Direction.ToLower() == "reverse" || x.Direction.ToLower() == "exit" || x.Direction.ToLower() == "çıkış");
                    }
                }

                if (startDate.HasValue)
                {
                    query = query.Where(x => x.EventTimestamp >= startDate.Value.ToUniversalTime());
                }

                if (endDate.HasValue)
                {
                    query = query.Where(x => x.EventTimestamp <= endDate.Value.ToUniversalTime());
                }

                var events = await query
                    .OrderByDescending(x => x.EventTimestamp)
                    .ToListAsync();

                var resultList = new List<object>();
                foreach (var ev in events)
                {
                    var plateImgUrl = await GetImageUrlAsync(ev.PlateImagePath);
                    var vehicleImgUrl = await GetImageUrlAsync(ev.VehicleImagePath);
                    var fullImgUrl = await GetImageUrlAsync(ev.FullImagePath);

                    resultList.Add(new
                    {
                        id = ev.Id,
                        plate = ev.Plate,
                        cameraName = ev.CameraName,
                        eventTimestamp = ev.EventTimestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
                        confidence = ev.Confidence,
                        vehicleType = ev.VehicleType,
                        vehicleColor = ev.VehicleColor,
                        vehicleBrand = ev.VehicleBrand,
                        direction = ev.Direction.ToLower() == "forward" ? "Giriş" : (ev.Direction.ToLower() == "reverse" ? "Çıkış" : ev.Direction),
                        country = ev.Country,
                        plateImg = plateImgUrl,
                        vehicleImg = vehicleImgUrl,
                        fullImg = fullImgUrl,
                        category = ev.Category.ToString().ToLower(),
                        owner = ev.Owner,
                        model = ev.Model,
                        note = ev.Note
                    });
                }

                return Ok(resultList);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        private async Task<string> GetImageUrlAsync(string? path)
        {
            if (string.IsNullOrEmpty(path)) return "/img/no-car.png";
            if (path.StartsWith("platar-bucket/"))
            {
                try
                {
                    return await _minioService.GetPresignedUrlAsync(path);
                }
                catch
                {
                    return "/img/no-car.png";
                }
            }
            return "/" + path.Replace("\\", "/");
        }
    }

    public class LprEventWebhookDto
    {
        public Guid EventId { get; set; }
        public string Plate { get; set; } = null!;
        public string CameraName { get; set; } = null!;
        public string Timestamp { get; set; } = null!;
        public bool HasImages { get; set; }
    }
}
