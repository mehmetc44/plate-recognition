using MediatR;

namespace PlakaTanima.Application.Features.Cameras.Commands.UpdateCamera;

public sealed record UpdateCameraCommand(
    Guid Id,
    string Name,
    Guid LocationId,
    string IpAddress,
    int Port,
    string Username,
    string Password,
    int StreamChannel
) : IRequest<bool>;