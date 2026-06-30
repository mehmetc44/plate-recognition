using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.DTOs.Lpr;
using PlakaTanima.Application.Services;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/lpr")]
    public class LprController : ControllerBase
    {
        private readonly ILprService _lprService;

        public LprController(ILprService lprService)
        {
            _lprService = lprService;
        }

        [HttpPost("event")]
        public IActionResult ReceiveEvent([FromBody] LprEventWebhookDto dto)
        {
            if (dto == null || dto.EventId == Guid.Empty || string.IsNullOrEmpty(dto.Plate))
            {
                return BadRequest(new { success = false, message = "Geçersiz webhook verisi." });
            }

            _lprService.EnqueueLprEventProcessing(dto);
            return Ok(new { success = true });
        }

        [HttpGet("query")]
        public async Task<IActionResult> QueryEvents(
            [FromQuery] string? search,
            [FromQuery] string? categories,
            [FromQuery] string? direction,
            [FromQuery] DateTime? startDate,
            [FromQuery] DateTime? endDate,
            [FromQuery] int page = 1,
            [FromQuery] int pageSize = 50)
        {
            try
            {
                var result = await _lprService.QueryEventsAsync(
                    search,
                    categories,
                    direction,
                    startDate,
                    endDate,
                    page,
                    pageSize
                );

                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }

        [HttpGet("history")]
        public async Task<IActionResult> GetPlateHistory([FromQuery] string plate)
        {
            if (string.IsNullOrWhiteSpace(plate))
            {
                return BadRequest(new { success = false, message = "Plaka belirtilmedi." });
            }

            try
            {
                var result = await _lprService.GetPlateHistoryAsync(plate);
                return Ok(result);
            }
            catch (Exception ex)
            {
                return StatusCode(500, new { success = false, message = ex.Message });
            }
        }
    }
}
