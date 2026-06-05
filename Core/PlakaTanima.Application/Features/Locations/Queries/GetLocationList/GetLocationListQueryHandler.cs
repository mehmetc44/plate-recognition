using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationList;

public sealed class GetLocationListQueryHandler
    : IRequestHandler<GetLocationListQuery,
        List<GetLocationListResponse>>
{
    private readonly ILocationReadRepository _locationReadRepository;

    public GetLocationListQueryHandler(
        ILocationReadRepository locationReadRepository)
    {
        _locationReadRepository = locationReadRepository;
    }

    public async Task<List<GetLocationListResponse>> Handle(
        GetLocationListQuery request,
        CancellationToken cancellationToken)
    {
        var locations =
            await _locationReadRepository.GetAllWithCamerasAsync();

        return locations.Select(x => new GetLocationListResponse
        {
            Id = x.Id,
            Name = x.Name,
            Description = x.Description,
            CameraCount = x.Cameras.Count,
            CreatedAt = x.CreatedAt
        }).ToList();
    }
}