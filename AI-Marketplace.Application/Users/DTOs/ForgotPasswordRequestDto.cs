using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class ForgotPasswordRequestDto
    {
        public string Email { get; set; } = default!;
    }
}
