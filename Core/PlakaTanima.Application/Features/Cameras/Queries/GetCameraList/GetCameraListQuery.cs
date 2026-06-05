using MediatR;

namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraList;

public sealed record GetCameraListQuery()
    : IRequest<List<CameraListDto>>;