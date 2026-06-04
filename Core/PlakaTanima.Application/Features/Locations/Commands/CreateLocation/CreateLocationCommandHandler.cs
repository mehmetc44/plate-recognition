using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;

namespace PlakaTanima.Application.Features.Locations.Commands.CreateLocation;

public class CreateLocationCommandHandler
    : IRequestHandler<CreateLocationCommand, Guid>
{
    private readonly ILocationReadRepository _locationReadRepository;
    private readonly ILocationWriteRepository _locationWriteRepository;

    public CreateLocationCommandHandler(
        ILocationReadRepository locationReadRepository,
        ILocationWriteRepository locationWriteRepository)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
    }

    public async Task<Guid> Handle(CreateLocationCommand request, CancellationToken cancellationToken)
    {
        var location = new Location
        {
            Id = Guid.NewGuid(),
            Name = request.Name,
            Description = request.Description,
            CreatedAt = DateTime.UtcNow,
        };

        await _locationWriteRepository.AddAsync(location);
        await _locationWriteRepository.SaveAsync();

        return location.Id;
    }
}