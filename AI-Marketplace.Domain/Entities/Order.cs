using System;
using System.Collections.Generic;
using AI_Marketplace.Domain.enums;

namespace AI_Marketplace.Domain.Entities
{
    public class Order
    {
        public int Id { get; set; }
        public int BuyerId { get; set; }
        public int StoreId { get; set; }
        public int? OfferId { get; set; }
        public decimal TotalAmount { get; set; }
        public string Status { get; set; } = "Pending"; // Pending, Processing, Shipped, Delivered, Cancelled

        // Keep original string shipping address for compatibility with existing handlers
        public string? ShippingAddress { get; set; }

        // New FK to Address entity (optional)
        public int? ShippingAddressId { get; set; }
        public Address? ShippingAddressEntity { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.UtcNow;
        public DateTime? DeliveredAt { get; set; }

        // Navigation Properties
        public ApplicationUser Buyer { get; set; } = null!;
        public Offer? Offer { get; set; }
        public Store Store { get; set; } = null!;
        public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
        public ICollection<Payment> Payments { get; set; } = new List<Payment>();
    }
}