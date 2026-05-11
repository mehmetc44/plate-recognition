using System;
using PlakaTanima.Domain.DTO;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.WebUI.Models;

public class LiveViewModel
{
    public List<CameraDto> cameras;
    public List<Vehicle> vehicles; 
}
