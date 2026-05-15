using Hangfire;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Domain.DTO;
using PlakaTanima.Application.Abstract.Jobs;
using Microsoft.Extensions.Logging;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LprController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;
        private readonly ILogger<LprController> _logger;

        public LprController(IBackgroundJobClient backgroundJobClient, ILogger<LprController> logger)
        {
            _backgroundJobClient = backgroundJobClient;
            _logger = logger;
        }

        [HttpPost("event")]
        public IActionResult ReceiveEvent([FromBody] LprEventDto payload)
        {
            _logger.LogInformation("🚨 YENİ PLAKA GELDİ! Kamera: {Camera} | Plaka: {Plate}", payload.CameraName, payload.Plate);

            // KRİTİK DÜZELTME: JsonElement çöpe gitmeden önce onu string'e çevirip güvene alıyoruz.
            if (payload.Raw != null)
            {
                payload.Raw = System.Text.Json.JsonSerializer.Serialize(payload.Raw);
            }

            _backgroundJobClient.Enqueue<IEventProcessingJob>(job => job.ProcessEventAsync(payload));

            return Accepted(new { Message = "Event received and queued for processing." });
        }
    }
}