using System;
using Microsoft.AspNetCore.Mvc;
using Hangfire;
using PlakaTanima.Application.Services;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/lpr")]
    public class LprController : ControllerBase
    {
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
