using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class CameraListViewComponent : ViewComponent
    {
        private readonly ILocationRepository _locationRepository;

        public CameraListViewComponent(ILocationRepository locationRepository)
        {
            _locationRepository = locationRepository;
        }

        public async Task<IViewComponentResult> InvokeAsync()
        {
            var locations = await _locationRepository.GetAllWithCamerasAsync();
            return View(locations);
        }
    }
}