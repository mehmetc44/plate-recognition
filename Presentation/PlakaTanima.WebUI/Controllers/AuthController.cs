using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using PlakaTanima.Application.DTOs.Auth;
using PlakaTanima.Application.Services;

namespace PlakaTanima.WebUI.Controllers
{
    [ApiController]
    [Route("api/auth")]
    public class AuthController : ControllerBase
    {
        private readonly IAuthService _authService;

        public AuthController(IAuthService authService)
        {
            _authService = authService;
        }

        [HttpPost("login")]
        public async Task<IActionResult> Login([FromBody] LoginDto request)
        {
            var result = await _authService.LoginAsync(request);
            if (!result.Success)
            {
                if (result.Message == "E-posta ve şifre zorunludur.")
                {
                    return BadRequest(new { success = false, message = result.Message });
                }
                return Unauthorized(new { success = false, message = result.Message });
            }

            return Ok(new
            {
                success = true,
                token = result.Token,
                expiration = result.Expiration
            });
        }
    }
}
