using Microsoft.AspNetCore.Mvc;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class LiveTableStreamViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}