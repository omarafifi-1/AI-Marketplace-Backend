using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace AI_Marketplace.Application.Orders.DTOs
{
    public class CreateOrderDto
    {
        [MaxLength(500, ErrorMessage = "Shipping address cannot exceed 500 characters")]
        public string? ShippingAddress { get; set; }
    }

    // Kept for compatibility with existing references; not used in cart-based flow
    public class CreateOrderItemDto
    {
        [Required(ErrorMessage = "Product ID is required")]
        public int ProductId { get; set; }

        [Required(ErrorMessage = "Quantity is required")]
        [Range(1, int.MaxValue, ErrorMessage = "Quantity must be at least 1")]
        public int Quantity { get; set; }
    }
}
