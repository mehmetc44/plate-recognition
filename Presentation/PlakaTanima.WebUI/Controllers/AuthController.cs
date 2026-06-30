using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Http;
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

        public AuthController(IAuthService _authService)
        {
            this._authService = _authService;
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

            // Set Access Token Cookie (1 Hour)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = result.Expiration
            };
            Response.Cookies.Append("access_token", result.Token ?? "", cookieOptions);

            // Set Refresh Token Cookie (5 Days)
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = result.RefreshTokenExpiration
            };
            Response.Cookies.Append("refresh_token", result.RefreshToken ?? "", refreshCookieOptions);

            return Ok(new
            {
                success = true,
                token = result.Token,
                expiration = result.Expiration
            });
        }

        [HttpPost("refresh")]
        public async Task<IActionResult> Refresh()
        {
            var accessToken = Request.Cookies["access_token"];
            var refreshToken = Request.Cookies["refresh_token"];

            if (string.IsNullOrEmpty(accessToken) || string.IsNullOrEmpty(refreshToken))
            {
                return Unauthorized(new { success = false, message = "Eksik token bilgileri." });
            }

            var result = await _authService.RefreshTokenAsync(accessToken, refreshToken);
            if (!result.Success)
            {
                // Clear invalid tokens
                Response.Cookies.Delete("access_token");
                Response.Cookies.Delete("refresh_token");
                return Unauthorized(new { success = false, message = result.Message });
            }

            // Set New Access Token Cookie (1 Hour)
            var cookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = result.Expiration
            };
            Response.Cookies.Append("access_token", result.Token ?? "", cookieOptions);

            // Set New Refresh Token Cookie (5 Days)
            var refreshCookieOptions = new CookieOptions
            {
                HttpOnly = true,
                Secure = Request.IsHttps,
                SameSite = SameSiteMode.Lax,
                Expires = result.RefreshTokenExpiration
            };
            Response.Cookies.Append("refresh_token", result.RefreshToken ?? "", refreshCookieOptions);

            return Ok(new
            {
                success = true,
                token = result.Token,
                expiration = result.Expiration
            });
        }

        [HttpPost("logout")]
        public async Task<IActionResult> Logout()
        {
            var refreshToken = Request.Cookies["refresh_token"];
            if (!string.IsNullOrEmpty(refreshToken))
            {
                await _authService.RevokeTokenAsync(refreshToken);
            }

            Response.Cookies.Delete("access_token");
            Response.Cookies.Delete("refresh_token");

            return Ok(new { success = true, message = "Başarıyla çıkış yapıldı." });
        }
    }
}
