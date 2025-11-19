using System;

namespace AI_Marketplace.Domain.Entities
{
    public class GeneratedImage
    {
        public int Id { get; set; }
        public int CustomRequestId { get; set; }
        public string ImageUrl { get; set; } = string.Empty;
        public string Prompt { get; set; } = string.Empty;
        public DateTime GeneratedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public CustomRequest CustomRequest { get; set; } = null!;
    }
}