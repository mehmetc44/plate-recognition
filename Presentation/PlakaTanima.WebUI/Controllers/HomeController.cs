using Microsoft.AspNetCore.Mvc;
namespace PlakaTanima.WebUI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }
}
