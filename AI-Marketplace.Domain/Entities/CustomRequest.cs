using AI_Marketplace.Domain.enums;
using System;
using System.Collections.Generic;

namespace AI_Marketplace.Domain.Entities
{
    public class CustomRequest
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public string Description { get; set; } = string.Empty;
        public int CategoryId { get; set; }
        public CustomRequestStatus Status { get; set; } = CustomRequestStatus.Open; //  Open,InProgress, Completed
        public string? ImageUrl { get; set; }
        public decimal? Budget { get; set; }
        public DateTime? Deadline { get; set; }
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? UpdatedAt { get; set; }

        // Navigation Properties
        public ApplicationUser Buyer { get; set; } = null!;
        public Category Category { get; set; } = null!;
        public ICollection<Offer> Offers { get; set; } = new List<Offer>();
        public ICollection<GeneratedImage> GeneratedImages { get; set; } = new List<GeneratedImage>();
    }
}