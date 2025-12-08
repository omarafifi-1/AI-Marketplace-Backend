using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class ResetPasswordResponseDto
    {
        public bool Succeeded { get; set; }
        public string Message { get; set; } = default!;
    }
}
