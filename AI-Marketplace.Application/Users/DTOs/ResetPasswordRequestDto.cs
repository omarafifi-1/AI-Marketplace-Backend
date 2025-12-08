using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;
        public string NewPassword { get; set; } = default!;
        public string ConfirmPassword { get; set; } = default!;
    }
}
