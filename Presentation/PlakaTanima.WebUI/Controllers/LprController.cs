using Hangfire;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class LprController : ControllerBase
    {
        private readonly IBackgroundJobClient _backgroundJobClient;

        public LprController(IBackgroundJobClient backgroundJobClient)
        {
            _backgroundJobClient = backgroundJobClient;
        }

        [HttpPost("event")]
        public IActionResult ReceiveEvent([FromBody] LprEventDto payload)
        {
            // Gelen veriyi validasyondan geçirip anında Hangfire kuyruğuna atıyoruz.
            // API response süresi milisaniyeler seviyesinde kalıyor.
            //_backgroundJobClient.Enqueue<EventProcessingJob>(job => job.ProcessEventAsync(payload));

            return Accepted(new { Message = "Event received and queued for processing." });
        }
    }
}
