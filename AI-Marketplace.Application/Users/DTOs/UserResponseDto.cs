using System;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class UserResponseDto
    {
        public int Id { get; set; }
        public string UserName { get; set; }
        public string Email { get; set; }
        public string Role { get; set; }
        public DateTime CreatedAt { get; set; }
        public int? StoreId { get; set; }
        public string? StoreName { get; set; }
        public bool IsActive { get; set; }
    }
}