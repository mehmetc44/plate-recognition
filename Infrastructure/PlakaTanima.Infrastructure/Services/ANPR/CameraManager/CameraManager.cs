using System;
using System.Collections.Concurrent;
using PlakaTanima.Application.Abstractions.Services.ANPR.CameraConnection;

namespace PlakaTanima.Infrastructure.Services.ANPR.CameraManager;

public class CameraManager
{
    private readonly ConcurrentDictionary<Guid, ICameraConnection>
    _connections = new();
}
