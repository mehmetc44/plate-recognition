using System;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.DTOs.Cameras;

public sealed class CameraDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public Guid LocationId { get; set; }
    public string? LocationName { get; set; }
    public string IpAddress { get; set; } = string.Empty;
    public int Port { get; set; }
    public string Username { get; set; } = string.Empty;
    public string Password { get; set; } = string.Empty;
    public int StreamChannel { get; set; }
    public string Status { get; set; } = string.Empty;
}
