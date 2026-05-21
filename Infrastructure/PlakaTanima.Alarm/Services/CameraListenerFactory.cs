using PlakaTanima.Application.Abstract;

namespace PlakaTanima.Alarm.Services;
public class CameraListenerFactory : ICameraListenerFactory
{
    private readonly IEnumerable<ICameraAlarmListener> _listeners;

    public CameraListenerFactory(IEnumerable<ICameraAlarmListener> listeners)
    {
        _listeners = listeners;
    }

    public ICameraAlarmListener GetListener(string brand)
    {
        var listener = _listeners.FirstOrDefault(x => x.Brand.Equals(brand, StringComparison.OrdinalIgnoreCase));
        
        if (listener == null)
            throw new NotSupportedException($"'{brand}' markası için bir kamera dinleyicisi bulunamadı!");
            
        return listener;
    }
}