using System;

namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;


public sealed class LocationTreeDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public List<CameraTreeDto> Cameras { get; set; }
        = new();
}
