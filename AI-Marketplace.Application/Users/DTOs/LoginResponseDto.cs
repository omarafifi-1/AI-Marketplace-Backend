using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class LoginResponseDto
    {
        public string Token { get; set; }
        public DateTime Expiration { get; set; }
        public UserResponseDto User { get; set; }
    }
}
