using System;
using System.Collections.Generic;
using System.Text;

namespace AI_Marketplace.Application.CustomRequests.DTOs
{
    public class CustomRequestResponseDto
    {
        public int Id { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public int BuyerId { get; set; }
        public string BuyerName { get; set; } = string.Empty;
        public string Status { get; set; } = "Open";
        public int OffersCount { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }

        public string? ImageUrl { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
    }
}
