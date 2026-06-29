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

        // GetCameraListQueryHandler.cs içerisinde manuel eşleme yapıyorsanız:
var dtoList = cameras.Select(c => new CameraListDto
{
    Id = c.Id,
    Name = c.Name,
    IpAddress = c.IpAddress,
    LocationName = c.Location.Name,
    Status = c.Status,
    
    // EKLENMESİ GEREKEN ATAMALAR:
    Port = c.Port,
    Username = c.Username,
    Password = c.Password
}).ToList();

        return dtoList;
    }
}