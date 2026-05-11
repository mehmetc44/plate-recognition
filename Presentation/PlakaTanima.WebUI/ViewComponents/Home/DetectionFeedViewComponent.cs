using Microsoft.AspNetCore.Mvc;

namespace PlakaTanima.WebUI.ViewComponents.Home
{
    public class DetectionFeedViewComponent : ViewComponent
    {
        public IViewComponentResult Invoke()
        {
            var model = new List<string>
            {
                "34ABC123 - Gate 1",
                "06XYZ999 - Gate 2"
            };

            return View(model);
        }
    }
}