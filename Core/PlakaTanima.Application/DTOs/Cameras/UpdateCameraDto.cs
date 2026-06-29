using System;

namespace PlakaTanima.Application.DTOs.Cameras;

public sealed record UpdateCameraDto(
    Guid Id,
    string Name,
    Guid LocationId,
    string IpAddress,
    int Port,
    string Username,
    string Password,
    int StreamChannel
);
