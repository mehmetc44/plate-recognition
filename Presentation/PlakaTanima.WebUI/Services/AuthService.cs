using Microsoft.AspNetCore.Identity;
using Microsoft.IdentityModel.Tokens;
using PlakaTanima.Application.DTOs.Auth;
using PlakaTanima.Application.Services;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;
using System.Threading.Tasks;

namespace PlakaTanima.WebUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager)
        {
            _userManager = userManager;
            _signInManager = signInManager;
        }

        public async Task<AuthResultDto> LoginAsync(LoginDto dto)
        {
            if (dto == null || string.IsNullOrWhiteSpace(dto.Email) || string.IsNullOrWhiteSpace(dto.Password))
            {
                return new AuthResultDto { Success = false, Message = "E-posta ve şifre zorunludur." };
            }

            var user = await _userManager.FindByEmailAsync(dto.Email);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Message = "E-posta veya şifre hatalı." };
            }

            var result = await _signInManager.CheckPasswordSignInAsync(user, dto.Password, false);
            if (!result.Succeeded)
            {
                return new AuthResultDto { Success = false, Message = "E-posta veya şifre hatalı." };
            }

            // Generate JWT Token
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "PlatarSecretSecurityKeyThatNeedsToBeLongEnoughForHMACSHA256";
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PlatarIssuer";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PlatarAudience";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var expiration = DateTime.UtcNow.AddHours(3);

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            var tokenString = new JwtSecurityTokenHandler().WriteToken(token);

            return new AuthResultDto
            {
                Success = true,
                Token = tokenString,
                Expiration = expiration
            };
        }
    }
}
