using MediatR;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.Features.Cameras.Queries.GetCameraById;
using PlakaTanima.Application.Features.Locations.Queries.GetLocationList;
using PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;
using System.Threading.Tasks;

namespace PlakaTanima.WebUI.ViewComponents.Settings
{
    public class CameraSettingsViewComponent : ViewComponent
    {
        private readonly IMediator _mediator;

        public CameraSettingsViewComponent(IMediator mediator)
        {
            _mediator = mediator;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            return View();
        }
    }
}