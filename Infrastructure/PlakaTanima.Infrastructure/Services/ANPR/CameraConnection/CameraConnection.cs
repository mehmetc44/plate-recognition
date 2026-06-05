using System;
using PlakaTanima.Application.Abstractions.Services.ANPR.CameraConnection;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Domain.Enums;

namespace PlakaTanima.Infrastructure.Services.ANPR.CameraConnection;

public class CameraConnection : ICameraConnection
{
    public Guid CameraId { get; private set; }

    public CameraStatus Status { get; private set; }

    public Task StartAsync(
        Camera camera,
        CancellationToken cancellationToken)
    {
        CameraId = camera.Id;

        Status = CameraStatus.Online;

        return Task.CompletedTask;
    }

    public Task StopAsync(
        CancellationToken cancellationToken)
    {
        Status = CameraStatus.Offline;

        return Task.CompletedTask;
    }
}