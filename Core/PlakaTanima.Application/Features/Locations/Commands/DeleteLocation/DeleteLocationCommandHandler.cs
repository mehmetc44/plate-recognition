using MediatR;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.Application.Features.Locations.Commands.DeleteLocation;

public sealed class DeleteLocationCommandHandler
    : IRequestHandler<DeleteLocationCommand, bool>
{
    private readonly ILocationReadRepository _locationReadRepository;
    private readonly ILocationWriteRepository _locationWriteRepository;

    public DeleteLocationCommandHandler(
        ILocationReadRepository locationReadRepository,
        ILocationWriteRepository locationWriteRepository)
    {
        _locationReadRepository = locationReadRepository;
        _locationWriteRepository = locationWriteRepository;
    }

    public async Task<bool> Handle(
        DeleteLocationCommand request,
        CancellationToken cancellationToken)
    {
        var location =
            await _locationReadRepository.GetByIdAsync(request.Id);

        if (location is null)
            throw new Exception("Lokasyon bulunamadı.");

        var hasCamera =
            await _locationReadRepository.HasCameraAsync(request.Id);

        if (hasCamera)
            throw new Exception(
                "Bu lokasyona bağlı kamera bulunduğu için silinemez.");

        _locationWriteRepository.Remove(location);

        await _locationWriteRepository.SaveAsync();

        return true;
    }
}