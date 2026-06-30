using System.Threading.Tasks;

namespace PlakaTanima.Application.Services
{
    public interface ISignalRService
    {
        Task SendPlateDetectedAsync(
            string plate,
            string camera,
            string direction,
            string fullTime,
            string imgVehicle,
            string category,
            string owner,
            string model);
    }
}
