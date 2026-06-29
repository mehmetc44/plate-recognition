using System.Collections.Generic;
using PlakaTanima.Application.DTOs.Cameras;
using PlakaTanima.Application.DTOs.Locations;

namespace PlakaTanima.WebUI.Models;

public class SettingsPageViewModel
{
    public List<LocationDto> LocationTree { get; set; } = new();
    public List<CameraDto> AllCameras { get; set; } = new();
}