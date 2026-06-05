using MediatR;
using PlakaTanima.Application.Repositories.CameraRepositories;
using PlakaTanima.Application.Repositories.LocationRepositories;

namespace PlakaTanima.Application.Features.Cameras.Commands.UpdateCamera;

public sealed class UpdateCameraCommandHandler
    : IRequestHandler<UpdateCameraCommand, bool>
{
    private readonly ICameraReadRepository _cameraReadRepository;
    private readonly ICameraWriteRepository _cameraWriteRepository;
    private readonly ILocationReadRepository _locationReadRepository;

    public UpdateCameraCommandHandler(
        ICameraReadRepository cameraReadRepository,
        ICameraWriteRepository cameraWriteRepository,
        ILocationReadRepository locationReadRepository)
    {
        _cameraReadRepository = cameraReadRepository;
        _cameraWriteRepository = cameraWriteRepository;
        _locationReadRepository = locationReadRepository;
    }

    public async Task<bool> Handle(
        UpdateCameraCommand request,
        CancellationToken cancellationToken)
    {
        var camera = await _cameraReadRepository
            .GetByIdAsync(request.Id);

        if (camera is null)
            throw new Exception("Kamera bulunamadı.");

        var location = await _locationReadRepository
            .GetByIdAsync(request.LocationId);

        if (location is null)
            throw new Exception("Lokasyon bulunamadı.");

        var ipExists = await _cameraReadRepository
            .ExistsByIpAddressAsync(
                request.IpAddress,
                request.Id);

        if (ipExists)
            throw new Exception(
                "Bu IP adresi başka bir kamerada kullanılıyor.");

        camera.Name = request.Name;
        camera.LocationId = request.LocationId;
        camera.IpAddress = request.IpAddress;
        camera.Port = request.Port;
        camera.Username = request.Username;
        camera.Password = request.Password;
        camera.StreamChannel = request.StreamChannel;
        camera.UpdatedAt = DateTime.UtcNow;

        _cameraWriteRepository.Update(camera);

        await _cameraWriteRepository.SaveAsync();

        return true;
    }
}