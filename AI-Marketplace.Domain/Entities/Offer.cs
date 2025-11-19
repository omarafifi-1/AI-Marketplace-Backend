using System;
using System.Collections.Generic;

namespace AI_Marketplace.Domain.Entities
{
    public class Offer
    {
        public int Id { get; set; }
        public int CustomRequestId { get; set; }
        public int StoreId { get; set; }
        public decimal ProposedPrice { get; set; }
        public string? Message { get; set; }
        public int EstimatedDays { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Accepted, Rejected
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public CustomRequest CustomRequest { get; set; } = null!;
        public Store Store { get; set; } = null!;
        public Order? Order { get; set; }
    }
}