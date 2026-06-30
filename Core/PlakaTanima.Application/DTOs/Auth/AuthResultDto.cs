using System;

namespace PlakaTanima.Application.DTOs.Auth
{
    public class AuthResultDto
    {
        public bool Success { get; set; }
        public string? Token { get; set; }
        public DateTime? Expiration { get; set; }
        public string? RefreshToken { get; set; }
        public DateTime? RefreshTokenExpiration { get; set; }
        public string? Message { get; set; }
    }
}
