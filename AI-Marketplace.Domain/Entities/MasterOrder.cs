using System;
using System.Collections.Generic;

namespace AI_Marketplace.Domain.Entities
{
    /// <summary>
    /// Represents a single checkout session that can contain multiple orders from different stores.
    /// Allows a user to pay once for all items in their cart, while still maintaining separate orders per store.
    /// </summary>
    public class MasterOrder
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Completed, Cancelled
        
        // Keep original string shipping address for compatibility
        public string? ShippingAddress { get; set; }
        
        // New FK to Address entity (optional)
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddressEntity { get; set; }
        
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;
        public DateTime? CompletedAt { get; set; }
        
        // Navigation Properties
        public ApplicationUser Buyer { get; set; } = null!;
        public ICollection<Order> ChildOrders { get; set; } = new List<Order>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}
