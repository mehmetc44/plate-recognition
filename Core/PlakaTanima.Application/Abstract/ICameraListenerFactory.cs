
namespace PlakaTanima.Application.Abstract;
public interface ICameraListenerFactory
{
    ICameraAlarmListener GetListener(string brand);
}