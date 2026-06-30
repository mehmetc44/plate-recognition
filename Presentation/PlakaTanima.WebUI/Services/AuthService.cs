using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using PlakaTanima.Application.DTOs.Auth;
using PlakaTanima.Application.Services;
using PlakaTanima.Domain.Entities;
using PlakaTanima.Persistence.Contexts;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Threading.Tasks;

namespace PlakaTanima.WebUI.Services
{
    public class AuthService : IAuthService
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly SignInManager<IdentityUser> _signInManager;
        private readonly AppDbContext _context;

        public AuthService(
            UserManager<IdentityUser> userManager,
            SignInManager<IdentityUser> signInManager,
            AppDbContext context)
        {
            _userManager = userManager;
            _signInManager = signInManager;
            _context = context;
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

            // Generate Access Token (1 Hour)
            var expiration = DateTime.UtcNow.AddHours(1);
            var tokenString = GenerateAccessToken(user, expiration);

            // Generate Refresh Token (5 Days)
            var refreshTokenString = GenerateRefreshTokenString();
            var refreshTokenExpiration = DateTime.UtcNow.AddDays(5);

            var refreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = refreshTokenString,
                Expiration = refreshTokenExpiration,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserRefreshTokens.AddAsync(refreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                Success = true,
                Token = tokenString,
                Expiration = expiration,
                RefreshToken = refreshTokenString,
                RefreshTokenExpiration = refreshTokenExpiration
            };
        }

        public async Task<AuthResultDto> RefreshTokenAsync(string accessToken, string refreshToken)
        {
            var principal = GetPrincipalFromExpiredToken(accessToken);
            if (principal == null)
            {
                return new AuthResultDto { Success = false, Message = "Geçersiz erişim token'ı." };
            }

            var userId = principal.FindFirst(ClaimTypes.NameIdentifier)?.Value 
                         ?? principal.FindFirst(JwtRegisteredClaimNames.Sub)?.Value;

            if (string.IsNullOrEmpty(userId))
            {
                return new AuthResultDto { Success = false, Message = "Token içeriği geçersiz." };
            }

            var tokenRecord = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && x.UserId == userId && !x.IsRevoked);

            if (tokenRecord == null)
            {
                return new AuthResultDto { Success = false, Message = "Geçersiz refresh token." };
            }

            if (tokenRecord.Expiration <= DateTime.UtcNow)
            {
                return new AuthResultDto { Success = false, Message = "Süresi dolmuş refresh token." };
            }

            var user = await _userManager.FindByIdAsync(userId);
            if (user == null)
            {
                return new AuthResultDto { Success = false, Message = "Kullanıcı bulunamadı." };
            }

            // Revoke old token
            tokenRecord.IsRevoked = true;
            _context.UserRefreshTokens.Update(tokenRecord);

            // Generate New Access Token (1 Hour)
            var expiration = DateTime.UtcNow.AddHours(1);
            var newTokenString = GenerateAccessToken(user, expiration);

            // Generate New Refresh Token (5 Days)
            var newRefreshTokenString = GenerateRefreshTokenString();
            var newRefreshTokenExpiration = DateTime.UtcNow.AddDays(5);

            var newRefreshTokenEntity = new UserRefreshToken
            {
                Id = Guid.NewGuid(),
                UserId = user.Id,
                Token = newRefreshTokenString,
                Expiration = newRefreshTokenExpiration,
                IsRevoked = false,
                CreatedAt = DateTime.UtcNow
            };

            await _context.UserRefreshTokens.AddAsync(newRefreshTokenEntity);
            await _context.SaveChangesAsync();

            return new AuthResultDto
            {
                Success = true,
                Token = newTokenString,
                Expiration = expiration,
                RefreshToken = newRefreshTokenString,
                RefreshTokenExpiration = newRefreshTokenExpiration
            };
        }

        public async Task RevokeTokenAsync(string refreshToken)
        {
            var tokenRecord = await _context.UserRefreshTokens
                .FirstOrDefaultAsync(x => x.Token == refreshToken && !x.IsRevoked);

            if (tokenRecord != null)
            {
                tokenRecord.IsRevoked = true;
                _context.UserRefreshTokens.Update(tokenRecord);
                await _context.SaveChangesAsync();
            }
        }

        private string GenerateAccessToken(IdentityUser user, DateTime expiration)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "PlatarSecretSecurityKeyThatNeedsToBeLongEnoughForHMACSHA256";
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PlatarIssuer";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PlatarAudience";

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(jwtSecret));
            var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

            var claims = new[]
            {
                new Claim(ClaimTypes.NameIdentifier, user.Id),
                new Claim(JwtRegisteredClaimNames.Sub, user.Id),
                new Claim(JwtRegisteredClaimNames.Email, user.Email ?? ""),
                new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString())
            };

            var token = new JwtSecurityToken(
                issuer: jwtIssuer,
                audience: jwtAudience,
                claims: claims,
                expires: expiration,
                signingCredentials: creds
            );

            return new JwtSecurityTokenHandler().WriteToken(token);
        }

        private string GenerateRefreshTokenString()
        {
            var randomNumber = new byte[64];
            using var rng = RandomNumberGenerator.Create();
            rng.GetBytes(randomNumber);
            return Convert.ToBase64String(randomNumber);
        }

        private ClaimsPrincipal? GetPrincipalFromExpiredToken(string token)
        {
            var jwtSecret = Environment.GetEnvironmentVariable("JWT_SECRET") ?? "PlatarSecretSecurityKeyThatNeedsToBeLongEnoughForHMACSHA256";
            var jwtIssuer = Environment.GetEnvironmentVariable("JWT_ISSUER") ?? "PlatarIssuer";
            var jwtAudience = Environment.GetEnvironmentVariable("JWT_AUDIENCE") ?? "PlatarAudience";
            var key = Encoding.UTF8.GetBytes(jwtSecret);

            var tokenValidationParameters = new TokenValidationParameters
            {
                ValidateIssuer = true,
                ValidateAudience = true,
                ValidateLifetime = false, // Critical: do not validate lifetime of expired token
                ValidateIssuerSigningKey = true,
                ValidIssuer = jwtIssuer,
                ValidAudience = jwtAudience,
                IssuerSigningKey = new SymmetricSecurityKey(key)
            };

            var tokenHandler = new JwtSecurityTokenHandler();
            try
            {
                var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);
                if (securityToken is not JwtSecurityToken jwtSecurityToken || 
                    !jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
                {
                    return null;
                }
                return principal;
            }
            catch
            {
                return null;
            }
        }
    }
}
