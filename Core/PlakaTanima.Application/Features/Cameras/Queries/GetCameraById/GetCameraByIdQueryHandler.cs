using MediatR;
using PlakaTanima.Application.Repositories.CameraRepositories;

namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraById;

public sealed class GetCameraByIdQueryHandler
    : IRequestHandler<GetCameraByIdQuery, CameraDetailDto>
{
    private readonly ICameraReadRepository _cameraReadRepository;

    public GetCameraByIdQueryHandler(
        ICameraReadRepository cameraReadRepository)
    {
        _cameraReadRepository = cameraReadRepository;
    }

    public async Task<CameraDetailDto> Handle(
        GetCameraByIdQuery request,
        CancellationToken cancellationToken)
    {
        var camera = await _cameraReadRepository
            .GetByIdAsync(request.Id);

        if (camera is null)
            throw new Exception("Kamera bulunamadı.");

        return new CameraDetailDto
        {
            Id = camera.Id,
            Name = camera.Name,
            LocationId = camera.LocationId,
            IpAddress = camera.IpAddress,
            Port = camera.Port,
            Username = camera.Username,
            Password = camera.Password,
            StreamChannel = camera.StreamChannel
        };
    }
}