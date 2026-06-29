using System;
using System.Collections.Generic;
using PlakaTanima.Application.DTOs.Cameras;

namespace PlakaTanima.Application.DTOs.Locations;

public sealed class LocationDto
{
    public Guid Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public string? Description { get; set; }
    public List<CameraDto> Cameras { get; set; } = new();
}
