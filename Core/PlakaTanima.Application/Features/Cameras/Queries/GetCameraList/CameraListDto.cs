using PlakaTanima.Domain.Enums;
namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraList;
public sealed class CameraListDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string IpAddress { get; set; } = string.Empty;

    public string LocationName { get; set; } = string.Empty;

    public CameraStatus Status { get; set; }
}