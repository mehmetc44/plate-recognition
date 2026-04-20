using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.WebUI.Models;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;
namespace PlakaTanima.WebUI.Controllers;

public class HomeController : Controller
{
    private readonly ILogger<HomeController> _logger;
    private List<Camera> cameras;
    private List<Vehicle> vehicles;


    public HomeController(ILogger<HomeController> logger)
    {
        cameras = new List<Camera>(){
            new Camera(){IP="192.168.1.100", Name="Kamera 1", viewLink="~/img/kamera1.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.101", Name="Kamera 2", viewLink="~/img/kamera2.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.102", Name="Kamera 3", viewLink="~/img/kamera3.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.103", Name="Kamera 4", viewLink="~/img/kamera4.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.104", Name="Kamera 5", viewLink="~/img/kamera5.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.105", Name="Kamera 6", viewLink="~/img/kamera6.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.106", Name="Kamera 7", viewLink="~/img/kamera7.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.107", Name="Kamera 8", viewLink="~/img/kamera8.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.108", Name="Kamera 9", viewLink="~/img/kamera9.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.109", Name="Kamera 10", viewLink="~/img/kamera10.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.110", Name="Kamera 11", viewLink="~/img/kamera11.jpg", username="admin", password="password"},
            new Camera(){IP="192.168.1.111", Name="Kamera 12", viewLink="~/img/kamera12.jpg", username="admin", password="password"}
        };
        vehicles = new List<Vehicle>(){
            new Vehicle(){PlateNumber="06ABC123", Color="Beyaz", Model="Civic", Brand="Honda", FirstSeen=DateTime.Now, PlateType=VehiclePlateType.NORMAL},
            new Vehicle(){PlateNumber="34DEF456", Color="Siyah", Model="Corolla", Brand="Toyota", FirstSeen=DateTime.Now, PlateType=VehiclePlateType.VIP},
            new Vehicle(){PlateNumber="06GHI789", Color="Gri", Model="Camry", Brand="Toyota", FirstSeen=DateTime.Now, PlateType=VehiclePlateType.BLACK_LIST}
        };

        _logger = logger;
    }




    public IActionResult Index()
    {
        LiveViewModel model = new LiveViewModel(){
            cameras = cameras,
            vehicles = vehicles
        };
        return View(model);
    }
}
