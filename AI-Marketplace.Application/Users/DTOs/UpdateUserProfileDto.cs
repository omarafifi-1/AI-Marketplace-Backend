using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Users.DTOs
{
    public class UpdateUserProfileDto
    {
        public string? FirstName { get; set; }
        public string? LastName { get; set; }

        public string? PhoneNumber { get; set; }
    }
}
