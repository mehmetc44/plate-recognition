using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Auth;

namespace PlakaTanima.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto dto);
    }
}
