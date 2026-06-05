using MediatR;
using PlakaTanima.Application.Repositories.CameraRepositories;

namespace PlakaTanima.Application.Features.Cameras.Commands.DeleteCamera;
public sealed class DeleteCameraCommandHandler
    : IRequestHandler<DeleteCameraCommand, bool>
{
    private readonly ICameraReadRepository _cameraReadRepository;
    private readonly ICameraWriteRepository _cameraWriteRepository;

    public DeleteCameraCommandHandler(
        ICameraReadRepository cameraReadRepository,
        ICameraWriteRepository cameraWriteRepository)
    {
        _cameraReadRepository = cameraReadRepository;
        _cameraWriteRepository = cameraWriteRepository;
    }

    public async Task<bool> Handle(
        DeleteCameraCommand request,
        CancellationToken cancellationToken)
    {
        var camera =
            await _cameraReadRepository.GetByIdAsync(request.Id);

        if(camera is null)
            throw new Exception("Kamera bulunamadı.");

        _cameraWriteRepository.Remove(camera);

        await _cameraWriteRepository.SaveAsync();

        return true;
    }
}