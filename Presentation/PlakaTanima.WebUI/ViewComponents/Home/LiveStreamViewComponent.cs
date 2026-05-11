using Microsoft.AspNetCore.Mvc;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class LiveStreamViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}