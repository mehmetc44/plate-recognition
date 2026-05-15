using PlakaTanima.Domain.DTO;

namespace PlakaTanima.Application.Abstract.Services;

public interface IPlateNotificationService
{
    Task SendNewPlateAsync(LprEventDto plateData);
}