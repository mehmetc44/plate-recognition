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
                Name = c.Name
            }).ToList()
        }).ToList();
    }
}