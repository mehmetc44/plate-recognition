using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.WebUI.Models;

namespace PlakaTanima.WebUI.Controllers;

public class SearchController : Controller
{
    private readonly ILogger<SearchController> _logger;

    public SearchController(ILogger<SearchController> logger)
    {
        _logger = logger;
    }

    public IActionResult Index()
    {
        
        return View();
    }
}
