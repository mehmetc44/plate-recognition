using System.Threading.Channels;
using PlakaTanima.Application.Abstract;
using PlakaTanima.Application.Models;

namespace PlakaTanima.Alarm.Services;

public class AlarmQueue : IAlarmQueue
{
    private readonly Channel<CameraAlarmAlert> _channel;

    public AlarmQueue()
    {
        var options = new BoundedChannelOptions(5000)
        {
            SingleWriter = false, // Birden fazla kamera akışı aynı anda yazabilir
            SingleReader = true,  // Arka plandaki işçi (Worker) sırayla tek kanaldan tüketecek
            FullMode = BoundedChannelFullMode.Wait
        };

        _channel = Channel.CreateBounded<CameraAlarmAlert>(options);
    }

    public async ValueTask WriteAsync(CameraAlarmAlert alert, CancellationToken cancellationToken = default)
    {
        await _channel.Writer.WriteAsync(alert, cancellationToken);
    }

    public IAsyncEnumerable<CameraAlarmAlert> ReadAllAsync(CancellationToken cancellationToken = default)
    {
        return _channel.Reader.ReadAllAsync(cancellationToken);
    }
}