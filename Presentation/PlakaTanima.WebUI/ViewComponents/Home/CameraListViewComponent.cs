using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class CameraListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke(List<CameraDto> cameras)
        {
            return View(cameras);
        }
    }
}