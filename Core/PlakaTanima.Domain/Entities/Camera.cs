using System;

namespace PlakaTanima.Domain.Entities;
public class Camera
{
    public string Name { get; set; } = default!;
    public string Type { get; set; } = default!; // Hikvision, Dahua, RTSP
    public string Ip { get; set; } = default!;
    public string Username { get; set; } = default!;
    public string Password { get; set; } = default!;
}