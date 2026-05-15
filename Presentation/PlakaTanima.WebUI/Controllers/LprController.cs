using Hangfire;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Domain.DTO;
using PlakaTanima.Application.Abstract.Jobs;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Caching.Memory; // Cache için eklendi
using System.Text.Json;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LprController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<LprController> _logger;
        private readonly IMemoryCache _cache; // Cooldown kontrolü için

        public LprController(
            IBackgroundJobClient backgroundJobClient,
            ILogger<LprController> logger,
            IMemoryCache cache)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
            _cache = cache;
        }

        [HttpPost("event")]
        public IActionResult ReceiveEvent([FromBody] LprEventDto payload)
        {
            // 1. Mükerrer kontrolü (IMemoryCache)
            string cacheKey = $"lpr_cooldown_{payload.CameraName}_{payload.Plate}";
            if (_cache.TryGetValue(cacheKey, out _))
            {
                return Ok(new { Message = "Duplicate ignored." });
            }
            _cache.Set(cacheKey, true, TimeSpan.FromSeconds(10));

            _logger.LogInformation("🚨 ARAÇ GELDİ: {Plate}", payload.Plate);

            // Çizgiyi yok eden kısım: 
            // payload.Raw zaten JsonElement olduğu için Hangfire bunu otomatik paketler.
            // Ekstra Serialize işlemine burada gerek yok!

            _backgroundJobClient.Enqueue<IEventProcessingJob>(job => job.ProcessEventAsync(payload));

            return Accepted();
        }
    }
}