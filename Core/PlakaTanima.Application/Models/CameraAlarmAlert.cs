namespace PlakaTanima.Application.Models;

public class CameraAlarmAlert
{
    public string CameraName { get; set; } = string.Empty;
    public string PlateNumber { get; set; } = string.Empty;
    public DateTime EventTime { get; set; }
    public string VehicleType { get; set; } = "Unknown";
    public string VehicleColor { get; set; } = "Unknown";
    public string VehicleBrand { get; set; } = "Unknown";
    public string MovingDirection { get; set; } = "Unknown";
    public string Country { get; set; } = "Turkey";
    public Dictionary<string, string> RawMetadata { get; set; } = new();
}