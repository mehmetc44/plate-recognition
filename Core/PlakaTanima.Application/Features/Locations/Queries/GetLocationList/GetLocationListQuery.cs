using MediatR;

namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationList;

public sealed record GetLocationListQuery()
    : IRequest<List<GetLocationListResponse>>;