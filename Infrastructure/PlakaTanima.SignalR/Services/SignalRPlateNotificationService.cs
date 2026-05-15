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

    public async Task SendNewPlateAsync(LprEventDto plateData)
    {
        // Client tarafına (JS) gönderilecek objeyi DTO'daki yeni alanlara göre genişletiyoruz
        await _hubContext.Clients.All.SendAsync("NewPlateDetected", new
        {
            EventId = plateData.EventId,
            Plate = plateData.Plate,
            Camera = plateData.CameraName,
            // UI'da güzel görünmesi için formatlıyoruz
            Timestamp = plateData.Timestamp.ToString("HH:mm:ss"),
            // Yüzdelik olarak göstermek isteyebilirsin (Örn: %98.5)
            Confidence = plateData.Confidence.ToString("F1"), 
            // Görüntüleri doğrudan aktarıyoruz
            PlateImage = plateData.Images?.Plate,
            VehicleImage = plateData.Images?.Vehicle
        });
    }
}