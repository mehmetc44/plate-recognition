namespace PlakaTanima.Application.DTOs.Locations;

public sealed record CreateLocationDto(
    string Name,
    string? Description
);
