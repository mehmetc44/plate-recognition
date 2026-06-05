using MediatR;
namespace PlakaTanima.Application.Features.Locations.Queries.GetLocationTree;

public sealed record GetLocationTreeQuery()
    : IRequest<List<LocationTreeDto>>;