using Microsoft.AspNetCore.Mvc;
namespace PlakaTanima.WebUI.Controllers;

public class AuthController : Controller
{
    [HttpGet("/Login")]
    public IActionResult Login()
    {
        return View("~/Views/Auth/Login/Index.cshtml");
    }
}
