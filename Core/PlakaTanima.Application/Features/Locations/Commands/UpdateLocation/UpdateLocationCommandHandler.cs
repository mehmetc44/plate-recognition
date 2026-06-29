using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.Application.Features.Locations.Commands.UpdateLocation;

public sealed class UpdateLocationCommandHandler
    : IRequestHandler<UpdateLocationCommand, bool>
{
    private readonly ILocationReadRepository _locationReadRepository;
    private readonly ILocationWriteRepository _locationWriteRepository;

    public UpdateLocationCommandHandler(
        ILocationReadRepository locationReadRepository,
        ILocationWriteRepository locationWriteRepository)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
    }

    public async Task<bool> Handle(
        UpdateLocationCommand request,
        CancellationToken cancellationToken)
    {
        var location =
            await _locationReadRepository.GetByIdAsync(request.Id);

        if (location is null)
            throw new Exception("Lokasyon bulunamadı.");

        var exists =
            await _locationReadRepository.ExistsByNameAsync(
                request.Name,
                request.Id);

        if (exists)
            throw new Exception("Bu isimde lokasyon mevcut.");

        location.Name = request.Name;
        location.Description = request.Description;
        location.UpdatedAt = DateTime.UtcNow;

        _locationWriteRepository.Update(location);

        await _locationWriteRepository.SaveAsync();

        return true;
    }
}