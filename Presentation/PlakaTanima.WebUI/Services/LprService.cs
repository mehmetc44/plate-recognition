using Hangfire;
using Microsoft.EntityFrameworkCore;
using PlakaTanima.Application.DTOs.Lpr;
using PlakaTanima.Application.Services;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace PlakaTanima.WebUI.Services
{
    public class LprService : ILprService
    {
        private readonly AppDbContext _context;
        private readonly IMinioStorageService _minioService;

        public LprService(AppDbContext context, IMinioStorageService minioService)
        {
            _context = context;
            _minioService = minioService;
        }

        public void EnqueueLprEventProcessing(LprEventWebhookDto dto)
        {
            BackgroundJob.Enqueue<ILprProcessingJob>(x => x.ProcessEventAsync(
                dto.EventId,
                dto.Plate,
                dto.CameraName,
                dto.Timestamp
            ));
        }

        public async Task<QueryEventsResponseDto> QueryEventsAsync(
            string? search,
            string? categories,
            string? direction,
            DateTime? startDate,
            DateTime? endDate,
            int page,
            int pageSize)
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

            // 1. Get filtered total count of unique plates
            var filteredCount = await query.CountAsync();

            // 2. Get total database count of all events
            var totalCount = await _context.AnprEvents.CountAsync();

            // 3. Paginate
            var events = await query
                .OrderByDescending(x => x.EventTimestamp)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();

            var resultList = new List<AnprEventDto>();
            foreach (var ev in events)
            {
                var plateImgUrl = await GetImageUrlAsync(ev.PlateImagePath);
                var vehicleImgUrl = await GetImageUrlAsync(ev.VehicleImagePath);
                var fullImgUrl = await GetImageUrlAsync(ev.FullImagePath);

                resultList.Add(new AnprEventDto
                {
                    Id = ev.Id,
                    Plate = ev.Plate,
                    CameraName = ev.CameraName,
                    EventTimestamp = ev.EventTimestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
                    Confidence = ev.Confidence,
                    VehicleType = ev.VehicleType,
                    VehicleColor = ev.VehicleColor,
                    VehicleBrand = ev.VehicleBrand,
                    Direction = ev.Direction.ToLower() == "forward" ? "Giriş" : (ev.Direction.ToLower() == "reverse" ? "Çıkış" : ev.Direction),
                    Country = ev.Country,
                    PlateImg = plateImgUrl,
                    VehicleImg = vehicleImgUrl,
                    FullImg = fullImgUrl,
                    Category = ev.Category.ToString().ToLower(),
                    Owner = ev.Owner,
                    Model = ev.Model,
                    Note = ev.Note
                });
            }

            return new QueryEventsResponseDto
            {
                TotalCount = totalCount,
                FilteredCount = filteredCount,
                Events = resultList
            };
        }

        public async Task<List<AnprEventHistoryDto>> GetPlateHistoryAsync(string plate)
        {
            var normalizedPlate = plate.Replace(" ", "").ToUpper();
            var passes = await _context.AnprEvents
                .Where(x => x.Plate.Replace(" ", "").ToUpper() == normalizedPlate)
                .OrderByDescending(x => x.EventTimestamp)
                .Take(10)
                .ToListAsync();

            var result = new List<AnprEventHistoryDto>();
            foreach (var p in passes)
            {
                var plateImgUrl = await GetImageUrlAsync(p.PlateImagePath);
                var vehicleImgUrl = await GetImageUrlAsync(p.VehicleImagePath);
                var fullImgUrl = await GetImageUrlAsync(p.FullImagePath);

                result.Add(new AnprEventHistoryDto
                {
                    Id = p.Id,
                    Plate = p.Plate,
                    CameraName = p.CameraName,
                    EventTimestamp = p.EventTimestamp.ToLocalTime().ToString("dd.MM.yyyy HH:mm:ss"),
                    Direction = p.Direction.ToLower() == "forward" ? "Giriş" : (p.Direction.ToLower() == "reverse" ? "Çıkış" : p.Direction),
                    PlateImg = plateImgUrl,
                    VehicleImg = vehicleImgUrl,
                    FullImg = fullImgUrl
                });
            }

            return result;
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
}
