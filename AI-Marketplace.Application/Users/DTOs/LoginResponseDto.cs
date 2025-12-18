using System;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class LoginResponseDto
    {
        public string AccessToken { get; set; } = string.Empty;
        public string? RefreshToken { get; set; }
        public DateTime AccessTokenExpiration { get; set; }
        public DateTime RefreshTokenExpiration { get; set; }
        public UserResponseDto User { get; set; } = null!;
    }
}
