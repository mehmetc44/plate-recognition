using MediatR;

namespace PlakaTanima.Application.Features.Locations.Commands.DeleteLocation;

public sealed record DeleteLocationCommand(
    Guid Id
) : IRequest<bool>;