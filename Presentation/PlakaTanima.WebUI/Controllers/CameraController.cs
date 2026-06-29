using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.Features.Cameras.Commands.CreateCamera;
using PlakaTanima.Application.Features.Cameras.Commands.DeleteCamera;
using PlakaTanima.Application.Features.Cameras.Commands.UpdateCamera;
using PlakaTanima.Application.Features.Cameras.Queries.GetCameraById;
using PlakaTanima.Application.Features.Cameras.Queries.GetCameraList;
using PlakaTanima.Application.Features.Locations.Commands;
using PlakaTanima.Application.Features.Locations.Commands.DeleteLocation;
using PlakaTanima.Application.Features.Locations.Commands.UpdateLocation;
using PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;

namespace PlakaTanima.WebUI.Controllers
{
    public class CameraController : Controller
    {
        private readonly IMediator _mediator;
        public CameraController (IMediator mediator)
        {
            _mediator = mediator;
        }

        // ========== JSON API: Tree verisi ==========
        [HttpGet]
        public async Task<IActionResult> GetTree()
        {
            var tree = await _mediator.Send(new GetLocationTreeQuery());
            var cameras = await _mediator.Send(new GetCameraListQuery());

            var result = tree.Select(loc => new
            {
                loc.Id,
                loc.Name,
                loc.Description,
                Cameras = cameras
                    .Where(c => loc.Cameras.Any(lc => lc.Id == c.Id))
                    .Select(c => new
                    {
                        c.Id,
                        c.Name,
                        c.IpAddress,
                        c.Port,
                        c.Username,
                        c.Password,
                        c.LocationName,
                        Status = c.Status.ToString().ToLower()
                    })
            });

            return Json(result);
        }

        // ========== JSON API: Kamera detay ==========
        [HttpGet]
        public async Task<IActionResult> GetCameraDetail(Guid id)
        {
            var detail = await _mediator.Send(new GetCameraByIdQuery(id));
            return Json(detail);
        }

        
        // ========== CAMERA CRUD (JSON) ==========
        [HttpPost]
        public async Task<IActionResult> AddCameraApi([FromBody] CreateCameraCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var id = await _mediator.Send(command);
                return Json(new { success = true, id, name = command.Name });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateCameraApi([FromBody] UpdateCameraCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                await _mediator.Send(command);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteCameraApi([FromBody] DeleteCameraCommand command)
        {
            try
            {
                await _mediator.Send(command);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        // ========== LOCATION CRUD (JSON API) ==========
        [HttpPost]
        public async Task<IActionResult> AddLocationApi([FromBody] CreateLocationCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                var id = await _mediator.Send(command);
                return Json(new { success = true, id, name = command.Name });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> UpdateLocationApi([FromBody] UpdateLocationCommand command)
        {
            if (!ModelState.IsValid)
                return Json(new { success = false, message = "Geçersiz veri." });
            try
            {
                await _mediator.Send(command);
                return Json(new { success = true });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public async Task<IActionResult> DeleteLocationApi([FromBody] DeleteLocationCommand command)
        {
            try
            {
                await _mediator.Send(command);
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
        public async Task<IActionResult> DeleteLocation(DeleteLocationCommand command)
        {
            await _mediator.Send(command);
            return Ok();
        }

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