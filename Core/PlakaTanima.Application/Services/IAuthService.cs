using System.Threading.Tasks;
using PlakaTanima.Application.DTOs.Auth;

namespace PlakaTanima.Application.Services
{
    public interface IAuthService
    {
        Task<AuthResultDto> LoginAsync(LoginDto dto);
        Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken);
        Task RevokeTokenAsync(string refreshToken);
    }
}
