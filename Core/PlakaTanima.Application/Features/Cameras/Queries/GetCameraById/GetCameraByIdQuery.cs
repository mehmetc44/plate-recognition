using MediatR;

namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraById;
public sealed record GetCameraByIdQuery(
    Guid Id
) : IRequest<CameraDetailDto>;