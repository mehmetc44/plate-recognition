using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Domain.DTO;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class PlateStreamViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            return View();
        }
    }
}