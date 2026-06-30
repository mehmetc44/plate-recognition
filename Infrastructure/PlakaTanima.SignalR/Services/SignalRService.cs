using System.Threading.Tasks;
using Microsoft.AspNetCore.SignalR;
using PlakaTanima.Application.Services;
using PlakaTanima.SignalR.Hubs;

namespace PlakaTanima.SignalR.Services
{
    public class SignalRService : ISignalRService
    {
        private readonly IHubContext<PlateHub> _hubContext;

        public SignalRService(IHubContext<PlateHub> hubContext)
        {
            _hubContext = hubContext;
        }

        public async Task SendPlateDetectedAsync(
            string plate,
            string camera,
            string direction,
            string fullTime,
            string imgVehicle,
            string category,
            string owner,
            string model)
        {
            await _hubContext.Clients.All.SendAsync("NewPlateDetected", new
            {
                plate,
                camera,
                direction,
                fullTime,
                imgVehicle,
                category,
                owner,
                model
            });
        }
    }
}
