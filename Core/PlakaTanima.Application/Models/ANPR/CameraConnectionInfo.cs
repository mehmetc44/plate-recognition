using System;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Application.Models.ANPR;

public sealed class CameraConnectionInfo
{
    public Guid CameraId { get; init; }

    public string CameraName { get; init; } = null!;

    public CameraStatus Status { get; set; }

    public DateTime LastActivityAt { get; set; }
}