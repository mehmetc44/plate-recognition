using PlakaTanima.Application.Features.Cameras.Queries.GetCameraList;
using PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;

namespace PlakaTanima.WebUI.Models;

public class SettingsPageViewModel
{
    public List<LocationTreeDto> LocationTree { get; set; } = new();
    public List<CameraListDto> AllCameras { get; set; } = new();
}