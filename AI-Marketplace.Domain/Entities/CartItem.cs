using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace AI_Marketplace.Domain.Entities
{
    public class CartItem
    {
        public int Id { get; set; }
        [ForeignKey("Cart")]
        public int CartId { get; set; }
        [ForeignKey("Product")]
        public int ProductId { get; set; }
        public int Quantity { get; set; }
        public decimal UnitPrice { get; set; } 
        public DateTime AddedAt { get; set; } = DateTime.UtcNow;
        public DateTime UpdatedAt { get; set; } = DateTime.UtcNow;

        // Navigation Properties
        public Cart Cart { get; set; } = null!;
        public Product Product { get; set; } = null!;

        // Calculated Property
        public decimal TotalPrice => UnitPrice * Quantity;
    }
}