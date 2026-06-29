using System;
using System.Threading.Tasks;

namespace PlakaTanima.Application.Services
{
    public interface ILprProcessingJob
    {
        Task ProcessEventAsync(Guid eventId, string plate, string cameraName, string timestamp);
    }
}
