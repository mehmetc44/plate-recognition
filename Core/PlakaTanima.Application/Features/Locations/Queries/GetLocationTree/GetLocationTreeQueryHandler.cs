using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;

public sealed class GetLocationTreeQueryHandler
    : IRequestHandler<GetLocationTreeQuery,
        List<LocationTreeDto>>
{
    private readonly ILocationReadRepository _locationReadRepository;

    public GetLocationTreeQueryHandler(
        ILocationReadRepository locationReadRepository)
    {
        _locationReadRepository = locationReadRepository;
    }

    public async Task<List<LocationTreeDto>> Handle(
        GetLocationTreeQuery request,
        CancellationToken cancellationToken)
    {
        var locations =
            await _locationReadRepository.GetAllWithCamerasAsync();

        return locations.Select(x => new LocationTreeDto
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,   // Eklendi
            Cameras = x.Cameras.Select(c => new CameraTreeDto
            {
                Id = c.Id,
                Name = c.Name,
                IpAddress = c.IpAddress,
                Port = c.Port,
                Username = c.Username,
                Password = c.Password,
                StreamChannel = c.StreamChannel,
                LocationId = c.LocationId
            }).ToList()
        }).ToList();
    }
}