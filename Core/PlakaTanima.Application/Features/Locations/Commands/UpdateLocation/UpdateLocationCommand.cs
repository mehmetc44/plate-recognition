using MediatR;

namespace PlakaTanima.Application.Features.Locations.Commands.UpdateLocation;

public sealed record UpdateLocationCommand(
    Guid Id,
    string Name,
    string? Description
) : IRequest<bool>;