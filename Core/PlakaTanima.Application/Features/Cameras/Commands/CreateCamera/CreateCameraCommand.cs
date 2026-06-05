using MediatR;

namespace PlakaTanima.Application.Features.Cameras.Commands.CreateCamera;

public sealed record CreateCameraCommand(
    string Name,
    Guid LocationId,
    string IpAddress,
    int Port,
    string Username,
    string Password,
    int StreamChannel
) : IRequest<Guid>;