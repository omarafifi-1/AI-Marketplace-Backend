using AI_Marketplace.Application.Addresses.DTOs;
using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class GetUserProfileDto
    {
        public int Id { get; set; }

        // Identity fields
        public string UserName { get; set; } = string.Empty;
        public string Email { get; set; } = string.Empty;
        public string? PhoneNumber { get; set; }

        // Custom fields
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public List<CreateAddressDto> Addresses { get; set; } = new();

    }
}
