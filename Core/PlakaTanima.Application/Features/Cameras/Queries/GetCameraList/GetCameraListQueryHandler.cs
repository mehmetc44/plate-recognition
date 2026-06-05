using MediatR;
using PlakaTanima.Application.Repositories.CameraRepositories;

namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraList;
public sealed class GetCameraListQueryHandler
    : IRequestHandler<GetCameraListQuery,
        List<CameraListDto>>
{
    private readonly ICameraReadRepository _cameraReadRepository;

    public GetCameraListQueryHandler(
        ICameraReadRepository cameraReadRepository)
    {
        _cameraReadRepository = cameraReadRepository;
    }

    public async Task<List<CameraListDto>> Handle(
        GetCameraListQuery request,
        CancellationToken cancellationToken)
    {
        var cameras =
            await _cameraReadRepository
                .GetAllWithLocationAsync();

        return cameras.Select(x => new CameraListDto
        {
            Id = x.Id,

            Name = x.Name,

            IpAddress = x.IpAddress,

            LocationName = x.Location.Name,

            Status = x.Status
        }).ToList();
    }
}