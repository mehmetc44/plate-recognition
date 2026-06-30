using Microsoft.AspNetCore.Mvc;
namespace PlakaTanima.WebUI.Controllers;

public class HomeController : Controller
{
    public IActionResult Index()
    {
        return View();
    }

    [HttpGet("/login")]
    public IActionResult Login()
    {
        var token = Request.Cookies["access_token"];
        var refreshToken = Request.Cookies["refresh_token"];
        if (!string.IsNullOrEmpty(token) || !string.IsNullOrEmpty(refreshToken))
        {
            return RedirectToAction("Index", "Home");
        }
        return View("~/Views/Auth/Login/Index.cshtml");
    }

    [HttpGet("/PlateManagement")]
    public IActionResult PlateManagement()
    {
        return View("~/Views/PlateManagement/Index.cshtml");
    }
    [HttpGet("/AdvancedSearch")]
    public IActionResult AdvancedSearch()
    {
        return View("~/Views/AdvancedSearch/Index.cshtml");
    }
    [HttpGet("/VehicleDetails")]
    public IActionResult VehicleDetails()
    {
        return View("~/Views/VehicleDetails/Index.cshtml");
    }
    [HttpGet("/Settings")]
    public IActionResult Settings()
    {
        return View("~/Views/Settings/Index.cshtml");
    }
}
