using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;

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

    public async Task<Guid> Handle(
        CreateLocationCommand request,
        CancellationToken cancellationToken)
    {
        throw new NotImplementedException();
    }
}