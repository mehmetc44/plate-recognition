using Microsoft.AspNetCore.Mvc;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class CameraListViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}