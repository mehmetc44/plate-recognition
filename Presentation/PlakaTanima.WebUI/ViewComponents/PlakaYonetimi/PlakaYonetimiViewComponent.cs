using Microsoft.AspNetCore.Mvc;

namespace PlakaTanima.WebUI.ViewComponents.PlakaYonetimi
{
    public class PlakaYonetimiViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}
