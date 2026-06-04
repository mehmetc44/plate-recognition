using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.Features.Locations.Commands;

namespace PlakaTanima.WebUI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IMediator _mediator;
        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }
        public ActionResult Index()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> AddLocation([FromBody] CreateLocationCommand command)
        {
            var id = await _mediator.Send(command);
            return Ok(id);
        }

    }
}
