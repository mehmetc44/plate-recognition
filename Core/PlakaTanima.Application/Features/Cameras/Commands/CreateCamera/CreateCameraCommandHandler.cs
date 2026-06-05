using MediatR;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Repositories.LocationRepositories;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.Features.Cameras.Commands.CreateCamera;

public sealed class CreateCameraCommandHandler
    : IRequestHandler<CreateCameraCommand, Guid>
{
    private readonly ICameraReadRepository _cameraReadRepository;
    private readonly ICameraWriteRepository _cameraWriteRepository;

    private readonly ILocationReadRepository _locationReadRepository;

    public CreateCameraCommandHandler(
        ICameraReadRepository cameraReadRepository,
        ICameraWriteRepository cameraWriteRepository,
        ILocationReadRepository locationReadRepository)
    {
        _cameraReadRepository = cameraReadRepository;
        _cameraWriteRepository = cameraWriteRepository;
        _locationReadRepository = locationReadRepository;
    }

    public async Task<Guid> Handle(
        CreateCameraCommand request,
        CancellationToken cancellationToken)
    {
        var location =
            await _locationReadRepository
                .GetByIdAsync(request.LocationId);

        if (location is null)
            throw new Exception("Lokasyon bulunamadı.");

        var exists =
            await _cameraReadRepository
                .ExistsByIpAddressAsync(
                    request.IpAddress);

        if (exists)
            throw new Exception(
                "Bu IP adresi ile kayıtlı kamera mevcut.");

        var camera = new Camera
        {
            Name = request.Name,

            LocationId = request.LocationId,

            IpAddress = request.IpAddress,

            Port = request.Port,

            Username = request.Username,

            Password = request.Password,

            StreamChannel = request.StreamChannel,

            Status = CameraStatus.Offline
        };

        await _cameraWriteRepository.AddAsync(camera);

        await _cameraWriteRepository.SaveAsync();

        return camera.Id;
    }
}