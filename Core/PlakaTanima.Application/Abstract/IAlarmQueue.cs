using PlakaTanima.Application.Models;

namespace PlakaTanima.Application.Abstract;

public interface IAlarmQueue
{
    ValueTask WriteAsync(CameraAlarmAlert alert, CancellationToken cancellationToken = default);
    IAsyncEnumerable<CameraAlarmAlert> ReadAllAsync(CancellationToken cancellationToken = default);
}