using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.WebUI.Models;

namespace PlakaTanima.WebUI.Controllers;

public class GalleryController : Controller
{
    private readonly ILogger<GalleryController> _logger;

    public GalleryController(ILogger<GalleryController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        
        return View();
    }
}
