using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.Features.Cameras.Commands.CreateCamera;
using PlakaTanima.Application.Features.Cameras.Commands.DeleteCamera;
using PlakaTanima.Application.Features.Cameras.Commands.UpdateCamera;
using PlakaTanima.Application.Features.Locations.Commands;
using PlakaTanima.Application.Features.Locations.Commands.DeleteLocation;
using PlakaTanima.Application.Features.Locations.Commands.UpdateLocation;

namespace PlakaTanima.WebUI.Controllers
{
    public class SettingsController : Controller
    {
        private readonly IMediator _mediator;
        public SettingsController(IMediator mediator)
        {
            _mediator = mediator;
        }

        public IActionResult Index()
        {
            return View();
        }

        // ========== LOCATION CRUD ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddLocation(CreateLocationCommand command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateLocation(UpdateLocationCommand command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteLocation(DeleteLocationCommand command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        // ========== CAMERA CRUD ==========
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> AddCamera(CreateCameraCommand command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> UpdateCamera(UpdateCameraCommand command)
        {
            if (!ModelState.IsValid)
                return RedirectToAction(nameof(Index));
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteCamera(DeleteCameraCommand command)
        {
            await _mediator.Send(command);
            return RedirectToAction(nameof(Index));
        }
    }
}