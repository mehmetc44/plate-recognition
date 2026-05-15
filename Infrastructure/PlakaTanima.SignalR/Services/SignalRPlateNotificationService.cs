using Microsoft.AspNetCore.SignalR;
using PlakaTanima.Application.Abstract.Services;
using PlakaTanima.Domain.DTO;
using PlakaTanima.SignalR.Hubs;

namespace PlakaTanima.SignalR.Services;

public class SignalRPlateNotificationService : IPlateNotificationService
{
    private readonly IHubContext<PlateHub> _hubContext;

    public SignalRPlateNotificationService(IHubContext<PlateHub> hubContext)
    {
        _hubContext = hubContext;
    }

    public async Task SendNewPlateAsync(LprEventDto data)
{
    await _hubContext.Clients.All.SendAsync("NewPlateDetected", new {
        plate = data.Plate,
        camera = data.CameraName,
        time = data.Timestamp.ToString("HH:mm:ss"),
        fullTime = data.Timestamp.ToString("yyyy-MM-dd HH:mm:ss"),
        vType = data.VehicleType,
        vColor = data.VehicleColor,
        vBrand = data.VehicleBrand,
        dir = data.Direction,
        // Resimler
        imgPlate = data.Images?.Plate,
        imgVehicle = data.Images?.Vehicle,
        imgFull = data.Images?.Full
    });
}
}