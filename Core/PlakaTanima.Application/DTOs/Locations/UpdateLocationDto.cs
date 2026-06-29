using System;

namespace PlakaTanima.Application.DTOs.Locations;

public sealed record UpdateLocationDto(
    Guid Id,
    string Name,
    string? Description
);
