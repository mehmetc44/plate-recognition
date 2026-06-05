using System;
namespace PlakaTanima.Application.Features.Cameras.Queries.GetCameraById;


public sealed class CameraDetailDto
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public Guid LocationId { get; set; }

    public string IpAddress { get; set; } = string.Empty;

    public int Port { get; set; }

    public string Username { get; set; } = string.Empty;

    public string Password { get; set; } = string.Empty;

    public int StreamChannel { get; set; }
}