using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.WebUI.Models;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
using PlakaTanima.Application.Abstract.Services;
using System.Text;
using PlakaTanima.Domain.DTO;
namespace PlakaTanima.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private List<CameraDto> cameras;
    //private List<Vehicle> vehicles;


    public HomeController(ILogger<HomeController> logger)
    {
        cameras = new List<CameraDto>
        {
            new CameraDto { Name = "Kamera 1", IP = "10.10.155.77:554", StreamId = "cam1" },
            new CameraDto { Name = "Kamera 2", IP = "10.10.155.78:554", StreamId = "cam2" },
        };


        _logger = logger;
    }

    public IActionResult Index()
    {
        LiveViewModel model = new LiveViewModel(){
            cameras = cameras,
        };
        return View(model);
    }
}
