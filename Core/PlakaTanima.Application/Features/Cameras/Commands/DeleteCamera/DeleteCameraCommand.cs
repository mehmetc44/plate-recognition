using MediatR;

namespace PlakaTanima.Application.Features.Cameras.Commands.DeleteCamera;

public sealed record DeleteCameraCommand(
    Guid Id
) : IRequest<bool>;