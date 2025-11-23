using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace AI_Marketplace.Application.Vendors.DTOs
{
    public class VendorEditDto
    {
        [Required]
        [StringLength(200), MinLength(3)]
        public string StoreName { get; set; } = string.Empty;
        [StringLength(500)]
        public string? Description { get; set; }
        [Url]
        public string? LogoUrl { get; set; }
        [Url]
        public string? BannerUrl { get; set; }
        [EmailAddress]
        public string? ContactEmail { get; set; }
        [Phone]
        public string? ContactPhone { get; set; }
        public string? Address { get; set; }
    }
}
