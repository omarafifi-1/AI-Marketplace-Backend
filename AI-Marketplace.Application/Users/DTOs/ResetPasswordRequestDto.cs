using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class ResetPasswordRequestDto
    {
        public string Email { get; set; } = default!;
        public string Token { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string NewPassword { get; set; } = default!;

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string ConfirmPassword { get; set; } = default!;
    }
}
