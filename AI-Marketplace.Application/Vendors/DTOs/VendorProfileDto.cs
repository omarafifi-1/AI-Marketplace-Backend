using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.Vendors.DTOs
{
    public class VendorProfileDto
    {
        public int Id { get; set; }
        public string StoreName { get; set; } = string.Empty;
        public string? Description { get; set; }
        public string? LogoUrl { get; set; }
        public string? BannerUrl { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
        public bool IsActive { get; set; } = true;
        public bool IsVerified { get; set; } = false;
        public DateOnly VerifiedAt { get; set; } = DateOnly.MinValue;
        public string VerifiedBy { get; set; } = string.Empty;
        public decimal Rating { get; set; } = 0;
        public int TotalReviews { get; set; } = 0;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }
    }
}
