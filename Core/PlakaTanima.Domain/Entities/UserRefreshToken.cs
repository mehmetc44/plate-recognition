using System;

namespace PlakaTanima.Domain.Entities
{
    public class UserRefreshToken
    {
        public Guid Id { get; set; }
        public string UserId { get; set; } = null!;
        public string Token { get; set; } = null!;
        public DateTime Expiration { get; set; }
        public bool IsRevoked { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}
