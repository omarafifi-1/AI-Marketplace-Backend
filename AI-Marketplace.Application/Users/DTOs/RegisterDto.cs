using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class RegisterUserDto
    {
        [Required]
        [EmailAddress]
        public string Email { get; set; }

        [Required]
        [StringLength(100, MinimumLength = 8)]
        public string Password { get; set; }

        [Required]
        public string UserName { get; set; }

        /// <summary>
        /// Role for the user. Valid values: Admin, Seller, Customer. Defaults to Customer if not provided.
        /// </summary>
        public string? Role { get; set; }
    }
}