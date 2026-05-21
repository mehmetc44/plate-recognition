namespace PlakaTanima.Application.Models;

public class CameraOptions
{
    public List<CameraConfig> Cameras { get; set; } = new();
}

public class CameraConfig
{
    public string Name { get; set; } = string.Empty;
    public string Ip { get; set; } = string.Empty;
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public string Brand { get; set; } = "Hikvision"; // Hikvision, Dahua, Axis vb.
}