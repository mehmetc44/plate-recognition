using System.Threading.Tasks;

namespace PlakaTanima.Application.Services
{
    public interface IMinioStorageService
    {
        Task<string> GetPresignedUrlAsync(string objectPath, int expirySeconds = 900);
    }
}
